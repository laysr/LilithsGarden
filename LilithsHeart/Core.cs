using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Systems;

namespace LilithsHeart;

public static class Core
{
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

    // Added: Event that fires when LilithsHeart has finished initializing
    public static event Action? OnInitialized;

    static bool _initialized;
    public static bool IsReady => _initialized;

    internal static void OnInitialize()
    {
        if (_initialized) return;

        LilithsLogger.Info("LilithsHeart core initializing...");

        PrefabNameResolver.Initialize();

        _initialized = true;
        LilithsLogger.Info("LilithsHeart core initialized.");

        // Added: Fire event so modules know initialization is complete
        OnInitialized?.Invoke();
    }
}