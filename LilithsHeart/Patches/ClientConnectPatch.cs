using HarmonyLib;
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
//  This fires after the client has been authenticated and their
//  User entity is created — safe to send network messages at
//  this point. The clientId on the User component maps directly
//  to the Unity Netcode client ID used by CustomMessagingManager.
//
//  Why not OnUserCreated?
//  ──────────────────────
//  OnUserCreated fires before the connection is fully established.
//  OnUserConnected fires after the client is ready to receive
//  messages, which is what we need.
//
//  [PERFORMANCE] Postfix runs once per client connect.
//                The actual send is a memcpy from cache —
//                negligible cost on the server main thread.
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
            // Guard: Heart must be fully initialized before we can send.
            // If a client connects before Heart is ready (unlikely but possible
            // on a slow first boot), we skip — they won't have Soul data for
            // this session. They will receive it on reconnect.
            if (!Heart.IsReady)
            {
                HeartLogger.Warning(LOG_SOURCE,
                    "Client connected before Heart was ready — sync payload not sent. " +
                    "Client should reconnect to receive server config.");
                return;
            }

            // Resolve the NetConnectionId to a Unity Netcode client ID.
            // ServerBootstrapSystem maintains a mapping of connection → user data.
            if (!__instance._NetEndPointToApprovedUserIndex.TryGetValue(
                    netConnectionId, out int userIndex))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"Could not resolve connection {netConnectionId} to user index.");
                return;
            }

            var serverClient = __instance._ApprovedUsersLookup[userIndex];
            ulong clientId   = serverClient.UserEntity.Index; // Netcode client ID

            HeartLogger.Info(LOG_SOURCE,
                $"Client {clientId} connected — sending sync payload.");

            SyncSender.SendSyncToClient(clientId);
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE,
                $"ClientConnectPatch failed: {ex.Message}");
        }
    }
}