using System.Text.Json;
using AlbionConsole.Data;
using AlbionConsole.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlbionConsole.Services;

public class ItemImporter
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ItemImporter> _logger;
    private readonly HttpClient _httpClient;
    private const string AlbionApiBaseUrl = "https://east.albion-online-data.com/api/v2";
    
    private readonly string[] commonItems = new[]
    {
        "T4_BAG", "T5_BAG", "T6_BAG", "T7_BAG", "T8_BAG",
        "T4_CAPE", "T5_CAPE", "T6_CAPE", "T7_CAPE", "T8_CAPE",
        "T4_2H_ARCANESTAFF", "T5_2H_ARCANESTAFF", "T6_2H_ARCANESTAFF", "T7_2H_ARCANESTAFF", "T8_2H_ARCANESTAFF",
        "T4_HEAD_CLOTH_SET1", "T5_HEAD_CLOTH_SET1", "T6_HEAD_CLOTH_SET1", "T7_HEAD_CLOTH_SET1", "T8_HEAD_CLOTH_SET1"
    };

    public ItemImporter(ApplicationDbContext context, ILogger<ItemImporter> logger)
    {
        _context = context;
        _logger = logger;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task ImportAsync()
    {
        try
        {
            _logger.LogInformation("Starting item import from Albion Online API...");
            Console.WriteLine("Importing common items from Albion Online...");

            foreach (var itemId in commonItems)
            {
                Console.WriteLine($"Importing {itemId}...");
                var requestUrl = $"{AlbionApiBaseUrl}/stats/prices/{itemId}?locations=Caerleon,Bridgewatch,Fort Sterling,Lymhurst,Martlock,Thetford";
                Console.WriteLine($"Request URL: {requestUrl}");
                
                // Get market data for the item
                // Add delay between requests to avoid rate limiting
                await Task.Delay(1000); // 1 second delay
                
                var response = await _httpClient.GetAsync(requestUrl);
                Console.WriteLine($"Response status: {response.StatusCode}");
                    
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {content}");
                }
                
                // If we hit rate limit, wait longer and retry
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    Console.WriteLine("Rate limit hit, waiting 5 seconds before retry...");
                    await Task.Delay(5000);
                    response = await _httpClient.GetAsync(requestUrl);
                    Console.WriteLine($"Retry response status: {response.StatusCode}");
                }
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to get data for {itemId}: {response.StatusCode}");
                    continue;
                }

                var json = await response.Content.ReadAsStringAsync();
                var marketData = JsonSerializer.Deserialize<List<AlbionMarketResponse>>(json);

                if (marketData == null || !marketData.Any())
                {
                    _logger.LogWarning($"No market data found for {itemId}");
                    continue;
                }

                // Check if item exists in our database
                var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.UniqueName == itemId);
                if (existingItem == null)
                {
                    var item = new Item
                    {
                        UniqueName = itemId,
                        Name = GetReadableName(itemId),
                        DescriptionEN = $"Item {itemId} from Albion Online"
                    };

                    _context.Items.Add(item);
                    await _context.SaveChangesAsync();

                    // Add initial price history
                    foreach (var market in marketData)
                    {
                        var priceHistory = new PriceHistory
                        {
                            ItemId = item.ItemId,
                            Location = market.City,
                            Quality = market.Quality,
                            SellPriceMin = (int)market.SellPriceMin,
                            SellPriceMax = (int)market.SellPriceMax,
                            BuyPriceMin = (int)market.BuyPriceMin,
                            BuyPriceMax = (int)market.BuyPriceMax,
                            Timestamp = DateTime.UtcNow
                        };

                        _context.PriceHistories.Add(priceHistory);
                    }
                }
                else
                {
                    // Update existing item's price history
                    foreach (var market in marketData)
                    {
                        var priceHistory = new PriceHistory
                        {
                            ItemId = existingItem.ItemId,
                            Location = market.City,
                            Quality = market.Quality,
                            SellPriceMin = (int)market.SellPriceMin,
                            SellPriceMax = (int)market.SellPriceMax,
                            BuyPriceMin = (int)market.BuyPriceMin,
                            BuyPriceMax = (int)market.BuyPriceMax,
                            Timestamp = DateTime.UtcNow
                        };

                        _context.PriceHistories.Add(priceHistory);
                    }
                }

                await _context.SaveChangesAsync();
            }

            Console.WriteLine("Import completed successfully.");
            _logger.LogInformation("Item import completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing data from Albion Online API");
            throw;
        }
    }

    private string GetReadableName(string itemId)
    {
        // Convert item ID to readable name
        // Example: T4_BAG -> Tier 4 Bag
        var parts = itemId.Split('_');
        if (parts.Length < 2) return itemId;

        var tier = parts[0].Replace("T", "Tier ");
        var name = parts[1].ToLower();
        
        // Handle special cases
        name = name switch
        {
            "2H" => parts[2], // For weapons like T4_2H_ARCANESTAFF
            "HEAD" => $"{parts[3]} {parts[2]}", // For armor like T4_HEAD_CLOTH_SET1
            _ => name
        };

        return $"{tier} {char.ToUpper(name[0])}{name[1..]}";
    }
} 