using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using LilithsHeart.Config;
using LilithsHeart.Foundation;
using LilithsCookbook.Config;   // [CHANGED] was LilithsCookbook.Systems
using LilithsCookbook.Data;
using LilithsCookbook.Systems;

namespace LilithsCookbook;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("audaciousbovine.lilithsheart")]
public class CookbookPlugin : BasePlugin
{
    private const string LOG_SOURCE = "LilithsCookbook";

    public static CookbookRecipeData?  RecipeData  { get; private set; }
    public static CookbookStationData? StationData { get; private set; }

    public override void Load()
    {
        // [CHANGED] LilithsLogger → HeartLogger throughout.
        HeartLogger.Info(LOG_SOURCE, $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loading.");

        var configFile = new ConfigFile(HeartPaths.ModuleConfig("LilithsCookbook"), saveOnInit: true);

        // 1. Read BepInEx cfg bindings first — GenerateAllRecipes flag lives here.
        //    [CHANGED] CookbookConfig now in LilithsCookbook.Config namespace.
        CookbookConfig.Initialize(configFile);

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
        HeartLogger.Info(LOG_SOURCE, $"{MyPluginInfo.PLUGIN_NAME} unloaded.");
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