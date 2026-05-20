using HarmonyLib;
using ProjectM;
using ProjectM.Network;
using Unity.Entities;
using LilithsHeart.Foundation;
using LilithsHeart.Network;

// ============================================================
//  ClientConnectPatch — LilithsHeart
//
//  Detects when a client successfully joins and sends them the
//  cached ServerSyncPayload via SyncSender.
//
//  Hook target: ServerBootstrapSystem.OnUserConnected
//  ──────────────────────────────────────────────────
//  Fires after authentication is complete and the User entity
//  exists. We read the User and Character entities from the
//  approved user lookup — both are needed to construct the
//  ChatMessageServerEvent that carries our payload chunks.
//
//  [CHANGED] No longer uses Unity Netcode clientId.
//  SyncSender now uses ChatMessageServerEvent (system messages)
//  which requires User + Character entity references, not a
//  Netcode transport ID.
//
//  [PERFORMANCE] Runs once per client connect — negligible cost.
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
                    "Client connected before Heart was ready — sync not sent. " +
                    "Client should reconnect to receive server config.");
                return;
            }

            // Resolve connection → approved user index.
            if (!__instance._NetEndPointToApprovedUserIndex.TryGetValue(
                    netConnectionId, out int userIndex))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"Could not resolve connection {netConnectionId} to user index.");
                return;
            }

            var serverClient  = __instance._ApprovedUsersLookup[userIndex];
            Entity userEntity = serverClient.UserEntity;

            // Resolve the player's character entity from their User component.
            // Character may be null if the player hasn't spawned yet — guard it.
            var user = userEntity.Read<User>();
            Entity characterEntity = user.LocalCharacter.GetEntityOnServer();

            if (characterEntity == Entity.Null)
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"Character entity null for user {user.CharacterName} — " +
                    "payload deferred. Client should reconnect.");
                return;
            }

            HeartLogger.Info(LOG_SOURCE,
                $"Client '{user.CharacterName}' connected — sending sync payload.");

            SyncSender.SendSyncToClient(userEntity, characterEntity);
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"ClientConnectPatch failed: {ex.Message}");
        }
    }
}