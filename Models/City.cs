namespace AlbionConsole.Models;

public static class City
{
    public static readonly string[] ValidCities = {
        "Brecilien",
        "Caerleon",
        "Bridgewatch",
        "Fort Sterling",
        "Lymhurst",
        "Martlock",
        "Thetford"
    };

    public static readonly Dictionary<string, string> CommonMisspellings = new()
    {
        { "brecillien", "Brecilien" },
        { "brecillian", "Brecilien" },
        { "bracilien", "Brecilien" },
        { "bresilien", "Brecilien" },
        { "carleon", "Caerleon" },
        { "careleon", "Caerleon" },
        { "fortsterling", "Fort Sterling" },
        { "fort-sterling", "Fort Sterling" },
        { "ft sterling", "Fort Sterling" },
        { "ft. sterling", "Fort Sterling" },
        { "limhurst", "Lymhurst" },
        { "lynhurst", "Lymhurst" },
        { "thetfort", "Thetford" }
    };

    public static bool IsValid(string city)
    {
        return ValidCities.Contains(NormalizeName(city));
    }

    public static string NormalizeName(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return string.Empty;

        var normalized = city.Trim();
        
        // Check for exact match first (case-insensitive)
        var exactMatch = ValidCities.FirstOrDefault(c => c.Equals(normalized, StringComparison.OrdinalIgnoreCase));
        if (exactMatch != null)
            return exactMatch;

        // Check for common misspellings
        var key = normalized.ToLowerInvariant().Replace(" ", "");
        if (CommonMisspellings.TryGetValue(key, out var correctedName))
            return correctedName;

        // If no match found, return the original string
        return city;
    }

    public static string GetSuggestion(string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return "Please enter a city name.";

        var normalized = NormalizeName(city);
        if (IsValid(normalized))
            return $"'{city}' will be saved as '{normalized}'";

        return $"Invalid city name. Valid cities are: {string.Join(", ", ValidCities)}";
    }
} 