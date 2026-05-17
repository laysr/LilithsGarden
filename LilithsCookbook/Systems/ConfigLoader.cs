using System.Text.Json;
using LilithsHeart;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class ConfigLoader
{
    private static readonly string ConfigPath = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsCookbook",
        "recipes-config.json"
    );

    // Changed: RecipeData -> CookbookRecipeData
    public static CookbookRecipeData Load()
    {
        if (!File.Exists(ConfigPath))
        {
            LilithsLogger.Warning($"recipes-config.json not found at {ConfigPath}, using vanilla values.");
            return new CookbookRecipeData();
        }

        try
        {
            var json = File.ReadAllText(ConfigPath);
            // Changed: RecipeData -> CookbookRecipeData
            var config = JsonSerializer.Deserialize<CookbookRecipeData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            LilithsLogger.Info($"Loaded recipes-config.json successfully.");
            return config ?? new CookbookRecipeData();
        }
        catch (Exception e)
        {
            LilithsLogger.Error($"Failed to load recipes-config.json: {e.Message}");
            return new CookbookRecipeData();
        }
    }
}