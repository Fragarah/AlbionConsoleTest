using Microsoft.EntityFrameworkCore;
using AlbionConsole.Data;
using AlbionConsole.Models;
using Microsoft.Extensions.Logging;

namespace AlbionConsole.Services;

public class DatabaseService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=albion.db")
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .LogTo(message => _logger.LogDebug(message))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _dbContext.Database.EnsureCreated();
        NormalizeCityNames().Wait();
    }

    private async Task NormalizeCityNames()
    {
        // Update TrackedItems
        var trackedItems = await _dbContext.TrackedItems.ToListAsync();
        var itemsUpdated = false;

        foreach (var item in trackedItems)
        {
            var normalizedCity = City.NormalizeName(item.Location);
            if (normalizedCity != item.Location && City.IsValid(normalizedCity))
            {
                _logger.LogInformation($"Normalizing city name from '{item.Location}' to '{normalizedCity}' in TrackedItems");
                item.Location = normalizedCity;
                itemsUpdated = true;
            }
        }

        // Update PriceHistories
        var priceHistories = await _dbContext.PriceHistories.ToListAsync();
        var pricesUpdated = false;

        foreach (var price in priceHistories)
        {
            var normalizedCity = City.NormalizeName(price.Location);
            if (normalizedCity != price.Location && City.IsValid(normalizedCity))
            {
                _logger.LogInformation($"Normalizing city name from '{price.Location}' to '{normalizedCity}' in PriceHistories");
                price.Location = normalizedCity;
                pricesUpdated = true;
            }
        }

        if (itemsUpdated || pricesUpdated)
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    // Item operations
    public async Task<List<Item>> GetItemsAsync()
    {
        return await _dbContext.Items.ToListAsync();
    }

    public async Task<Item?> GetItemByIdAsync(int id)
    {
        return await _dbContext.Items.FindAsync(id);
    }

    // TrackedItem operations
    public async Task<List<TrackedItem>> GetTrackedItemsAsync()
    {
        return await _dbContext.TrackedItems.Include(t => t.Item).ToListAsync();
    }

    public async Task SaveTrackedItemAsync(TrackedItem item)
    {
        // Validate and normalize city name
        var normalizedCity = City.NormalizeName(item.Location);
        if (!City.IsValid(normalizedCity))
        {
            throw new ArgumentException(City.GetSuggestion(item.Location));
        }
        item.Location = normalizedCity;

        _dbContext.TrackedItems.Add(item);
        await _dbContext.SaveChangesAsync();
    }

    // PriceHistory operations
    public async Task SavePriceHistoryAsync(PriceHistory priceHistory)
    {
        // Validate and normalize city name
        var normalizedCity = City.NormalizeName(priceHistory.Location);
        if (!City.IsValid(normalizedCity))
        {
            throw new ArgumentException(City.GetSuggestion(priceHistory.Location));
        }
        priceHistory.Location = normalizedCity;

        _dbContext.PriceHistories.Add(priceHistory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<PriceHistory?> GetLatestPriceAsync(int itemId, string location, int quality)
    {
        // Normalize city name for query
        var normalizedCity = City.NormalizeName(location);
        if (!City.IsValid(normalizedCity))
        {
            throw new ArgumentException(City.GetSuggestion(location));
        }

        return await _dbContext.PriceHistories
            .Where(p => p.ItemId == itemId && p.Location == normalizedCity && p.Quality == quality)
            .OrderByDescending(p => p.Timestamp)
            .FirstOrDefaultAsync();
    }

    public async Task<List<PriceHistory>> GetPriceHistoryForItemAsync(int itemId)
    {
        return await _dbContext.PriceHistories
            .Where(p => p.ItemId == itemId)
            .OrderByDescending(p => p.Timestamp)
            .ToListAsync();
    }
} 