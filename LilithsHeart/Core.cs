using ProjectM;
using Stunlock.Core;
using Unity.Entities;

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

    static bool _initialized;
    public static bool IsReady => _initialized;

    internal static void OnInitialize()
    {
        if (_initialized) return;

        LilithsLogger.Info("LilithsHeart core initializing...");
        _initialized = true;
        LilithsLogger.Info("LilithsHeart core initialized.");
    }
}