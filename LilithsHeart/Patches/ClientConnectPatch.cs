using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using LilithsHeart.Foundation;
using LilithsHeart.Network;

// ============================================================
//  ClientConnectPatch — LilithsHeart
//
//  Detects when a client successfully joins the server and
//  sends them the cached ServerSyncPayload via SyncSender.
//
//  Hook target: ServerBootstrapSystem.OnUserConnected
//  ──────────────────────────────────────────────────
//  Fires after authentication is complete and the client is
//  ready to receive network messages.
//
//  [PERFORMANCE] Postfix runs once per client connect.
//                Send is a memcpy from cache — negligible cost.
// ============================================================

namespace LilithsHeart.Patches;

[HarmonyPatch(typeof(ServerBootstrapSystem), nameof(ServerBootstrapSystem.OnUserConnected))]
internal static class ClientConnectPatch
{
    private const string LOG_SOURCE = "LilithsHeart.ClientConnectPatch";

    [HarmonyPostfix]
    static void Postfix(ServerBootstrapSystem __instance, NetConnectionId netConnectionId)
    {
        try
        {
            if (!Heart.IsReady)
            {
                HeartLogger.Warning(LOG_SOURCE,
                    "Client connected before Heart was ready — sync payload not sent.");
                return;
            }

            // Resolve connection → approved user index → Netcode client ID.
            if (!__instance._NetEndPointToApprovedUserIndex.TryGetValue(
                    netConnectionId, out int userIndex))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"Could not resolve connection to user index.");
                return;
            }

            var serverClient = __instance._ApprovedUsersLookup[userIndex];

            // The Netcode client ID is the Index of the User entity.
            ulong clientId = (ulong)serverClient.UserEntity.Index;

            HeartLogger.Info(LOG_SOURCE,
                $"Client {clientId} connected — sending sync payload.");

            SyncSender.SendSyncToClient(clientId);
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"ClientConnectPatch failed: {ex.Message}");
        }
    }
}