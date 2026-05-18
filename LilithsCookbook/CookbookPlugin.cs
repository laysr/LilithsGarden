using BepInEx;
using BepInEx.Unity.IL2CPP;
using LilithsHeart;
using LilithsCookbook.Data;
using LilithsCookbook.Systems;

namespace LilithsCookbook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("audaciousbovine.lilithsheart")]
public class CookbookPlugin : BasePlugin
{
    private const string LOG_SOURCE = "LilithsCookbook";

    public static CookbookRecipeData? RecipeData { get; private set; }
    public static CookbookStationData? StationData { get; private set; }

    public override void Load()
    {
        LilithsLogger.Info(LOG_SOURCE, $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loading.");

        // 1. Read BepInEx cfg bindings first — GenerateAllRecipes flag lives here.
        CookbookConfig.Initialize(Config);

        // 2. Create config directories and write example files if missing.
        //    Safe to call here — no ECS access required.
        CookbookGenerator.Initialize();

        // 3. Subscribe to Heart.OnInitialized in order:
        //       a. Generate all-recipes.json if the flag is set (ECS read)
        //       b. Load merged config from disk into static properties
        //       c. Apply recipe and station changes to ECS
        Heart.OnInitialized += OnHeartInitialized;
    }

    public override bool Unload()
    {
        Heart.OnInitialized -= OnHeartInitialized;

        LilithsLogger.Info(LOG_SOURCE, $"{MyPluginInfo.PLUGIN_NAME} unloaded.");
        return true;
    }

    static void OnHeartInitialized()
    {
        // a. Generate all-recipes.json if requested — must run before loading
        //    so the newly written file is included in the merge if present.
        CookbookGenerator.GenerateAllRecipesIfRequested();

        // b. Load and merge all *.json files from Recipes/ and Stations/ folders.
        RecipeData  = CookbookLoader.LoadRecipes(CookbookGenerator.RecipesDir);
        StationData = CookbookLoader.LoadStations(CookbookGenerator.StationsDir);

        // c. Apply changes to ECS.
        RecipeSystem.ApplyChanges();
        StationSystem.ApplyChanges();
    }
}