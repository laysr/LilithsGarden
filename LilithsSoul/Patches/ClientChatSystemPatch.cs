using HarmonyLib;
using ProjectM.Network;
using Unity.Entities;
using LilithsSoul.Foundation;
using LilithsSoul.Network;

// ============================================================
//  ClientChatSystemPatch — LilithsSoul
//
//  Intercepts incoming chat messages on the client to extract
//  LilithsGarden sync payload chunks before they reach the UI.
//
//  Hook target: ClientChatSystem.OnUpdate (postfix)
//  ─────────────────────────────────────────────────
//  ClientChatSystem processes ChatMessageServerEvent entities
//  each frame. We read the same query after the system runs
//  and check each message for our [[LG:N]] prefix.
//
//  Why postfix on OnUpdate instead of the event itself?
//  ─────────────────────────────────────────────────────
//  This mirrors the pattern used by Eclipse (ClientChatSystemPatch)
//  and ZUI. The system reads a query of ChatMessageServerEvent
//  entities — we do the same query after the system runs.
//  Patching OnUpdate postfix is safe and doesn't interfere with
//  the game's own chat rendering.
//
//  Consumed messages:
//  ──────────────────
//  Messages identified as LilithsGarden chunks are passed to
//  SyncReceiver. We then destroy the entity so the chunk text
//  never appears in the player's chat window.
//
//  [PERFORMANCE] Runs every frame but only does a string.StartsWith
//                check per message. Typically zero messages per frame
//                outside of a connect event — negligible cost.
// ============================================================

namespace LilithsSoul.Patches;

[HarmonyPatch(typeof(ClientChatSystem), nameof(ClientChatSystem.OnUpdate))]
internal static class ClientChatSystemPatch
{
    private const string LOG_SOURCE = "LilithsSoul.ClientChatSystemPatch";

    [HarmonyPostfix]
    static void Postfix(ClientChatSystem __instance)
    {
        try
        {
            var em = __instance.EntityManager;

            // Query for system messages — same type Heart sends.
            var query = em.CreateEntityQuery(
                ComponentType.ReadOnly<ChatMessageServerEvent>());

            var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

            try
            {
                foreach (var entity in entities)
                {
                    if (!em.HasComponent<ChatMessageServerEvent>(entity)) continue;

                    var chatEvent = em.GetComponentData<ChatMessageServerEvent>(entity);

                    // Only inspect system messages — player chat is a different type.
                    if (chatEvent.MessageType != ServerChatMessageType.System) continue;

                    string text = chatEvent.MessageText.Value;

                    if (SyncReceiver.TryHandleMessage(text))
                    {
                        // Consumed — destroy entity so it doesn't appear in chat UI.
                        em.DestroyEntity(entity);
                    }
                }
            }
            finally
            {
                entities.Dispose();
                query.Dispose();
            }
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"ClientChatSystemPatch failed: {ex.Message}");
        }
    }
}