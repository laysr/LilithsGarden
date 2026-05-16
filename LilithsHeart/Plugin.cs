using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LilithsHeart;

namespace LilithsHeart;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    static Harmony? _harmony;

    public override void Load()
    {
        LilithsLogger.Initialize(base.Log);
        LilithsLogger.Info($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loaded.");

        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        _harmony.PatchAll();
    }

    public override bool Unload()
    {
        _harmony?.UnpatchSelf();
        return true;
    }
}