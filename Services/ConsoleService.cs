using AlbionConsole.Data;
using AlbionConsole.Models;
using Microsoft.Extensions.Logging;

namespace AlbionConsole.Services;

public class ConsoleService
{
    private readonly AlbionApiService _albionApiService;
    private readonly DatabaseService _databaseService;
    private readonly ItemImporter _itemImporter;
    private readonly MailNotificationService _mailNotificationService;
    private readonly ILogger<ConsoleService> _logger;

    public ConsoleService(
        AlbionApiService albionApiService,
        DatabaseService databaseService,
        ItemImporter itemImporter,
        MailNotificationService mailNotificationService,
        ILogger<ConsoleService> logger)
    {
        _albionApiService = albionApiService;
        _databaseService = databaseService;
        _itemImporter = itemImporter;
        _mailNotificationService = mailNotificationService;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Albion Online Market Tracker ===");
            Console.WriteLine("1. Import items");
            Console.WriteLine("2. Track new item");
            Console.WriteLine("3. View tracked items");
            Console.WriteLine("4. Update prices");
            Console.WriteLine("5. View price history");
            Console.WriteLine("6. Exit");
            Console.Write("\nSelect an option: ");

            var choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ImportItemsAsync();
                        break;
                    case "2":
                        await TrackNewItemAsync();
                        break;
                    case "3":
                        await ViewTrackedItemsAsync();
                        break;
                    case "4":
                        await UpdatePricesAsync();
                        break;
                    case "5":
                        await ViewPriceHistoryAsync();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }

    private async Task ImportItemsAsync()
    {
        Console.WriteLine("\nImporting items...");
        await _itemImporter.ImportAsync();
        Console.WriteLine("Import completed. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task TrackNewItemAsync()
    {
        Console.WriteLine("\nEnter item name to search: ");
        var searchTerm = Console.ReadLine() ?? "";

        var items = await _databaseService.GetItemsAsync();
        var matchingItems = items
            .Where(i => i.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                       i.UniqueName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matchingItems.Any())
        {
            Console.WriteLine("No items found. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nMatching items:");
        for (int i = 0; i < matchingItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {matchingItems[i].Name}");
        }

        Console.Write("\nSelect item number (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 0 || selection > matchingItems.Count)
        {
            Console.WriteLine("Invalid selection. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (selection == 0) return;

        var selectedItem = matchingItems[selection - 1];

        Console.Write($"Enter location ({string.Join(", ", City.ValidCities)}): ");
        var location = Console.ReadLine() ?? "";
        
        var normalizedCity = City.NormalizeName(location);
        if (!City.IsValid(normalizedCity))
        {
            Console.WriteLine(City.GetSuggestion(location));
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (normalizedCity != location)
        {
            Console.WriteLine($"City name will be saved as: {normalizedCity}");
        }

        var trackedItem = new TrackedItem
        {
            ItemId = selectedItem.ItemId,
            Location = normalizedCity,
            Item = selectedItem
        };

        await _databaseService.SaveTrackedItemAsync(trackedItem);
        Console.WriteLine("Item tracked successfully. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewTrackedItemsAsync()
    {
        var trackedItems = await _databaseService.GetTrackedItemsAsync();

        if (!trackedItems.Any())
        {
            Console.WriteLine("\nNo tracked items. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nTracked Items:");
        foreach (var item in trackedItems)
        {
            var latestPrice = await _databaseService.GetLatestPriceAsync(item.ItemId, item.Location, 1);
            var priceInfo = latestPrice != null 
                ? $"Sell: {latestPrice.SellPriceMin}-{latestPrice.SellPriceMax}, Buy: {latestPrice.BuyPriceMin}-{latestPrice.BuyPriceMax}"
                : "No price data available";
            Console.WriteLine($"- {item.Item?.Name} ({item.Location}): {priceInfo}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private async Task UpdatePricesAsync()
    {
        Console.WriteLine("\nUpdating prices for all tracked items...");
        await _albionApiService.UpdatePricesForTrackedItemsAsync();
        Console.WriteLine("Update completed. Press any key to continue...");
        Console.ReadKey();
    }

    private async Task ViewPriceHistoryAsync()
    {
        var trackedItems = await _databaseService.GetTrackedItemsAsync();

        if (!trackedItems.Any())
        {
            Console.WriteLine("\nNo tracked items. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nSelect item to view price history:");
        for (int i = 0; i < trackedItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {trackedItems[i].Item?.Name} ({trackedItems[i].Location})");
        }

        Console.Write("\nSelect item number (or 0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int selection) || selection < 0 || selection > trackedItems.Count)
        {
            Console.WriteLine("Invalid selection. Press any key to continue...");
            Console.ReadKey();
            return;
        }

        if (selection == 0) return;

        var selectedItem = trackedItems[selection - 1];
        var history = await _databaseService.GetPriceHistoryForItemAsync(selectedItem.ItemId);

        Console.WriteLine($"\nPrice history for {selectedItem.Item?.Name} in {selectedItem.Location}:");
        foreach (var price in history)
        {
            Console.WriteLine($"{price.Timestamp:g}: Sell: {price.SellPriceMin}-{price.SellPriceMax}, Buy: {price.BuyPriceMin}-{price.BuyPriceMax}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
} 