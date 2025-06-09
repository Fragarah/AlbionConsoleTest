namespace AlbionConsole.Models;

public class PriceHistory
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string Location { get; set; } = string.Empty;
    public int Quality { get; set; }
    public int SellPriceMin { get; set; }
    public int SellPriceMax { get; set; }
    public int BuyPriceMin { get; set; }
    public int BuyPriceMax { get; set; }
    public DateTime Timestamp { get; set; }

    public Item? Item { get; set; }
} 