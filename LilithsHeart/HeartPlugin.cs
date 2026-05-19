using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LilithsHeart.Config;
using LilithsHeart.Systems;

namespace LilithsHeart;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("gg.deca.VampireCommandFramework")]
public class HeartPlugin : BasePlugin
{
    static Harmony? _harmony;

    public override void Load()
    {
        LilithsLogger.Initialize(base.Log);

        try
        {
            LilithsLogger.Info("LilithsHeart", $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded.");

            // [CHANGED] We no longer pass base.Config to HeartConfig.
            //
            //           BepInEx names its auto-generated ConfigFile after the plugin GUID,
            //           which produces "audaciousbovine.lilithsheart.cfg" — not ideal for
            //           server admins browsing the config folder.
            //
            //           Instead we create our own ConfigFile pointed at:
            //               BepInEx/config/LilithsHeart/LilithsHeart.cfg
            //
            //           The GUID (audaciousbovine.lilithsheart) is intentionally left
            //           unchanged — it is used by BepInEx for dependency resolution
            //           between modules and must remain stable.
            //
            //           Child modules should follow the same pattern:
            //               new ConfigFile(HeartPaths.ModuleConfig("LilithsCookbook"), saveOnInit: true)
            //           which produces BepInEx/config/LilithsHeart/LilithsCookbook.cfg
            //
            // [PERFORMANCE] ConfigFile constructor reads the file from disk once here.
            //               All subsequent Bind() calls are in-memory only.
            var configFile = new ConfigFile(HeartPaths.CoreConfig, saveOnInit: true);

            HeartConfig.Initialize(configFile);
            HeartEventBus.Initialize();
            HeartRegistry.Initialize();

            _harmony?.UnpatchSelf();
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            LilithsLogger.Info("LilithsHeart", "LilithsHeart is ready.");
        }
        catch (Exception ex)
        {
            LilithsLogger.Error("LilithsHeart", $"Failed to load: {ex.Message}");
        }
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        HeartEventBus.Shutdown();
        HeartRegistry.Shutdown();

        LilithsLogger.Info("LilithsHeart", "LilithsHeart unloaded.");
        return true;
    }
}