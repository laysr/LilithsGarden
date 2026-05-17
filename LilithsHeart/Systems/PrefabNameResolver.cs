using System.Text.Json;
using Stunlock.Core;

namespace LilithsHeart.Systems;

public static class PrefabNameResolver
{
    private const string LOG_SOURCE = "LilithsHeart.PrefabNameResolver";

    public static readonly PrefabGUID Empty = new PrefabGUID(0);

    static readonly Dictionary<string, PrefabGUID> _newNameToGuid = new();
    static readonly Dictionary<string, PrefabGUID> _originalNameToGuid = new();

    // [CHANGED] Updated config directory to LilithsGarden/Names/
    //           for a cleaner shared config folder structure across the suite.
    static readonly string ConfigDir = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsGarden",
        "Names"
    );

    public static void Initialize()
    {
        // [CHANGED] Replaced hardcoded filenames with automatic directory scan.
        //           Any JSON file in the Names directory will be loaded automatically.
        //           To add a new category, simply drop a new JSON file in the folder
        //           with no code changes needed.
        if (!Directory.Exists(ConfigDir))
        {
            LilithsLogger.Warning(LOG_SOURCE, $"Names directory not found at '{ConfigDir}', skipping prefab name loading.");
            return;
        }

        var files = Directory.GetFiles(ConfigDir, "*.json");

        if (files.Length == 0)
        {
            LilithsLogger.Warning(LOG_SOURCE, "No JSON files found in Names directory, skipping prefab name loading.");
            return;
        }

        foreach (var file in files)
            LoadPrefabNames(file);

        LilithsLogger.Info(LOG_SOURCE, $"Initialized with {_newNameToGuid.Count} entries from {files.Length} file(s).");
    }

    static void LoadPrefabNames(string filePath)
    {
        // [CHANGED] Receives full file path instead of just filename
        //           since files are now discovered by directory scan.
        if (!File.Exists(filePath))
        {
            LilithsLogger.Warning(LOG_SOURCE, $"'{Path.GetFileName(filePath)}' not found, skipping.");
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var entries = JsonSerializer.Deserialize<Dictionary<string, PrefabNameEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (entries == null) return;

            foreach (var (guidStr, entry) in entries)
            {
                if (!int.TryParse(guidStr, out int guidValue)) continue;

                var guid = new PrefabGUID(guidValue);

                // [CHANGED] Renamed Name -> OriginalName, ConfigName -> NewName
                if (!string.IsNullOrEmpty(entry.OriginalName))
                    _originalNameToGuid[entry.OriginalName] = guid;

                if (!string.IsNullOrEmpty(entry.NewName))
                    _newNameToGuid[entry.NewName] = guid;
            }

            LilithsLogger.Info(LOG_SOURCE, $"Loaded '{Path.GetFileName(filePath)}'.");
        }
        catch (Exception e)
        {
            LilithsLogger.Error(LOG_SOURCE, $"Failed to load '{Path.GetFileName(filePath)}': {e.Message}");
        }
    }

    /// <summary>
    /// Attempts to resolve a prefab name to a PrefabGUID.
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
        LilithsLogger.Warning(LOG_SOURCE, $"Could not resolve prefab name: '{name}'");
        return false;
    }
}

public class PrefabNameEntry
{
    // [CHANGED] Renamed Name -> OriginalName, ConfigName -> NewName
    public string OriginalName { get; set; } = string.Empty;
    public string NewName { get; set; } = string.Empty;
}