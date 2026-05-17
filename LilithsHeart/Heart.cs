using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Systems;

namespace LilithsHeart;

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

    internal static void OnInitialize()
    {
        if (_initialized) return;

        LilithsLogger.Info(LOG_SOURCE, "Heart initializing...");

        // [CHANGED] Export runs first so the Names directory and JSON files exist
        //           before PrefabNameResolver tries to load from them.
        //           On subsequent boots, Export() is a near-no-op (all files exist).
        PrefabNamesExporter.Export();

        PrefabNameResolver.Initialize();
        _initialized = true;

        LilithsLogger.Info(LOG_SOURCE, "Heart initialized.");
        OnInitialized?.Invoke();
    }
}