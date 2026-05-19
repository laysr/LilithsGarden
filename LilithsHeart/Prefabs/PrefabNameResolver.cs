using System.Text.Json;
using Stunlock.Core;
using LilithsHeart.Config;
using LilithsHeart.Foundation;

namespace LilithsHeart.Prefabs;

// [CHANGED] Moved from Systems/ → Prefabs/.
//           Namespace updated: LilithsHeart.Systems → LilithsHeart.Prefabs.
//           PrefabNameEntry extracted to its own file (PrefabNameEntry.cs).
//           LilithsLogger → HeartLogger.
public static class PrefabNameResolver
{
    private const string LOG_SOURCE = "LilithsHeart.PrefabNameResolver";

    public static readonly PrefabGUID Empty = new PrefabGUID(0);

    // Name → GUID (forward lookups — used when loading config files)
    static readonly Dictionary<string, PrefabGUID> _newNameToGuid      = new();
    static readonly Dictionary<string, PrefabGUID> _originalNameToGuid = new();

    // GUID → name (reverse lookup — used by generators that iterate
    // ECS entities and need a human-readable name for each GUID).
    // Prefers NewName over OriginalName when both exist, so generated
    // files use the same admin-facing names as config inputs.
    static readonly Dictionary<int, string> _guidToName = new();

    static readonly string ConfigDir = HeartPaths.NamesDir;

    public static void Initialize()
    {
        if (!Directory.Exists(ConfigDir))
        {
            HeartLogger.Warning(LOG_SOURCE, $"Names directory not found at '{ConfigDir}', skipping prefab name loading.");
            return;
        }

        var files = Directory.GetFiles(ConfigDir, "*.json");

        if (files.Length == 0)
        {
            HeartLogger.Warning(LOG_SOURCE, "No JSON files found in Names directory, skipping prefab name loading.");
            return;
        }

        foreach (var file in files)
            LoadPrefabNames(file);

        HeartLogger.Info(LOG_SOURCE, $"Initialized with {_guidToName.Count} entries from {files.Length} file(s).");
    }

    static void LoadPrefabNames(string filePath)
    {
        if (!File.Exists(filePath))
        {
            HeartLogger.Warning(LOG_SOURCE, $"'{Path.GetFileName(filePath)}' not found, skipping.");
            return;
        }

        try
        {
            var json    = File.ReadAllText(filePath);
            var entries = JsonSerializer.Deserialize<Dictionary<string, PrefabNameEntry>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (entries == null) return;

            foreach (var (guidStr, entry) in entries)
            {
                if (!int.TryParse(guidStr, out int guidValue)) continue;

                var guid = new PrefabGUID(guidValue);

                if (!string.IsNullOrEmpty(entry.OriginalName))
                    _originalNameToGuid[entry.OriginalName] = guid;

                if (!string.IsNullOrEmpty(entry.NewName))
                    _newNameToGuid[entry.NewName] = guid;

                // Reverse mapping. NewName wins over OriginalName —
                // admin-defined names take priority in generated output.
                // This means generators produce files that round-trip
                // cleanly back into the config loader.
                if (!string.IsNullOrEmpty(entry.NewName))
                    _guidToName[guidValue] = entry.NewName;
                else if (!string.IsNullOrEmpty(entry.OriginalName))
                    _guidToName[guidValue] = entry.OriginalName;
            }

            HeartLogger.Info(LOG_SOURCE, $"Loaded '{Path.GetFileName(filePath)}'.");
        }
        catch (Exception e)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to load '{Path.GetFileName(filePath)}': {e.Message}");
        }
    }

    // ── Forward lookup (name → GUID) ────────────────────────

    /// <summary>
    /// Resolves a prefab name string to a PrefabGUID.
    /// Checks NewName first (admin config names), then OriginalName (exact game names).
    /// Returns false and PrefabGUID.Empty if not found.
    /// </summary>
    public static bool TryResolve(string name, out PrefabGUID guid)
    {
        if (_newNameToGuid.TryGetValue(name, out guid))
            return true;

        if (_originalNameToGuid.TryGetValue(name, out guid))
            return true;

        guid = Empty;
        HeartLogger.Warning(LOG_SOURCE, $"Could not resolve prefab name: '{name}'");
        return false;
    }

    // ── Reverse lookup (GUID → name) ────────────────────────

    /// <summary>
    /// Resolves a PrefabGUID to a human-readable name.
    /// Prefers NewName (admin-defined) over OriginalName.
    /// Returns false and an empty string if the GUID has no known name.
    ///
    /// Use this in generators that iterate ECS entities and need
    /// readable names for config file output.
    ///
    /// [PERFORMANCE] O(1) dictionary lookup on the GUID int value.
    /// </summary>
    public static bool TryResolveName(PrefabGUID guid, out string name)
    {
        if (_guidToName.TryGetValue(guid._Value, out name!))
            return true;

        name = string.Empty;
        return false;
    }
}