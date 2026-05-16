using System.Text.Json;
using LilithsHeart;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class ConfigLoader
{
    private static readonly string ConfigPath = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsGarden",
        "LilithsCookbook",
        "recipes.json"
    );

    public static RecipeConfig Load()
    {
        if (!File.Exists(ConfigPath))
        {
            LilithsLogger.Warning($"recipes.json not found at {ConfigPath}, using vanilla values.");
            return new RecipeConfig();
        }

        try
        {
            var json = File.ReadAllText(ConfigPath);
            var config = JsonSerializer.Deserialize<RecipeConfig>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            LilithsLogger.Info($"Loaded recipes.json successfully.");
            return config ?? new RecipeConfig();
        }
        catch (Exception e)
        {
            LilithsLogger.Error($"Failed to load recipes.json: {e.Message}");
            return new RecipeConfig();
        }
    }
}