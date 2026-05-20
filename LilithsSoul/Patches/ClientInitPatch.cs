using HarmonyLib;
using ProjectM;
using LilithsSoul.Foundation;
using LilithsSoul.Network;

// ============================================================
//  ClientInitPatch — LilithsSoul
//
//  Detects when the client ECS world and prefabs are fully loaded
//  and safe to query. Notifies SyncReceiver so any queued payload
//  can be applied immediately.
//
//  Hook target: GameDataManager.LoadGameData (postfix)
//  ───────────────────────────────────────────────────
//  This fires after all prefabs and game data are loaded on the
//  client — the correct moment for localization injection and
//  future prefab patching. The player hasn't entered the world
//  yet so there are no visible side effects from patching here.
//
//  Single-fire guard ensures we only initialize once per session.
//  On world teardown (disconnect) _initialized resets so the next
//  session starts fresh.
//
//  [PERFORMANCE] Postfix runs once per session load.
//                SyncReceiver.NotifyWorldReady() is O(1) if no
//                pending payload, or the cost of one Apply() call.
// ============================================================

namespace LilithsSoul.Patches;

[HarmonyPatch(typeof(GameDataManager), nameof(GameDataManager.LoadGameData))]
internal static class ClientInitPatch
{
    private const string LOG_SOURCE = "LilithsSoul.ClientInitPatch";

    static bool _initialized;

    [HarmonyPostfix]
    static void Postfix()
    {
        if (_initialized) return;
        _initialized = true;

        try
        {
            SoulLogger.Info(LOG_SOURCE, "Client world ready.");
            SyncReceiver.NotifyWorldReady();
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"ClientInitPatch failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Resets the initialization flag when the client disconnects.
    /// Called from SoulPlugin.OnDisconnect (not yet implemented —
    /// add a NetworkManager disconnect hook when needed).
    /// </summary>
    internal static void Reset() => _initialized = false;
}