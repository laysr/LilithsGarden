using System.Reflection;
using System.Text.Json;
using Stunlock.Core;
using LilithsHeart.Config;
using LilithsHeart.Foundation;
using LilithsHeart.Prefabs.Definitions;

namespace LilithsHeart.Prefabs;

/// <summary>
/// Generates and maintains the LilithsHeart/Names/*.json files by reflecting over
/// all static prefab definition classes in the LilithsHeart.Prefabs.Definitions namespace.
///
/// [CHANGED] Renamed from PrefabNamesExporter → PrefabNameExporter (singular).
///           Moved from Systems/ → Prefabs/ alongside PrefabNameResolver.
///           Namespace updated: LilithsHeart.Systems → LilithsHeart.Prefabs.
///           Definitions namespace updated: LilithsHeart.Resources.Prefabs → LilithsHeart.Prefabs.Definitions.
///
/// Design — merge strategy:
///   - On every boot, each registry class is reflected and a fresh entry set is built.
///   - If a JSON file already exists, it is read first and any existing NewName values
///     are carried forward into the merged result. Only NewName is preserved from disk;
///     OriginalName is always authoritative from the code.
///   - New registry entries added to the C# classes are automatically added to the file.
///   - Removed registry entries are dropped from the file (no orphaned GUIDs).
///   - This means the file is always in sync with the code while admin NewName aliases
///     are never lost.
///   - Called from Heart.OnInitialize() before PrefabNameResolver.Initialize() so
///     the files are ready to be consumed in the same boot cycle.
///
/// [PERFORMANCE] Reflection runs once at startup; zero cost after initialization.
///               One file read + one file write per registry class per boot.
///               These are small JSON files — I/O cost is negligible vs world load time.
/// </summary>
public static class PrefabNameExporter
{
    private const string LOG_SOURCE = "LilithsHeart.PrefabNameExporter";

    // [CHANGED] Namespace string updated to match renamed Prefabs/Definitions folder.
    private const string PrefabNamespace = "LilithsHeart.Prefabs.Definitions";

    private static readonly string OutputDir = HeartPaths.NamesDir;

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Scans all prefab registry classes in Prefabs/Registry/ and writes a merged
    /// JSON file for each one. Existing NewName values on disk are preserved.
    /// </summary>
    public static void Export()
    {
        Directory.CreateDirectory(OutputDir);

        // Reflect over the executing assembly to find all static classes in the
        // prefab registry namespace. This includes all registry classes automatically —
        // no manual registration needed when new registry classes are added.
        var registryTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                t.IsAbstract &&       // static classes are abstract + sealed in IL
                t.IsSealed &&
                t.Namespace == PrefabNamespace)
            .ToList();

        if (registryTypes.Count == 0)
        {
            HeartLogger.Warning(LOG_SOURCE, "No prefab registry classes found. Nothing to export.");
            return;
        }

        int filesWritten    = 0;
        int newEntries      = 0;
        int removedEntries  = 0;

        foreach (var type in registryTypes)
        {
            string filePath = Path.Combine(OutputDir, $"{type.Name}.json");

            // Build fresh entries from code — these are always the ground truth.
            var freshEntries = BuildEntries(type);

            if (freshEntries.Count == 0)
            {
                HeartLogger.Warning(LOG_SOURCE, $"'{type.Name}' has no PrefabGUID fields — skipping.");
                continue;
            }

            // Merge strategy: read the existing file (if any) and carry forward
            // any NewName values the admin has set. OriginalName is always from code.
            if (File.Exists(filePath))
            {
                var (merged, added, removed) = MergeWithExisting(filePath, freshEntries, type.Name);
                freshEntries    = merged;
                newEntries      += added;
                removedEntries  += removed;
            }

            WriteFile(filePath, freshEntries, type.Name);
            filesWritten++;
        }

        HeartLogger.Info(LOG_SOURCE,
            $"Export complete. Files written: {filesWritten}, New entries: {newEntries}, Removed entries: {removedEntries}.");
    }

    /// <summary>
    /// Reads the existing JSON file and merges its NewName values into the fresh entries.
    /// GUIDs absent from code are dropped; GUIDs new in code are added.
    /// Returns the merged dictionary and counts of added/removed entries for logging.
    /// </summary>
    static (Dictionary<string, PrefabNameEntry> merged, int added, int removed) MergeWithExisting(
        string filePath,
        Dictionary<string, PrefabNameEntry> freshEntries,
        string typeName)
    {
        Dictionary<string, PrefabNameEntry>? existingEntries = null;

        try
        {
            var json = File.ReadAllText(filePath);
            existingEntries = JsonSerializer.Deserialize<Dictionary<string, PrefabNameEntry>>(json, ReadOptions);
        }
        catch (Exception ex)
        {
            // Malformed file — proceed with fresh entries and overwrite cleanly.
            HeartLogger.Warning(LOG_SOURCE, $"Could not read '{typeName}.json' for merge, regenerating: {ex.Message}");
            return (freshEntries, freshEntries.Count, 0);
        }

        if (existingEntries == null)
            return (freshEntries, freshEntries.Count, 0);

        int added   = 0;
        int removed = existingEntries.Keys.Count(k => !freshEntries.ContainsKey(k));

        foreach (var (key, freshEntry) in freshEntries)
        {
            if (existingEntries.TryGetValue(key, out var existingEntry))
            {
                // GUID exists in both — preserve the admin's NewName if they set one.
                freshEntry.NewName = existingEntry.NewName;
            }
            else
            {
                // GUID is new in code — wasn't in the file before.
                added++;
            }
        }

        if (added > 0 || removed > 0)
            HeartLogger.Info(LOG_SOURCE,
                $"'{typeName}.json': +{added} new, -{removed} removed entries.");

        return (freshEntries, added, removed);
    }

    /// <summary>
    /// Reflects over all public static PrefabGUID fields on the given type and
    /// builds a dictionary keyed by GUID hash (as string) for JSON serialization.
    /// OriginalName is the C# field name.
    /// NewName is read from [PrefabName("...")] if present, otherwise left empty.
    /// </summary>
    static Dictionary<string, PrefabNameEntry> BuildEntries(Type type)
    {
        var entries = new Dictionary<string, PrefabNameEntry>();

        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(PrefabGUID));

        foreach (var field in fields)
        {
            var guid = (PrefabGUID)field.GetValue(null)!;
            string key = guid.GuidHash.ToString();

            var attr    = field.GetCustomAttribute<PrefabNameAttribute>();
            string newName = attr?.NewName ?? string.Empty;

            // Note: duplicate GUID hashes in one registry class will have the
            // last field win. This is a data issue in the registry, not a bug here.
            entries[key] = new PrefabNameEntry
            {
                OriginalName = field.Name,
                NewName      = newName
            };
        }

        return entries;
    }

    /// <summary>
    /// Serializes the entry dictionary to indented JSON and writes it to disk.
    /// </summary>
    static void WriteFile(string filePath, Dictionary<string, PrefabNameEntry> entries, string typeName)
    {
        try
        {
            string json = JsonSerializer.Serialize(entries, WriteOptions);
            File.WriteAllText(filePath, json);
            HeartLogger.Debug(LOG_SOURCE, $"Wrote '{typeName}.json' ({entries.Count} entries).");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to write '{typeName}.json': {ex.Message}");
        }
    }
}