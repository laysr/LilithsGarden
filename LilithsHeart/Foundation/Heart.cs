using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Config;
using LilithsHeart.Events;
using LilithsHeart.Prefabs;

// [CHANGED] Moved from LilithsHeart/ root → Foundation/.
//           Namespace updated: LilithsHeart → LilithsHeart.Foundation.

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

    internal static void OnInitialize()
    {
        if (_initialized) return;

        HeartLogger.Info(LOG_SOURCE, "Heart initializing...");

        // PrefabNameExporter runs first so the Names directory and JSON files
        // exist before PrefabNameResolver tries to load from them.
        // On subsequent boots, Export() is a near-no-op (all files exist).
        PrefabNameExporter.Export();

        PrefabNameResolver.Initialize();

        // LocalizationConfig loads after prefab names are resolved so that
        // future name-key validation is possible. Reads once from disk.
        LocalizationConfig.Initialize();

        _initialized = true;

        HeartLogger.Info(LOG_SOURCE, "Heart initialized.");
        OnInitialized?.Invoke();
    }
}