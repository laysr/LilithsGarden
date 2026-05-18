using BepInEx.Configuration;
using LilithsHeart;

namespace LilithsCookbook.Systems;

public static class CookbookConfig
{
    private const string LOG_SOURCE = "LilithsCookbook.CookbookConfig";

    static ConfigEntry<bool> _generateAllRecipes = null!;

    // Read directly from the ConfigEntry rather than caching via Lazy<T>,
    // since DisableGenerateAllRecipes() mutates the value at runtime.
    public static bool GenerateAllRecipes => _generateAllRecipes.Value;

    public static void Initialize(ConfigFile config)
    {
        _generateAllRecipes = config.Bind(
            section:      "Generation",
            key:          "GenerateAllRecipes",
            defaultValue: false,
            description:  "When set to true, generates a JSON file containing all existing vanilla recipes " +
                          "with ChangesEnabled set to false. The file will be written to " +
                          "BepInEx/config/LilithsHeart/LilithsCookbook/Recipes/all-recipes.json on next boot. " +
                          "This setting will automatically reset to false after generation."
        );

        LilithsLogger.Info(LOG_SOURCE, $"CookbookConfig loaded. GenerateAllRecipes={GenerateAllRecipes}");
    }

    /// <summary>
    /// Resets GenerateAllRecipes to false in the config file after generation completes.
    /// Called automatically by CookbookGenerator after all-recipes.json is written.
    /// </summary>
    public static void DisableGenerateAllRecipes()
    {
        _generateAllRecipes.Value = false;
        LilithsLogger.Info(LOG_SOURCE, "GenerateAllRecipes reset to false.");
    }
}