namespace AlbionConsole.Models;

public class Item
{
    public int ItemId { get; set; }
    public string UniqueName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DescriptionEN { get; set; } = string.Empty;

    public ICollection<TrackedItem> TrackedItems { get; set; } = new List<TrackedItem>();
    public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
} 