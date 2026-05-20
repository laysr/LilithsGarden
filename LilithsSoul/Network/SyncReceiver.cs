using System.Text.Json;
using LilithsSoul.Config;
using LilithsSoul.Foundation;
using LilithsSoul.Localization;

// ============================================================
//  SyncReceiver — LilithsSoul
//
//  Intercepts system chat messages from Heart and reassembles
//  the chunked ServerSyncPayload.
//
//  Why chat messages?
//  ──────────────────
//  Unity.Netcode's CustomMessagingManager requires types not
//  available in VampireReferenceAssemblies. The established
//  pattern across shipped V Rising mods (Eclipse, ZUI, XPShared)
//  is to use ChatMessageServerEvent / ServerChatMessageType.System
//  for server→client data. SyncReceiver integrates with that
//  pattern by being called from a Harmony patch on the client
//  chat system (ClientChatSystemPatch).
//
//  Chunk protocol:
//  ───────────────
//  Heart sends N messages of the form [[LG:N]]<content>,
//  followed by a final [[LG:end]] sentinel.
//  SyncReceiver accumulates content strings in order and
//  processes the full JSON when end is received.
//
//  Integration point:
//  ──────────────────
//  ClientChatSystemPatch calls TryHandleMessage(string) for
//  every incoming system message. Returns true if the message
//  was a LilithsGarden chunk (consumed), false otherwise
//  (passed through to normal chat handling).
//
//  [PERFORMANCE] Per-message check is a string.StartsWith call
//                on the hot chat path — effectively free.
//                Deserialization and disk I/O only run once per
//                connect when [[LG:end]] is received.
// ============================================================

namespace LilithsSoul.Network;

public static class SyncReceiver
{
    private const string LOG_SOURCE  = "LilithsSoul.SyncReceiver";
    private const string CHUNK_PREFIX = "[[LG:";
    private const string CHUNK_END    = "[[LG:end]]";

    // Accumulated chunk content strings, in arrival order.
    static readonly List<string> _chunks = [];

    // Set by ClientInitPatch when client ECS world is ready.
    static bool _clientWorldReady;

    // Payload received before world was ready — applied on world ready.
    static ServerSyncPayload? _pendingPayload;

    // ── Called from ClientChatSystemPatch ────────────────────

    /// <summary>
    /// Inspects an incoming system message. If it is a LilithsGarden
    /// chunk, accumulates it and returns true (consumed).
    /// Returns false for unrelated messages.
    /// </summary>
    public static bool TryHandleMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return false;

        // End sentinel — reassemble and process.
        if (message == CHUNK_END)
        {
            ProcessAccumulatedChunks();
            return true;
        }

        // Numbered chunk — extract content after [[LG:N]].
        if (message.StartsWith(CHUNK_PREFIX))
        {
            int closeBracket = message.IndexOf("]]", CHUNK_PREFIX.Length,
                StringComparison.Ordinal);

            if (closeBracket >= 0)
            {
                string content = message[(closeBracket + 2)..];
                _chunks.Add(content);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Called by ClientInitPatch when the client ECS world is ready.
    /// Applies any payload that arrived before prefabs were loaded.
    /// </summary>
    public static void NotifyWorldReady()
    {
        _clientWorldReady = true;

        // Build the prefab name → AssetGuid lookup now that
        // PrefabCollectionSystem is available.
        LocalizationInjector.BuildLookupTable();

        if (_pendingPayload != null)
        {
            SoulLogger.Info(LOG_SOURCE,
                "Client world now ready — applying queued sync payload.");
            ApplyPayload(_pendingPayload);
            _pendingPayload = null;
        }
    }

    // ── Internal ─────────────────────────────────────────────

    static void ProcessAccumulatedChunks()
    {
        if (_chunks.Count == 0)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "Received [[LG:end]] but chunk list is empty — ignoring.");
            return;
        }

        var json = string.Concat(_chunks);
        _chunks.Clear();

        try
        {
            var payload = JsonSerializer.Deserialize<ServerSyncPayload>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                SoulLogger.Warning(LOG_SOURCE, "Deserialized payload is null — ignoring.");
                return;
            }

            SoulLogger.Info(LOG_SOURCE,
                $"Sync payload received from '{payload.ServerIdentity}' " +
                $"(hash: {payload.PayloadHash}).");

            WriteToDiskIfChanged(payload);

            if (_clientWorldReady)
                ApplyPayload(payload);
            else
                _pendingPayload = payload;
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"Failed to process sync payload: {ex.Message}");
            _chunks.Clear();
        }
    }

    static void ApplyPayload(ServerSyncPayload payload)
    {
        LocalizationInjector.Inject(payload);
    }

    static void WriteToDiskIfChanged(ServerSyncPayload payload)
    {
        var syncFile = SoulPaths.SyncFile(payload.ServerIdentity);

        if (File.Exists(syncFile))
        {
            try
            {
                var existing = JsonSerializer.Deserialize<ServerSyncPayload>(
                    File.ReadAllText(syncFile),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (existing?.PayloadHash == payload.PayloadHash)
                {
                    SoulLogger.Debug(LOG_SOURCE,
                        $"Payload hash unchanged ({payload.PayloadHash}) — skipping disk write.");
                    return;
                }
            }
            catch
            {
                // Malformed cache — overwrite below.
            }
        }

        try
        {
            Directory.CreateDirectory(SoulPaths.ServerDir(payload.ServerIdentity));
            File.WriteAllText(syncFile,
                JsonSerializer.Serialize(payload,
                    new JsonSerializerOptions { WriteIndented = true }));

            SoulLogger.Info(LOG_SOURCE, $"Sync payload cached to '{syncFile}'.");
        }
        catch (Exception ex)
        {
            SoulLogger.Warning(LOG_SOURCE,
                $"Failed to write sync payload to disk: {ex.Message}");
        }
    }
}