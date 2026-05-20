using HarmonyLib;
using ProjectM;
using LilithsSoul.Foundation;
using LilithsSoul.Network;

// ============================================================
//  ClientInitPatch — LilithsSoul
//
//  Detects when the client ECS world and prefabs are fully loaded.
//
//  Hook target: GameDataManager.OnUpdate (postfix, single-fire)
//  ──────────────────────────────────────────────────────────────
//  GameDataManager.OnUpdate fires every frame once initialized,
//  so we guard with a bool and only act once. This is the
//  established pattern for client-side V Rising mods.
//
//  We cannot use LoadGameData as it doesn't exist on the client-
//  side GameDataManager. OnUpdate fires after all prefabs and
//  game data are ready.
//
//  [PERFORMANCE] Guard check is a single bool read per frame
//                until initialization — negligible cost.
//                After first fire the patch is effectively free.
// ============================================================

namespace LilithsSoul.Patches;

[HarmonyPatch(typeof(GameDataManager), nameof(GameDataManager.OnUpdate))]
internal static class ClientInitPatch
{
    private const string LOG_SOURCE = "LilithsSoul.ClientInitPatch";

    static bool _initialized;

    [HarmonyPostfix]
    static void Postfix(GameDataManager __instance)
    {
        if (_initialized) return;

        // Guard: only fire once game data is actually loaded.
        // GameDataManager.GameDataInitialized is the reliable flag.
        if (!__instance.GameDataInitialized) return;

        _initialized = true;

        try
        {
            SoulLogger.Info(LOG_SOURCE, "Client world ready — game data initialized.");
            SyncReceiver.NotifyWorldReady();
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"ClientInitPatch failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Resets the initialization flag on client disconnect.
    /// </summary>
    internal static void Reset() => _initialized = false;
}