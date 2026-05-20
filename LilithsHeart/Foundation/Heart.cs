using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Config;
using LilithsHeart.Events;
using LilithsHeart.Network;
using LilithsHeart.Prefabs;

// [CHANGED] Added LilithsHeart.Network using for SyncPayloadCache.

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

    // [ADDED] Cached server identity used when rebuilding the sync payload
    //         after a LocalizationConfig.Reload(). Populated once at initialize.
    static string _serverIdentity = string.Empty;

    internal static void OnInitialize()
    {
        if (_initialized) return;

        HeartLogger.Info(LOG_SOURCE, "Heart initializing...");

        PrefabNameExporter.Export();
        PrefabNameResolver.Initialize();
        LocalizationConfig.Initialize();

        // [ADDED] Read server identity from GameSettings so the sync payload
        //         can use a stable, human-readable folder key on the Soul side.
        //         Falls back to "Unknown" if GameSettings isn't accessible yet
        //         (shouldn't happen at this point in the boot sequence).
        _serverIdentity = ResolveServerIdentity();

        // [ADDED] Build the compressed sync payload cache now that localization
        //         is loaded. Subsequent client connects just memcpy from this cache.
        SyncPayloadCache.Build(_serverIdentity);

        _initialized = true;

        HeartLogger.Info(LOG_SOURCE, "Heart initialized.");
        OnInitialized?.Invoke();
    }

    /// <summary>
    /// Rebuilds the sync payload cache with fresh localization data.
    /// Called by LocalizationConfig.Reload() after dictionaries are repopulated.
    /// </summary>
    internal static void OnLocalizationReloaded()
        => SyncPayloadCache.Rebuild(_serverIdentity);

    // ── Helpers ─────────────────────────────────────────────

    static string ResolveServerIdentity()
    {
        try
        {
            // GameSettings holds the server name configured by the server operator.
            // This is stable across restarts and human-readable — ideal as a folder key.
            var em       = EntityManager;
            var settings = em.CreateEntityQuery(typeof(GameSettings))
                             .GetSingleton<GameSettings>();

            var name = settings.ServerName.ToString();
            return string.IsNullOrWhiteSpace(name) ? "LilithsGarden" : name;
        }
        catch
        {
            HeartLogger.Warning(LOG_SOURCE,
                "Could not read server name from GameSettings — using 'LilithsGarden' as identity.");
            return "LilithsGarden";
        }
    }
}