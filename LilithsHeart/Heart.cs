using LilithsHeart.Systems;
using ProjectM;
using Stunlock.Core;
using Unity.Entities;

namespace LilithsHeart;

public static class Heart
{
    private const string LOG_SOURCE = "LilithsHeart.Heart";

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

    internal static void OnInitialize()
    {
        if (_initialized) return;

        LilithsLogger.Info(LOG_SOURCE, "Heart initializing...");

        PrefabNameResolver.Initialize();

        _initialized = true;

        LilithsLogger.Info(LOG_SOURCE, "Heart initialized.");

        // [CHANGED] Fire OnWorldReady through the event bus so all
        //           subscribed modules know ECS is safe to use.
        HeartEventBus.Publish(new OnWorldReady());

        // Also fire the direct event for any systems that
        // subscribed before the bus was available.
        OnInitialized?.Invoke();

        // Log the module registry summary now that world is ready.
        HeartRegistry.LogSummary();
    }
}