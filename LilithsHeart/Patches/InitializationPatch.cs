using HarmonyLib;
using ProjectM.Gameplay.WarEvents;

namespace LilithsHeart.Patches;

[HarmonyPatch(typeof(WarEventRegistrySystem), nameof(WarEventRegistrySystem.RegisterWarEventEntities))]
internal static class InitializationPatch
{
    // [CHANGED] Updated Core references to Heart following rename
    private const string LOG_SOURCE = "LilithsHeart";

    static bool _initialized;

    [HarmonyPostfix]
    static void Postfix()
    {
        if (_initialized) return;
        _initialized = true;

        try
        {
            Heart.OnInitialize();

            if (Heart.IsReady)
                LilithsLogger.Info(LOG_SOURCE, $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} initialized successfully.");
        }
        catch (Exception ex)
        {
            LilithsLogger.Error(LOG_SOURCE, $"{MyPluginInfo.PLUGIN_NAME} failed to initialize: {ex}");
        }
    }
}