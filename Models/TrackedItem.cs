namespace AlbionConsole.Models;

public class TrackedItem
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Location { get; set; } = string.Empty;

    public Item? Item { get; set; }
} 