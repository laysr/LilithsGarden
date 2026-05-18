using System.Text.Json;
using LilithsHeart;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class CookbookLoader
{
    private const string LOG_SOURCE = "LilithsCookbook.CookbookLoader";

    static readonly JsonSerializerOptions _readOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>
    /// Reads and merges all *.json files from the Recipes folder into one CookbookRecipeData.
    /// Later files win on key collision — admins can override example entries in their own files.
    /// </summary>
    public static CookbookRecipeData LoadRecipes(string recipesDir)
    {
        var merged = new CookbookRecipeData();

        foreach (var file in GetJsonFiles(recipesDir))
        {
            var incoming = Deserialize<CookbookRecipeData>(file);
            if (incoming == null) continue;

            foreach (var (key, value) in incoming.Recipes)
                merged.Recipes[key] = value;
        }

        LilithsLogger.Info(LOG_SOURCE, $"Loaded {merged.Recipes.Count} recipe entries from {recipesDir}.");
        return merged;
    }

    /// <summary>
    /// Reads and merges all *.json files from the Stations folder into one CookbookStationData.
    /// Later files win on key collision.
    /// </summary>
    public static CookbookStationData LoadStations(string stationsDir)
    {
        var merged = new CookbookStationData();

        foreach (var file in GetJsonFiles(stationsDir))
        {
            var incoming = Deserialize<CookbookStationData>(file);
            if (incoming == null) continue;

            foreach (var (key, value) in incoming.Stations)
                merged.Stations[key] = value;
        }

        LilithsLogger.Info(LOG_SOURCE, $"Loaded {merged.Stations.Count} station entries from {stationsDir}.");
        return merged;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static IEnumerable<string> GetJsonFiles(string directory)
    {
        if (!Directory.Exists(directory))
        {
            LilithsLogger.Warning(LOG_SOURCE, $"Config directory not found: {directory}");
            return Enumerable.Empty<string>();
        }

        return Directory.GetFiles(directory, "*.json").OrderBy(f => f);
    }

    static T? Deserialize<T>(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var result = JsonSerializer.Deserialize<T>(json, _readOptions);

            if (result == null)
                LilithsLogger.Warning(LOG_SOURCE, $"File deserialized to null: {Path.GetFileName(filePath)}");

            return result;
        }
        catch (Exception ex)
        {
            LilithsLogger.Error(LOG_SOURCE, $"Failed to load {Path.GetFileName(filePath)}: {ex.Message}");
            return default;
        }
    }
}
