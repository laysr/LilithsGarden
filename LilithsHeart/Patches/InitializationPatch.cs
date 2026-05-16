using HarmonyLib;
using ProjectM.Gameplay.WarEvents;

namespace LilithsHeart.Patches;

[HarmonyPatch(typeof(WarEventRegistrySystem), nameof(WarEventRegistrySystem.RegisterWarEventEntities))]
internal static class InitializationPatch
{
    static bool _initialized;

    [HarmonyPostfix]
    static void Postfix()
    {
        if (_initialized) return;
        _initialized = true;

        try
        {
            Core.OnInitialize();

            if (Core.IsReady)
                LilithsLogger.Info($"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} initialized successfully.");
        }
        catch (Exception ex)
        {
            LilithsLogger.Error($"{MyPluginInfo.PLUGIN_NAME} failed to initialize: {ex}");
        }
    }
}