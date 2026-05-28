using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Config;
using LilithsHeart.Events;
using LilithsHeart.Network;
using LilithsHeart.Modules;
using LilithsMind.Network;

namespace LilithsHeart.Foundation;

// ============================================================
//  Heart — LilithsHeart
//
//  Central access point for server ECS systems and the module
//  registration/sync pipeline.
//
//  Lifecycle:
//  ──────────
//  1. OnInitialize() fires via WarEventRegistrySystem patch.
//  2. PrefabNameResolver and LocalizationConfig are initialized.
//  3. A baseline SyncPayloadCache is built (empty overrides).
//  4. OnInitialized event fires — registered modules apply their
//     changes and call Register*() to queue overrides.
//  5. Payload is rebuilt with all accumulated overrides.
//  6. OnWorldReady event fires for late subscribers.
//
//  [PERFORMANCE] System accessors (PrefabCollectionSystem,
//                GameDataSystem) are fetched per-call via Unity's
//                GetExistingSystemManaged — cached internally by Unity.
//                No static system references that could go stale.
// ============================================================

public static class Heart
{
    private const string LOG_SOURCE = "LilithsHeart";

    static World? _server;
    static World Server
    {
        get
        {
            if (_server?.IsCreated == true)
                return _server;
            _server = WorldUtility.FindServerWorld();
            if (_server?.IsCreated != true)
                throw new InvalidOperationException("Server world is not ready yet.");
            return _server;
        }
    }

    public static EntityManager EntityManager
        => Server.EntityManager;

    public static PrefabCollectionSystem PrefabCollectionSystem
        => Server.GetExistingSystemManaged<PrefabCollectionSystem>();

    public static GameDataSystem GameDataSystem
        => Server.GetExistingSystemManaged<GameDataSystem>();

    public static event Action? OnInitialized;
    static bool _initialized;
    public static bool IsReady => _initialized;

    static string _serverIdentity = string.Empty;

    static readonly Dictionary<string, RecipeOverrideData>        _pendingRecipeOverrides        = new();
    static readonly Dictionary<string, StationRecipeOverrideData> _pendingStationRecipeOverrides = new();
    static readonly List<string>                                   _pendingPlayerRecipesToAdd     = new();
    static readonly List<string>                                   _pendingPlayerRecipesToRemove  = new();

    // ── Lifecycle ─────────────────────────────────────────────

    internal static void OnInitialize()
    {
        if (_initialized) return;

        HeartLogger.Info(LOG_SOURCE, "Heart initializing...");

        PrefabNameResolver.Initialize();
        LocalizationConfig.Initialize();

        _serverIdentity = ResolveServerIdentity();

        // Build a baseline payload before modules register overrides.
        SyncPayloadCache.Rebuild(_serverIdentity,
            _pendingRecipeOverrides,
            _pendingStationRecipeOverrides,
            _pendingPlayerRecipesToAdd,
            _pendingPlayerRecipesToRemove);

        _initialized = true;

        HeartLogger.Info(LOG_SOURCE, "Heart initialized.");

        // Fire OnInitialized — modules apply their changes and call Register*().
        OnInitialized?.Invoke();

        // Rebuild payload after all modules have registered their overrides.
        bool needsRebuild = _pendingRecipeOverrides.Count        > 0 ||
                            _pendingStationRecipeOverrides.Count  > 0 ||
                            _pendingPlayerRecipesToAdd.Count      > 0 ||
                            _pendingPlayerRecipesToRemove.Count   > 0;

        if (needsRebuild)
        {
            HeartLogger.Info(LOG_SOURCE,
                $"Rebuilding sync payload with " +
                $"{_pendingRecipeOverrides.Count} recipe override(s), " +
                $"{_pendingStationRecipeOverrides.Count} station override(s), " +
                $"{_pendingPlayerRecipesToAdd.Count} player add(s), " +
                $"{_pendingPlayerRecipesToRemove.Count} player remove(s).");

            SyncPayloadCache.Rebuild(_serverIdentity,
                _pendingRecipeOverrides,
                _pendingStationRecipeOverrides,
                _pendingPlayerRecipesToAdd,
                _pendingPlayerRecipesToRemove);
        }

        HeartRegistry.LogSummary();
        HeartEventBus.Publish(new OnWorldReady());
    }

    internal static void OnLocalizationReloaded()
        => SyncPayloadCache.Rebuild(_serverIdentity,
            _pendingRecipeOverrides,
            _pendingStationRecipeOverrides,
            _pendingPlayerRecipesToAdd,
            _pendingPlayerRecipesToRemove);

    internal static void OnDestroy()
    {
        _initialized    = false;
        _server         = null;
        _serverIdentity = string.Empty;
        _pendingRecipeOverrides.Clear();
        _pendingStationRecipeOverrides.Clear();
        _pendingPlayerRecipesToAdd.Clear();
        _pendingPlayerRecipesToRemove.Clear();
        OnInitialized = null;
        HeartLogger.Info(LOG_SOURCE, "Heart destroyed.");
    }

    // ── Module registration API ───────────────────────────────

    /// <summary>
    /// Called by RecipeSystem after applying recipe ECS changes.
    /// Queues overrides for inclusion in the next ServerSyncPayload build.
    ///
    /// [PERFORMANCE] Called once at startup per module — O(n) over overrides.
    /// </summary>
    public static void RegisterRecipeOverrides(Dictionary<string, RecipeOverrideData> overrides)
    {
        foreach (var (key, value) in overrides)
            _pendingRecipeOverrides[key] = value;

        HeartLogger.Debug(LOG_SOURCE,
            $"RegisterRecipeOverrides: +{overrides.Count} entries, total={_pendingRecipeOverrides.Count}");
    }

    /// <summary>
    /// Called by StationSystem after patching WorkstationRecipesBuffer station
    /// entities. Queues station overrides so Soul can patch placed workstation
    /// entities client-side to match server-side display.
    ///
    /// [PERFORMANCE] Called once at startup per WorkstationRecipesBuffer station.
    /// </summary>
    public static void RegisterStationRecipeChanges(string stationName, List<string> toAdd, List<string> toRemove)
    {
        if (!_pendingStationRecipeOverrides.TryGetValue(stationName, out var existing))
        {
            existing = new StationRecipeOverrideData();
            _pendingStationRecipeOverrides[stationName] = existing;
        }

        foreach (var r in toAdd)
            if (!existing.RecipesToAdd.Contains(r))
                existing.RecipesToAdd.Add(r);

        foreach (var r in toRemove)
            if (!existing.RecipesToRemove.Contains(r))
                existing.RecipesToRemove.Add(r);

        HeartLogger.Debug(LOG_SOURCE,
            $"RegisterStationRecipeChanges: '{stationName}' +{toAdd.Count} add, +{toRemove.Count} remove.");
    }

    /// <summary>
    /// Called by StationSystem after patching live User entities.
    /// Queues player recipe changes so Soul can patch the client player
    /// entity WorkstationRecipesBuffer to match the server.
    ///
    /// [PERFORMANCE] Called once at startup per player crafting entry.
    ///               Lists are accumulated — later calls append, not replace.
    /// </summary>
    public static void RegisterPlayerRecipeChanges(List<string> toAdd, List<string> toRemove)
    {
        foreach (var r in toAdd)
            if (!_pendingPlayerRecipesToAdd.Contains(r))
                _pendingPlayerRecipesToAdd.Add(r);

        foreach (var r in toRemove)
            if (!_pendingPlayerRecipesToRemove.Contains(r))
                _pendingPlayerRecipesToRemove.Add(r);

        HeartLogger.Debug(LOG_SOURCE,
            $"RegisterPlayerRecipeChanges: +{toAdd.Count} add, +{toRemove.Count} remove.");
    }

    // ── Internal helpers ──────────────────────────────────────

    static string ResolveServerIdentity()
    {
        var name = HeartConfig.ServerName.Value;
        return string.IsNullOrWhiteSpace(name) ? "LilithsGarden" : name;
    }
}