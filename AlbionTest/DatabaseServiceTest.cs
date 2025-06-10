using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using AlbionConsole.Services;
using AlbionConsole.Data;
using AlbionConsole.Models;
using Microsoft.EntityFrameworkCore;

namespace AlbionTest;

public class DatabaseServiceTests
{
    private DatabaseService CreateService(out ApplicationDbContext context)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unikalna baza
            .Options;

        context = new ApplicationDbContext(options);
        var logger = new Mock<ILogger<DatabaseService>>().Object;

        return new DatabaseService(logger, context);
    }

    [Fact]
    public async Task SaveTrackedItemAsync_Should_Add_Item_When_Not_Duplicate()
    {
        // Arrange
        var service = CreateService(out var context);

        var item = new Item { ItemId = 1, Name = "Example Item", UniqueName = "example_item" };
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var trackedItem = new TrackedItem
        {
            ItemId = item.ItemId,
            Location = "Martlock"
        };

        // Act
        await service.SaveTrackedItemAsync(trackedItem);
        var result = await service.GetTrackedItemsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Martlock", result[0].Location);
    }

    [Fact]
    public void CanCreateDatabaseService()
    {
        var loggerMock = new Mock<ILogger<DatabaseService>>();
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        var service = new DatabaseService(loggerMock.Object, context);

        Assert.NotNull(service);
    }

    [Fact]
    public async Task SaveTrackedItemAsync_Should_Not_Add_Duplicate()
    {
        var service = CreateService(out var context);

        var item = new Item { ItemId = 2, Name = "Iron Sword", UniqueName = "iron_sword" };
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var trackedItem = new TrackedItem
        {
            ItemId = item.ItemId,
            Location = "Bridgewatch"
        };

        await service.SaveTrackedItemAsync(trackedItem);
        await service.SaveTrackedItemAsync(trackedItem); // drugi raz – duplikat

        var result = await service.GetTrackedItemsAsync();

        Assert.Single(result);
    }

    [Fact]
    public async Task RemoveTrackedItemAsync_Should_Remove_Item()
    {
        var service = CreateService(out var context);

        var item = new Item { ItemId = 3, Name = "Steel Axe", UniqueName = "steel_axe" };
        context.Items.Add(item);
        await context.SaveChangesAsync();

        var trackedItem = new TrackedItem
        {
            ItemId = item.ItemId,
            Location = "Fort Sterling"
        };

        await service.SaveTrackedItemAsync(trackedItem);
        var saved = (await service.GetTrackedItemsAsync()).First();

        await service.RemoveTrackedItemAsync(saved);
        var result = await service.GetTrackedItemsAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetItemsAsync_Should_Return_All_Items()
    {
        var service = CreateService(out var context);

        context.Items.AddRange(
            new Item { ItemId = 10, Name = "Cloth Helmet", UniqueName = "cloth_helmet" },
            new Item { ItemId = 11, Name = "Leather Armor", UniqueName = "leather_armor" }
        );
        await context.SaveChangesAsync();

        var result = await service.GetItemsAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetLast30DaysPriceHistoryGroupedByItemAsync_Should_Return_Grouped_Data()
    {
        var service = CreateService(out var context);

        var item = new Item { ItemId = 4, Name = "Battleaxe", UniqueName = "battleaxe" };
        context.Items.Add(item);

        context.PriceHistories.AddRange(Enumerable.Range(1, 35).Select(i =>
            new PriceHistory
            {
                ItemId = item.ItemId,
                Location = "Lymhurst",
                Quality = 1,
                SellPriceMin = 100 + i,
                SellPriceMax = 150 + i,
                BuyPriceMin = 80 + i,
                BuyPriceMax = 120 + i,
                Timestamp = DateTime.UtcNow.AddDays(-i)
            }));

        await context.SaveChangesAsync();

        var result = await service.GetLast30DaysPriceHistoryGroupedByItemAsync();

        Assert.Single(result);

        var entry = result.First();
        Assert.Equal("Battleaxe", entry.Key.Name);
        Assert.Equal(30, entry.Value.Count); // tylko ostatnie 30 dni
    }
}
