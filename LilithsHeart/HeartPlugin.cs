using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LilithsHeart.Config;
using LilithsHeart.Events;
using LilithsHeart.Foundation;
using LilithsHeart.Modules;

namespace LilithsHeart;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
public class HeartPlugin : BasePlugin
{
    static Harmony? _harmony;

    public override void Load()
    {
        // [CHANGED] LilithsLogger → HeartLogger throughout.
        HeartLogger.Initialize(base.Log);

        try
        {
            HeartLogger.Info("LilithsHeart", $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded.");

            // Config file named explicitly so admins see LilithsHeart.cfg rather than
            // the plugin GUID filename BepInEx would generate by default.
            // [PERFORMANCE] ConfigFile constructor reads from disk once here.
            //               All subsequent Bind() calls in HeartConfig are in-memory only.
            var configFile = new ConfigFile(HeartPaths.CoreConfig, saveOnInit: true);

            HeartConfig.Initialize(configFile);

            // [CHANGED] Updated namespace references: Systems → Events / Registry
            HeartEventBus.Initialize();
            HeartRegistry.Initialize();

            _harmony?.UnpatchSelf();
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            HeartLogger.Info("LilithsHeart", "LilithsHeart is ready.");
        }
        catch (Exception ex)
        {
            HeartLogger.Error("LilithsHeart", $"Failed to load: {ex.Message}");
        }
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        HeartEventBus.Shutdown();
        HeartRegistry.Shutdown();

        HeartLogger.Info("LilithsHeart", "LilithsHeart unloaded.");
        return true;
    }
}