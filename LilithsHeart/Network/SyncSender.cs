using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using LilithsHeart.Foundation;

// ============================================================
//  SyncSender — LilithsHeart
//
//  Sends network messages to Soul clients via Unity Netcode's
//  CustomMessagingManager (named messages).
//
//  Two channels:
//  ─────────────
//  • "LilithsGarden_Sync"  — ServerSyncPayload, sent once on connect.
//                            Uses SyncPayloadCache.CachedBytes directly
//                            — no serialization at send time.
//
//  • "LilithsGarden_Event" — ServerEventPayload, sent per trigger.
//                            Serialized and compressed fresh per send
//                            since event data is moment-specific.
//
//  Message format (both channels):
//  ────────────────────────────────
//  [ 4 bytes: int32 payload length ][ N bytes: GZip-compressed JSON ]
//
//  The length prefix lets Soul's reader know how many bytes to read
//  before attempting decompression.
//
//  [PERFORMANCE] Sync send: memcpy of cached bytes into FastBufferWriter.
//                Event send: JSON serialize + GZip per occurrence.
//                Both run on the server's main thread at low frequency
//                (connect events, game triggers) — cost is acceptable.
// ============================================================

namespace LilithsHeart.Network;

public static class SyncSender
{
    private const string LOG_SOURCE    = "LilithsHeart.SyncSender";
    public  const string SyncChannel   = "LilithsGarden_Sync";
    public  const string EventChannel  = "LilithsGarden_Event";

    // ── Sync payload (connect-time) ─────────────────────────

    /// <summary>
    /// Sends the cached ServerSyncPayload to a specific connecting client.
    /// Called from ClientConnectPatch when a client joins.
    /// No-ops if the cache hasn't been built yet (Heart not ready).
    /// </summary>
    public static void SendSyncToClient(ulong clientId)
    {
        var bytes = SyncPayloadCache.CachedBytes;

        if (bytes == null)
        {
            HeartLogger.Warning(LOG_SOURCE,
                $"Sync payload cache is empty — cannot send to client {clientId}. " +
                "Is Heart fully initialized?");
            return;
        }

        try
        {
            // FastBufferWriter: 4 bytes for length prefix + N bytes for payload.
            // [PERFORMANCE] Initial capacity pre-sized to avoid internal realloc.
            var writer = new FastBufferWriter(4 + bytes.Length, Unity.Collections.Allocator.Temp);

            using (writer)
            {
                writer.WriteValueSafe(bytes.Length);
                writer.WriteBytesSafe(bytes);

                NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                    SyncChannel,
                    clientId,
                    writer,
                    NetworkDelivery.ReliableFragmentedSequenced // large payload, order matters
                );
            }

            HeartLogger.Info(LOG_SOURCE,
                $"Sent sync payload to client {clientId} ({bytes.Length} bytes compressed).");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE,
                $"Failed to send sync payload to client {clientId}: {ex.Message}");
        }
    }

    // ── Event payload (trigger-based) ───────────────────────

    /// <summary>
    /// Sends a ServerEventPayload to a specific client.
    /// Serializes and compresses fresh — event data is moment-specific.
    /// </summary>
    public static void SendEventToClient(ulong clientId, ServerEventPayload payload)
        => SendEvent(clientId, payload, broadcast: false);

    /// <summary>
    /// Broadcasts a ServerEventPayload to all connected clients.
    /// Use for server-wide events (e.g. world boss killed).
    /// </summary>
    public static void BroadcastEvent(ServerEventPayload payload)
        => SendEvent(NetworkManager.ServerClientId, payload, broadcast: true);

    static void SendEvent(ulong clientId, ServerEventPayload payload, bool broadcast)
    {
        try
        {
            var bytes = Compress(JsonSerializer.Serialize(payload));

            var writer = new FastBufferWriter(4 + bytes.Length, Unity.Collections.Allocator.Temp);

            using (writer)
            {
                writer.WriteValueSafe(bytes.Length);
                writer.WriteBytesSafe(bytes);

                if (broadcast)
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessageToAll(
                        EventChannel,
                        writer,
                        NetworkDelivery.ReliableSequenced
                    );
                }
                else
                {
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                        EventChannel,
                        clientId,
                        writer,
                        NetworkDelivery.ReliableSequenced
                    );
                }
            }

            HeartLogger.Debug(LOG_SOURCE,
                $"Sent event {payload.Kind} to {(broadcast ? "all clients" : $"client {clientId}")} " +
                $"({bytes.Length} bytes).");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE,
                $"Failed to send event {payload.Kind}: {ex.Message}");
        }
    }

    // ── Compression ─────────────────────────────────────────

    static byte[] Compress(string json)
    {
        var inputBytes = Encoding.UTF8.GetBytes(json);
        using var output = new MemoryStream();
        using var gzip   = new GZipStream(output, CompressionLevel.Optimal);
        gzip.Write(inputBytes, 0, inputBytes.Length);
        gzip.Close();
        return output.ToArray();
    }
}