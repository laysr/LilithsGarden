using System.Text.Json;
using LilithsSoul.Config;
using LilithsSoul.Foundation;
using LilithsSoul.Services;
using LilithsMind.Network;

// ============================================================
//  SyncReceiver — LilithsSoul
//
//  Intercepts system chat messages from Heart and reassembles
//  the chunked ServerSyncPayload.
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
//  was a LilithsGarden chunk (consumed), false otherwise.
//
//  [CHANGED] NotifyWorldReady now accepts the connection string
//            from ClientBootstrapSystem. On world ready, Soul
//            loads ServerRegistry (servers.json), looks up the
//            connection string, and pre-applies the cached
//            sync.json BEFORE CharacterHUD builds. This eliminates
//            the UI timing race where the server payload arrives
//            after the UI is already drawn from stale vanilla data.
//
//  [CHANGED] ProcessAccumulatedChunks now calls
//            ServerRegistry.Register() when a payload arrives so
//            future connects can pre-apply without waiting.
//
//  [PERFORMANCE] Per-message check is a string.StartsWith call
//                on the hot chat path — effectively free.
//                Deserialization and disk I/O only run once per
//                connect when [[LG:end]] is received.
// ============================================================

namespace LilithsSoul.Network;

public static class SyncReceiver
{
    private const string LOG_SOURCE   = "LilithsSoul.SyncReceiver";
    private const string CHUNK_PREFIX = "[[LG:";
    private const string CHUNK_END    = "[[LG:end]]";

    // Accumulated chunk content strings, in arrival order.
    static readonly List<string> _chunks = [];

    // Set by ClientInitPatch when client ECS world is ready.
    static bool _clientWorldReady;

    // Payload received before world was ready — applied on world ready.
    static ServerSyncPayload? _pendingPayload;

    // [CHANGED] Connection string from ClientBootstrapSystem, set in NotifyWorldReady.
    // Used to register server→folder mapping when payload arrives.
    static string _connectionString = string.Empty;

    // ── Called from ClientChatSystemPatch ────────────────────

    /// <summary>
    /// Inspects an incoming system message. If it is a LilithsGarden
    /// chunk, accumulates it and returns true (consumed).
    /// Returns false for unrelated messages.
    /// </summary>
    public static bool TryHandleMessage(string message)
    {
        if (string.IsNullOrEmpty(message)) return false;

        if (message == CHUNK_END)
        {
            ProcessAccumulatedChunks();
            return true;
        }

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
    ///
    /// [CHANGED] Now accepts connectionString from ClientBootstrapSystem.
    /// Loads ServerRegistry, looks up the cached sync folder for this
    /// server, and pre-applies the cached sync.json immediately so
    /// patches are in place before CharacterHUD builds.
    /// </summary>
    public static void NotifyWorldReady(string connectionString)
    {
        _clientWorldReady  = true;
        _connectionString  = connectionString;

        // Build lookup tables now that PrefabCollectionSystem is available.
        LocalizationInjector.BuildLookupTable();
        RecipePatcher.BuildNameMap();

        // [CHANGED] Pre-apply cached sync.json from disk before the server
        //           payload arrives so the UI builds with correct data.
        //           This uses servers.json to map connection string → folder.
        TryPreApplyCachedSync(connectionString);

        if (_pendingPayload != null)
        {
            SoulLogger.Info(LOG_SOURCE,
                "Client world now ready — applying queued sync payload.");
            ApplyPayload(_pendingPayload);
            _pendingPayload = null;
        }
    }

    // ── Internal ─────────────────────────────────────────────

    /// <summary>
    /// Attempts to load and apply a cached sync.json from disk
    /// based on the current server connection string.
    ///
    /// [CHANGED] New method — pre-applies cached data before server
    ///           payload arrives to prevent UI timing race conditions.
    ///
    /// [PERFORMANCE] One disk read at world ready — negligible.
    /// </summary>
    static void TryPreApplyCachedSync(string connectionString)
    {
        ServerRegistry.Load();

        if (string.IsNullOrEmpty(connectionString))
        {
            SoulLogger.Debug(LOG_SOURCE,
                "No connection string available — cannot pre-apply cached sync.");
            return;
        }

        if (!ServerRegistry.TryGetFolderName(connectionString, out var folderName))
        {
            SoulLogger.Info(LOG_SOURCE,
                $"No cached sync for '{connectionString}' — waiting for server payload.");
            return;
        }

        var syncFile = SoulPaths.SyncFile(folderName);
        if (!File.Exists(syncFile))
        {
            SoulLogger.Info(LOG_SOURCE,
                $"Sync file not found for '{folderName}' — waiting for server payload.");
            return;
        }

        try
        {
            var json = File.ReadAllText(syncFile);
            var payload = JsonSerializer.Deserialize<ServerSyncPayload>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (payload == null)
            {
                SoulLogger.Warning(LOG_SOURCE,
                    $"Cached sync.json for '{folderName}' deserialized to null — skipping.");
                return;
            }

            SoulLogger.Info(LOG_SOURCE,
                $"Pre-applying cached sync for '{folderName}' " +
                $"(hash: {payload.PayloadHash}) before UI builds.");

            ApplyPayload(payload);
        }
        catch (Exception ex)
        {
            SoulLogger.Warning(LOG_SOURCE,
                $"Failed to pre-apply cached sync for '{folderName}': {ex.Message}");
        }
    }

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
                $"(hash: {payload.PayloadHash}, " +
                $"recipes: {payload.RecipeOverrides.Count}).");

            // [CHANGED] Register connection string → folder name mapping so
            //           future connects can pre-apply without waiting for payload.
            if (!string.IsNullOrEmpty(_connectionString))
                ServerRegistry.Register(_connectionString, payload.ServerIdentity);

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
        // 1. Localization — display names and tooltips.
        LocalizationInjector.Inject(payload);

        // 2. Recipe patching — ingredients, outputs, craft duration.
        RecipePatcher.Apply(payload.RecipeOverrides);

        // 3. Station recipe patching — WorkstationRecipesBuffer on placed stations.
        RecipePatcher.ApplyStationRecipes(payload.StationRecipeOverrides);

        // 4. Player recipe patching — add/remove from client player entity buffer.
        RecipePatcher.ApplyPlayerRecipes(payload.PlayerRecipesToAdd, payload.PlayerRecipesToRemove);
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