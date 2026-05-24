using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Config;
using LilithsHeart.Events;
using LilithsHeart.Network;
using LilithsHeart.Prefabs;
using LilithsHeart.Modules;

namespace LilithsHeart.Foundation;

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

    internal static void OnInitialize()
    {
        if (_initialized) return;

        HeartLogger.Info(LOG_SOURCE, "Heart initializing...");

        PrefabNameResolver.Initialize();
        LocalizationConfig.Initialize();

        _serverIdentity = ResolveServerIdentity();

        SyncPayloadCache.Build(_serverIdentity);

        _initialized = true;

        HeartLogger.Info(LOG_SOURCE, "Heart initialized.");

        // Fire the C# event first — modules subscribe to this in their Load()
        // and use it to run their own initialization against ECS.
        OnInitialized?.Invoke();

        // Log the registry summary after OnInitialized fires so that any module
        // that registers itself inside its OnInitialized handler is included.
        HeartRegistry.LogSummary();

        // Publish OnWorldReady to the event bus after all OnInitialized handlers
        // have run. Modules can subscribe to either pattern — the C# event
        // (OnInitialized) for direct coupling, or the bus (OnWorldReady) for
        // looser pub/sub. Both are supported.
        //
        // [PERFORMANCE] Publish dispatches synchronously to a snapshot of
        //               subscribers. Keep OnWorldReady handlers fast —
        //               no heavy ECS queries or I/O inside them.
        HeartEventBus.Publish(new OnWorldReady());
    }

    // [CHANGED] Added OnDestroy() to reset Heart state on world teardown.
    //           Called from HeartPlugin.Unload() so the next world load can
    //           fire OnInitialize() again cleanly.
    //           Publishes OnWorldDestroyed before resetting state so subscribers
    //           can act on it while Heart is still nominally valid.
    internal static void OnDestroy()
    {
        if (!_initialized) return;

        HeartLogger.Info(LOG_SOURCE, "Heart shutting down...");

        // Publish first — subscribers may need Heart.IsReady to still be true
        // during their cleanup (e.g. releasing cached entity references).
        HeartEventBus.Publish(new OnWorldDestroyed());

        _initialized    = false;
        _serverIdentity = string.Empty;
        _server         = null;

        HeartLogger.Info(LOG_SOURCE, "Heart shut down.");
    }

    internal static void OnLocalizationReloaded()
        => SyncPayloadCache.Rebuild(_serverIdentity);

    static string ResolveServerIdentity()
    {
        var name = HeartConfig.ServerName.Value;
        return string.IsNullOrWhiteSpace(name) ? "LilithsGarden" : name;
    }
}