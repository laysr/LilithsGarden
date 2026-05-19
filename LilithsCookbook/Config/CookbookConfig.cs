using BepInEx.Configuration;
using LilithsHeart.Foundation;

namespace LilithsCookbook.Config;

// [CHANGED] Moved from LilithsCookbook.Systems → LilithsCookbook.Config.
//           Config is not a system — it's configuration. This mirrors the
//           structure in LilithsHeart where HeartConfig lives in LilithsHeart.Config.
//           Any file that imports CookbookConfig must update its using statement.
//
// [CHANGED] LilithsLogger → HeartLogger throughout.
public static class CookbookConfig
{
    private const string LOG_SOURCE = "LilithsCookbook.CookbookConfig";

    static ConfigEntry<bool> _generateAllRecipes = null!;

    // Read directly from ConfigEntry.Value — no Lazy<T> needed.
    // ConfigEntry<T> caches the parsed value after the first read.
    public static bool GenerateAllRecipes => _generateAllRecipes.Value;

    public static void Initialize(ConfigFile config)
    {
        _generateAllRecipes = config.Bind(
            section:      "Generation",
            key:          "GenerateAllRecipes",
            defaultValue: false,
            description:  "When set to true, generates a JSON file containing all existing vanilla recipes " +
                          "with ChangesEnabled set to false. The file will be written to " +
                          "BepInEx/config/LilithsHeart/Recipes/all-recipes.json on next boot. " +
                          "This setting will automatically reset to false after generation."
        );

        HeartLogger.Info(LOG_SOURCE, $"CookbookConfig loaded. GenerateAllRecipes={GenerateAllRecipes}");
    }

    /// <summary>
    /// Resets GenerateAllRecipes to false in the config file after generation completes.
    /// Called automatically by CookbookGenerator after all-recipes.json is written.
    /// </summary>
    public static void DisableGenerateAllRecipes()
    {
        _generateAllRecipes.Value = false;
        HeartLogger.Info(LOG_SOURCE, "GenerateAllRecipes reset to false.");
    }
}