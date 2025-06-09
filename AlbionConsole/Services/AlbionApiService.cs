using System.Text.Json;
using System.Text.Json.Serialization;
using AlbionConsole.Models;

namespace AlbionConsole.Services;

public class AlbionApiService
{
    private readonly HttpClient _httpClient;
    private readonly DatabaseService _databaseService;
    private const string BaseUrl = "https://east.albion-online-data.com/api/v2";

    public AlbionApiService(DatabaseService databaseService)
    {
        _httpClient = new HttpClient();
        _databaseService = databaseService;
    }

    private async Task<string> GetWithRetryAsync(string url, int maxRetries = 3)
    {
        for (int i = 1; i <= maxRetries; i++)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"Response status: {response.StatusCode}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {content}");
                    return content;
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response content: {content}");
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    var waitTime = TimeSpan.FromSeconds(5 * i);
                    Console.WriteLine($"Rate limit hit, waiting {waitTime.TotalSeconds} seconds before retry...");
                    await Task.Delay(waitTime);
                    continue;
                }
                
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex) when (i < maxRetries)
            {
                Console.WriteLine($"Attempt {i} failed: {ex.Message}");
                await Task.Delay(TimeSpan.FromSeconds(2 * i));
                continue;
            }
        }
        
        throw new Exception($"Failed to get response after {maxRetries} attempts");
    }

    public async Task<PriceHistory?> GetItemPriceAsync(string uniqueName, string location, TrackedItem trackedItem)
    {
        try
        {
            var url = $"{BaseUrl}/stats/prices/{uniqueName}?locations={location}";
            Console.WriteLine($"Request URL: {url}");
            
            var response = await GetWithRetryAsync(url);
            Console.WriteLine($"Response: {response}");
            
            var prices = JsonSerializer.Deserialize<List<AlbionMarketResponse>>(response);

            if (prices == null || !prices.Any())
            {
                Console.WriteLine($"No price data available for {uniqueName} in {location}");
                return null;
            }

            var price = prices.First();
            Console.WriteLine($"Price data: SellMin={price.SellPriceMin}, SellMax={price.SellPriceMax}, BuyMin={price.BuyPriceMin}, BuyMax={price.BuyPriceMax}");

            // Save price to history
            var priceHistory = new PriceHistory
            {
                ItemId = trackedItem.Item.ItemId,
                Location = location,
                Quality = price.Quality,
                SellPriceMin = (int)Math.Max(0, price.SellPriceMin),
                SellPriceMax = (int)Math.Max(0, price.SellPriceMax),
                BuyPriceMin = (int)Math.Max(0, price.BuyPriceMin),
                BuyPriceMax = (int)Math.Max(0, price.BuyPriceMax),
                Timestamp = DateTime.UtcNow
            };

            await _databaseService.SavePriceHistoryAsync(priceHistory);
            Console.WriteLine($"Saved price history for {uniqueName} in {location}");

            return priceHistory;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting price for {uniqueName}: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            return null;
        }
    }

    public async Task UpdatePricesForTrackedItemsAsync()
    {
        try
        {
            var trackedItems = await _databaseService.GetTrackedItemsAsync();
            Console.WriteLine($"Found {trackedItems.Count} tracked items to update");

            foreach (var trackedItem in trackedItems)
            {
                if (trackedItem.Item == null)
                {
                    Console.WriteLine($"Skipping tracked item {trackedItem.Id} - no item data");
                    continue;
                }

                Console.WriteLine($"Updating prices for {trackedItem.Item.Name} ({trackedItem.Item.UniqueName}) in {trackedItem.Location}");

                try
                {
                    var priceHistory = await GetItemPriceAsync(
                        trackedItem.Item.UniqueName,
                        trackedItem.Location,
                        trackedItem
                    );

                    if (priceHistory != null)
                    {
                        Console.WriteLine($"Updated prices: Sell {priceHistory.SellPriceMin}-{priceHistory.SellPriceMax}, Buy {priceHistory.BuyPriceMin}-{priceHistory.BuyPriceMax}");
                    }
                    else
                    {
                        Console.WriteLine("Failed to get price data");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating prices for {trackedItem.Item.UniqueName}: {ex.Message}");
                }

                // Add small delay between requests to not overload the API
                Console.WriteLine("Waiting 2 seconds before next request...");
                await Task.Delay(2000);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating prices: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}

// Class for deserializing API response
public class AlbionMarketResponse
{
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;
    
    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;
    
    [JsonPropertyName("quality")]
    public int Quality { get; set; }
    
    [JsonPropertyName("sell_price_min")]
    public decimal SellPriceMin { get; set; }
    
    [JsonPropertyName("sell_price_max")]
    public decimal SellPriceMax { get; set; }
    
    [JsonPropertyName("buy_price_min")]
    public decimal BuyPriceMin { get; set; }
    
    [JsonPropertyName("buy_price_max")]
    public decimal BuyPriceMax { get; set; }
} 