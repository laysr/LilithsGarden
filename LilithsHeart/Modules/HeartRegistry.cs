using System.Collections.Generic;
using LilithsHeart.Foundation;

// ============================================================
//  HeartRegistry — LilithsHeart Module Registration Hub
//
//  [CHANGED] Moved from Registry/ → Modules/.
//            Namespace updated: LilithsHeart.Registry → LilithsHeart.Modules.
//
//  Purpose: Each module announces itself here on load. This gives
//  Heart and other modules awareness of what is loaded without
//  hard assembly references.
//
//  Usage in another module's Plugin.Load():
//
//      HeartRegistry.Register(new ModuleInfo
//      {
//          ModuleId   = "audaciousbovine.lilithsbounty",
//          ModuleName = "LilithsBounty",
//          Version    = "0.1.0"
//      });
//
//  Checking for optional integrations:
//
//      if (HeartRegistry.IsLoaded("audaciousbovine.lilithstreasury"))
//          // wire up Treasury integration
//
//  [PERFORMANCE] Registry is a small in-memory dictionary.
//  IsLoaded() is O(1) — no concern.
// ============================================================

namespace LilithsHeart.Modules;

public static class HeartRegistry
{
    private const string LOG_SOURCE = "LilithsHeart.HeartRegistry";

    private static readonly Dictionary<string, ModuleInfo> _modules = new();

    // Optional callback fired when any module registers.
    // Useful for late-arriving optional integrations — a module
    // can watch for a partner it depends on optionally.
    public static event Action<ModuleInfo>? OnModuleRegistered;

    public static void Initialize()
    {
        _modules.Clear();
        OnModuleRegistered = null;
        HeartLogger.Info(LOG_SOURCE, "HeartRegistry initialized.");
    }

    public static void Shutdown()
    {
        _modules.Clear();
        OnModuleRegistered = null;
        HeartLogger.Info(LOG_SOURCE, "HeartRegistry shut down.");
    }

    /// <summary>
    /// Register a LilithsHeart module.
    /// Call this in your module's Plugin.Load().
    /// Safe to call multiple times with the same ID — later calls update the entry.
    /// </summary>
    public static void Register(ModuleInfo info)
    {
        _modules[info.ModuleId] = info;
        HeartLogger.Info(LOG_SOURCE, $"Registered: {info.ModuleName} v{info.Version}");
        OnModuleRegistered?.Invoke(info);
    }

    /// <summary>
    /// Returns true if the module with the given ID is loaded.
    /// Use this to guard optional cross-module integrations.
    /// </summary>
    public static bool IsLoaded(string moduleId)
        => _modules.ContainsKey(moduleId);

    /// <summary>
    /// Returns the ModuleInfo for a module, or null if not found.
    /// </summary>
    public static ModuleInfo? GetModule(string moduleId)
        => _modules.TryGetValue(moduleId, out var info) ? info : null;

    /// <summary>
    /// Returns all currently registered modules.
    /// </summary>
    public static IReadOnlyList<ModuleInfo> GetAllModules()
        => _modules.Values.ToList().AsReadOnly();

    /// <summary>
    /// Logs a formatted summary of all loaded modules.
    /// Called automatically on world ready.
    /// </summary>
    public static void LogSummary()
    {
        HeartLogger.Info(LOG_SOURCE, "=== LilithsHeart Modules ===");

        if (_modules.Count == 0)
        {
            HeartLogger.Info(LOG_SOURCE, "  (no modules registered)");
            return;
        }

        foreach (var m in _modules.Values.OrderBy(m => m.ModuleName))
            HeartLogger.Info(LOG_SOURCE, $"  {m.ModuleName} v{m.Version}");

        HeartLogger.Info(LOG_SOURCE, $"  Total: {_modules.Count} module(s)");
        HeartLogger.Info(LOG_SOURCE, "=============================");
    }
}