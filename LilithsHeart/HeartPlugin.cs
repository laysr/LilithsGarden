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
        HeartLogger.Initialize(base.Log);

        try
        {
            HeartLogger.Info("LilithsHeart", $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded.");

            var configFile = new ConfigFile(HeartPaths.CoreConfig, saveOnInit: true);

            HeartConfig.Initialize(configFile);
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

        // [CHANGED] Call Heart.OnDestroy() before shutting down the bus and registry.
        //           This publishes OnWorldDestroyed to any remaining subscribers
        //           and resets Heart's initialized state so a future world load
        //           can fire OnInitialize() cleanly.
        //           Order: Heart teardown → bus shutdown → registry shutdown.
        //           Heart.OnDestroy() publishes to the bus, so the bus must still
        //           be alive when it runs.
        Heart.OnDestroy();

        HeartEventBus.Shutdown();
        HeartRegistry.Shutdown();

        HeartLogger.Info("LilithsHeart", "LilithsHeart unloaded.");
        return true;
    }
}