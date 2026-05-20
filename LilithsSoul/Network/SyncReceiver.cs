using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Unity.Netcode;
using LilithsSoul.Config;
using LilithsSoul.Foundation;
using LilithsSoul.Localization;

// ============================================================
//  SyncReceiver — LilithsSoul
//
//  Registers Unity Netcode named message handlers for both
//  Heart→Soul channels and processes incoming payloads.
//
//  Channels:
//  ─────────
//  "LilithsGarden_Sync"  — receives ServerSyncPayload once on connect.
//  "LilithsGarden_Event" — receives ServerEventPayload per trigger.
//
//  Sync payload flow:
//  ──────────────────
//  1. Receive compressed bytes from Heart.
//  2. Decompress + deserialize to ServerSyncPayload.
//  3. Compare PayloadHash against cached sync.json on disk.
//  4. If changed (or no cache): write sync.json to disk.
//  5. Apply localization — immediately if client world is ready,
//     or queue for ClientInitPatch to apply on world ready.
//
//  Event payload flow:
//  ───────────────────
//  1. Receive compressed bytes from Heart.
//  2. Decompress + deserialize to ServerEventPayload.
//  3. Route to appropriate handler by EventKind.
//     (No handlers defined yet — added per module.)
//
//  [PERFORMANCE] Decompression and deserialization run once per
//                connect (sync) or per trigger (event).
//                Neither is in a hot path.
//                Disk write only occurs when PayloadHash differs.
// ============================================================

namespace LilithsSoul.Network;

public static class SyncReceiver
{
    private const string LOG_SOURCE = "LilithsSoul.SyncReceiver";

    // Set by ClientInitPatch when the client world is ready.
    // Guards against injecting localization before prefabs are loaded.
    static bool _clientWorldReady;

    // Queued payload received before the world was ready.
    // Applied by NotifyWorldReady() when the world comes up.
    static ServerSyncPayload? _pendingPayload;

    /// <summary>
    /// Registers the named message handlers with Unity Netcode.
    /// Call from SoulPlugin.Load() — handlers must be registered
    /// before any message can arrive.
    /// </summary>
    public static void RegisterHandlers()
    {
        // Guard: NetworkManager may not be ready at plugin load time.
        // Handlers are registered here and will fire once messages arrive.
        var messaging = NetworkManager.Singleton?.CustomMessagingManager;

        if (messaging == null)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "CustomMessagingManager not available at handler registration time. " +
                "Ensure SyncReceiver.RegisterHandlers() is called after NetworkManager initializes.");
            return;
        }

        messaging.RegisterNamedMessageHandler(
            SyncSenderChannels.Sync,
            OnSyncReceived);

        messaging.RegisterNamedMessageHandler(
            SyncSenderChannels.Event,
            OnEventReceived);

        SoulLogger.Info(LOG_SOURCE, "Named message handlers registered.");
    }

    /// <summary>
    /// Unregisters handlers. Call from SoulPlugin.Unload().
    /// </summary>
    public static void UnregisterHandlers()
    {
        var messaging = NetworkManager.Singleton?.CustomMessagingManager;
        if (messaging == null) return;

        messaging.UnregisterNamedMessageHandler(SyncSenderChannels.Sync);
        messaging.UnregisterNamedMessageHandler(SyncSenderChannels.Event);

        SoulLogger.Info(LOG_SOURCE, "Named message handlers unregistered.");
    }

    /// <summary>
    /// Called by ClientInitPatch when the client ECS world is ready.
    /// Applies any payload that arrived before the world was up.
    /// </summary>
    public static void NotifyWorldReady()
    {
        _clientWorldReady = true;

        // [ADDED] Build the prefab name → AssetGuid lookup table now that
        //         the client world and PrefabCollectionSystem are available.
        //         Must happen before any payload is applied so injection
        //         has the lookup ready to use.
        LocalizationInjector.BuildLookupTable();

        if (_pendingPayload != null)
        {
            SoulLogger.Info(LOG_SOURCE,
                "Client world now ready — applying queued sync payload.");
            ApplyPayload(_pendingPayload);
            _pendingPayload = null;
        }
    }

    // ── Handlers ────────────────────────────────────────────

    static void OnSyncReceived(ulong senderId, FastBufferReader reader)
    {
        try
        {
            reader.ReadValueSafe(out int length);
            var bytes = new byte[length];
            reader.ReadBytesSafe(ref bytes, length);

            var json    = Decompress(bytes);
            var payload = JsonSerializer.Deserialize<ServerSyncPayload>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                SoulLogger.Warning(LOG_SOURCE, "Received null sync payload — ignoring.");
                return;
            }

            SoulLogger.Info(LOG_SOURCE,
                $"Received sync payload from server '{payload.ServerIdentity}' " +
                $"(hash: {payload.PayloadHash}).");

            // Write to disk only if hash differs from cached version.
            WriteToDiskIfChanged(payload);

            // Apply immediately if world is ready, otherwise queue.
            if (_clientWorldReady)
                ApplyPayload(payload);
            else
            {
                SoulLogger.Info(LOG_SOURCE,
                    "Client world not ready — queuing payload for application on world ready.");
                _pendingPayload = payload;
            }
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"Failed to process sync payload: {ex.Message}");
        }
    }

    static void OnEventReceived(ulong senderId, FastBufferReader reader)
    {
        try
        {
            reader.ReadValueSafe(out int length);
            var bytes = new byte[length];
            reader.ReadBytesSafe(ref bytes, length);

            var json    = Decompress(bytes);
            var payload = JsonSerializer.Deserialize<ServerEventPayload>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                SoulLogger.Warning(LOG_SOURCE, "Received null event payload — ignoring.");
                return;
            }

            SoulLogger.Debug(LOG_SOURCE, $"Received event: {payload.Kind}");
            RouteEvent(payload);
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"Failed to process event payload: {ex.Message}");
        }
    }

    // ── Apply ────────────────────────────────────────────────

    static void ApplyPayload(ServerSyncPayload payload)
    {
        LocalizationInjector.Inject(payload);
        // Future: PrefabPatcher.Apply(payload) when gameplay data is added.
    }

    static void RouteEvent(ServerEventPayload payload)
    {
        switch (payload.Kind)
        {
            case EventKind.None:
                SoulLogger.Warning(LOG_SOURCE, "Received event with Kind=None — ignoring.");
                break;

            // Future event handlers added here per module.
            // e.g. case EventKind.VBloodUnlocked: VBloodHandler.Handle(payload.Data); break;

            default:
                SoulLogger.Warning(LOG_SOURCE,
                    $"Unhandled event kind: {payload.Kind}. " +
                    "Is Soul up to date with Heart's EventKind enum?");
                break;
        }
    }

    // ── Disk cache ───────────────────────────────────────────

    static void WriteToDiskIfChanged(ServerSyncPayload payload)
    {
        var syncFile = SoulPaths.SyncFile(payload.ServerIdentity);

        // Check existing hash before writing.
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
                // Malformed cache file — overwrite it.
            }
        }

        // [PERFORMANCE] Disk write on connect — runs off hot path,
        //               acceptable cost. Only writes when hash differs.
        try
        {
            Directory.CreateDirectory(SoulPaths.ServerDir(payload.ServerIdentity));

            File.WriteAllText(syncFile,
                JsonSerializer.Serialize(payload,
                    new JsonSerializerOptions { WriteIndented = true }));

            SoulLogger.Info(LOG_SOURCE,
                $"Sync payload written to '{syncFile}'.");
        }
        catch (Exception ex)
        {
            SoulLogger.Warning(LOG_SOURCE,
                $"Failed to write sync payload to disk: {ex.Message}");
        }
    }

    // ── Decompression ────────────────────────────────────────

    static string Decompress(byte[] compressed)
    {
        using var input  = new MemoryStream(compressed);
        using var gzip   = new GZipStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}

/// <summary>
/// Named message channel strings — must match SyncSender.cs on the Heart side exactly.
/// Duplicated here to avoid any assembly reference to Heart from Soul.
/// </summary>
internal static class SyncSenderChannels
{
    public const string Sync  = "LilithsGarden_Sync";
    public const string Event = "LilithsGarden_Event";
}