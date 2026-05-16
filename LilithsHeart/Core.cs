using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Systems;

namespace LilithsHeart;

internal static class Core
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

    // Added: GameDataSystem for registering recipe and item changes
    public static GameDataSystem GameDataSystem
        => Server.GetExistingSystemManaged<GameDataSystem>();

    static bool _initialized;
    public static bool IsReady => _initialized;

internal static void OnInitialize()
{
    if (_initialized) return;

    LilithsLogger.Info("LilithsHeart core initializing...");

    // Added: Initialize name resolver so modules can look up GUIDs by name
    PrefabNameResolver.Initialize();

    _initialized = true;
    LilithsLogger.Info("LilithsHeart core initialized.");
}
}