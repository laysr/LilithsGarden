using Il2CppInterop.Runtime;
using ProjectM;
using ProjectM.Network;
using Unity.Collections;
using Unity.Entities;
using LilithsHeart.Foundation;

// ============================================================
//  SyncSender — LilithsHeart
//
//  Sends the ServerSyncPayload to connecting Soul clients using
//  V Rising's existing chat message infrastructure.
//
//  Why chat messages instead of CustomMessagingManager?
//  ─────────────────────────────────────────────────────
//  Unity.Netcode (FastBufferWriter, CustomMessagingManager etc.)
//  is not exposed by VampireReferenceAssemblies and the DLLs do
//  not exist on disk in an IL2CPP build. The established pattern
//  across all shipped V Rising client mods (Eclipse, ZUI, XPShared)
//  is to use ChatMessageServerEvent with ServerChatMessageType.System
//  for server→client data transport. Soul intercepts these on the
//  client side before they reach the chat UI.
//
//  Chunking:
//  ─────────
//  ChatMessageServerEvent.MessageText is FixedString512Bytes (~510
//  usable chars). We split the JSON payload across multiple messages,
//  each prefixed [[LG:N]] where N is the chunk index, and a final
//  [[LG:end]] sentinel. Soul reassembles in order.
//
//  Message format per chunk:
//      [[LG:0]]<chunk content>
//      [[LG:1]]<chunk content>
//      [[LG:end]]
//
//  [PERFORMANCE] Chunking runs once per client connect on the server
//                main thread. A typical localization payload will be
//                a few KB of JSON — roughly 10-20 messages.
//                Acceptable cost for a one-time connect event.
// ============================================================

namespace LilithsHeart.Network;

public static class SyncSender
{
    private const string LOG_SOURCE = "LilithsHeart.SyncSender";

    // Prefix Soul uses to identify LilithsGarden messages in chat stream.
    public const string CHUNK_PREFIX = "[[LG:";
    public const string CHUNK_END    = "[[LG:end]]";

    // FixedString512Bytes fits 510 chars. Reserve ~12 chars for the
    // [[LG:NNN]] prefix, leaving 450 for content with UTF-8 headroom.
    private const int MAX_CHUNK_CONTENT = 450;

    // [PERFORMANCE] Static readonly component arrays — allocated once.
    static readonly ComponentType[] _networkEventComponents =
    [
        ComponentType.ReadOnly(Il2CppType.Of<FromCharacter>()),
        ComponentType.ReadOnly(Il2CppType.Of<NetworkEventType>()),
        ComponentType.ReadOnly(Il2CppType.Of<SendNetworkEventTag>()),
        ComponentType.ReadOnly(Il2CppType.Of<ChatMessageServerEvent>())
    ];

    static readonly NetworkEventType _networkEventType = new()
    {
        IsAdminEvent = false,
        EventId      = NetworkEvents.EventId_ChatMessageServerEvent,
        IsDebugEvent = false,
    };

    // ── Public API ───────────────────────────────────────────

    /// <summary>
    /// Sends the cached sync payload to a connecting client.
    /// Called from ClientConnectPatch.
    /// </summary>
    public static void SendSyncToClient(Entity userEntity, Entity characterEntity)
    {
        var json = SyncPayloadCache.CachedJson;

        if (json == null)
        {
            HeartLogger.Warning(LOG_SOURCE,
                "Sync payload cache is empty — cannot send. Is Heart fully initialized?");
            return;
        }

        try
        {
            var chunks = Chunkify(json);
            var em     = Heart.EntityManager;

            for (int i = 0; i < chunks.Count; i++)
            {
                SendSystemMessage(em, userEntity, characterEntity,
                    $"{CHUNK_PREFIX}{i}]]{chunks[i]}");
            }

            // End sentinel — tells Soul to reassemble and process.
            SendSystemMessage(em, userEntity, characterEntity, CHUNK_END);

            HeartLogger.Info(LOG_SOURCE,
                $"Sync payload sent in {chunks.Count} chunk(s) + end sentinel.");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"SendSyncToClient failed: {ex.Message}");
        }
    }

    // ── Internal ─────────────────────────────────────────────

    static void SendSystemMessage(
        EntityManager em,
        Entity userEntity,
        Entity characterEntity,
        string text)
    {
        // Defensive truncation — should never trigger if MAX_CHUNK_CONTENT is correct.
        if (text.Length > 509) text = text[..509];

        ChatMessageServerEvent chatEvent = new()
        {
            MessageText   = new FixedString512Bytes(text),
            MessageType   = ServerChatMessageType.System,
            FromCharacter = characterEntity.GetNetworkId(),
            FromUser      = userEntity.GetNetworkId(),
            TimeUTC       = DateTime.UtcNow.Ticks
        };

        Entity entity = em.CreateEntity(_networkEventComponents);
        entity.Write(new FromCharacter { Character = characterEntity, User = userEntity });
        entity.Write(_networkEventType);
        entity.Write(chatEvent);
    }

    static List<string> Chunkify(string input)
    {
        var chunks = new List<string>();
        int pos    = 0;

        while (pos < input.Length)
        {
            int len = Math.Min(MAX_CHUNK_CONTENT, input.Length - pos);
            chunks.Add(input.Substring(pos, len));
            pos += len;
        }

        return chunks;
    }
}