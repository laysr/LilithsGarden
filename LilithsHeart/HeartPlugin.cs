using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

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

            // [CHANGED] Initialization sequence temporarily simplified.
            //           HeartConfig, EventBus, and ModuleRegistry will be
            //           added back as each system is written.
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

        LilithsLogger.Info("LilithsHeart", "LilithsHeart unloaded.");
        return true;
    }
}