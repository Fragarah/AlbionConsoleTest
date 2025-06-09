using System.Text.Json.Serialization;

namespace AlbionConsole.Models;

public class AlbionApiItem
{
    [JsonPropertyName("UniqueName")]
    public string UniqueName { get; set; } = string.Empty;

    [JsonPropertyName("LocalizedNames")]
    public LocalizedStrings? LocalizedNames { get; set; }

    [JsonPropertyName("LocalizedDescriptions")]
    public LocalizedStrings? LocalizedDescriptions { get; set; }
}

public class LocalizedStrings
{
    [JsonPropertyName("EN-US")]
    public string EN_US { get; set; } = string.Empty;

    [JsonPropertyName("DE-DE")]
    public string DE_DE { get; set; } = string.Empty;

    [JsonPropertyName("FR-FR")]
    public string FR_FR { get; set; } = string.Empty;

    [JsonPropertyName("RU-RU")]
    public string RU_RU { get; set; } = string.Empty;

    [JsonPropertyName("PL-PL")]
    public string PL_PL { get; set; } = string.Empty;

    [JsonPropertyName("ES-ES")]
    public string ES_ES { get; set; } = string.Empty;

    [JsonPropertyName("PT-BR")]
    public string PT_BR { get; set; } = string.Empty;

    [JsonPropertyName("ZH-CN")]
    public string ZH_CN { get; set; } = string.Empty;

    [JsonPropertyName("KO-KR")]
    public string KO_KR { get; set; } = string.Empty;
}

public class AlbionMarketResponse
{
    [JsonPropertyName("item_id")]
    public string ItemId { get; set; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; set; } = string.Empty;

    [JsonPropertyName("quality")]
    public int Quality { get; set; }

    [JsonPropertyName("sell_price_min")]
    public int SellPriceMin { get; set; }

    [JsonPropertyName("sell_price_min_date")]
    public DateTime SellPriceMinDate { get; set; }

    [JsonPropertyName("sell_price_max")]
    public int SellPriceMax { get; set; }

    [JsonPropertyName("sell_price_max_date")]
    public DateTime SellPriceMaxDate { get; set; }

    [JsonPropertyName("buy_price_min")]
    public int BuyPriceMin { get; set; }

    [JsonPropertyName("buy_price_min_date")]
    public DateTime BuyPriceMinDate { get; set; }

    [JsonPropertyName("buy_price_max")]
    public int BuyPriceMax { get; set; }

    [JsonPropertyName("buy_price_max_date")]
    public DateTime BuyPriceMaxDate { get; set; }
} 