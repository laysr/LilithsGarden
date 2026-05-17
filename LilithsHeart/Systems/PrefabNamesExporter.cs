using System.Reflection;
using System.Text.Json;
using Stunlock.Core;
using LilithsHeart.Resources.Prefabs;

namespace LilithsHeart.Systems;

/// <summary>
/// Generates and maintains the LilithsHeart/Names/*.json files by reflecting over
/// all static prefab registry classes in the LilithsHeart.Resources.Prefabs namespace.
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
/// Performance:
///   - Reflection runs once at startup; zero cost after initialization.
///   - One file read + one file write per registry class per boot. These are small
///     JSON files so the I/O cost is negligible compared to world load time.
///   - JsonSerializer with WriteIndented for human-readability; acceptable at startup.
/// </summary>
public static class PrefabNamesExporter
{
    private const string LOG_SOURCE = "LilithsHeart.PrefabNamesExporter";

    // The namespace containing all prefab registry classes.
    private const string PrefabNamespace = "LilithsHeart.Resources.Prefabs";

    private static readonly string OutputDir = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsHeart",
        "Names"
    );

    private static readonly JsonSerializerOptions ReadOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly JsonSerializerOptions WriteOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Scans all prefab registry classes in the Prefabs namespace and writes a merged
    /// JSON file for each one. Existing NewName values on disk are preserved.
    /// </summary>
    public static void Export()
    {
        Directory.CreateDirectory(OutputDir);

        // Reflect over the executing assembly to find all static classes in the
        // prefab registry namespace. This includes Items, Weapons, Equipment,
        // Stations, Recipes, and Unsorted — and any future classes — automatically.
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
            LilithsLogger.Warning(LOG_SOURCE, "No prefab registry classes found. Nothing to export.");
            return;
        }

        int filesWritten = 0;
        int newEntries = 0;
        int removedEntries = 0;

        foreach (var type in registryTypes)
        {
            string filePath = Path.Combine(OutputDir, $"{type.Name}.json");

            // Build fresh entries from the current code — these are the ground truth.
            var freshEntries = BuildEntries(type);

            if (freshEntries.Count == 0)
            {
                LilithsLogger.Warning(LOG_SOURCE, $"'{type.Name}' has no PrefabGUID fields — skipping.");
                continue;
            }

            // [CHANGED] Merge strategy replaces skip-if-exists.
            //           Read the existing file (if any) and carry forward any NewName
            //           values the admin has set. OriginalName is always taken from code.
            if (File.Exists(filePath))
            {
                var (merged, added, removed) = MergeWithExisting(filePath, freshEntries, type.Name);
                freshEntries = merged;
                newEntries += added;
                removedEntries += removed;
            }

            WriteFile(filePath, freshEntries, type.Name);
            filesWritten++;
        }

        LilithsLogger.Info(LOG_SOURCE,
            $"Export complete. Files written: {filesWritten}, New entries: {newEntries}, Removed entries: {removedEntries}.");
    }

    /// <summary>
    /// Reads the existing JSON file and merges its NewName values into the fresh entries.
    /// - Fresh entries (from code) are the authority for which GUIDs exist.
    /// - Existing NewName values are carried forward where the GUID still exists in code.
    /// - GUIDs present on disk but absent from code are dropped (removed entries).
    /// - GUIDs present in code but absent from disk are added (new entries).
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
            // If the file is malformed, log and proceed with fresh entries only.
            // The file will be overwritten with clean data.
            LilithsLogger.Warning(LOG_SOURCE, $"Could not read '{typeName}.json' for merge, regenerating: {ex.Message}");
            return (freshEntries, freshEntries.Count, 0);
        }

        if (existingEntries == null)
            return (freshEntries, freshEntries.Count, 0);

        int added = 0;
        int removed = existingEntries.Keys.Count(k => !freshEntries.ContainsKey(k));

        // Walk the fresh entries (from code) and carry forward any existing NewName.
        foreach (var (key, freshEntry) in freshEntries)
        {
            if (existingEntries.TryGetValue(key, out var existingEntry))
            {
                // GUID exists in both — preserve the admin's NewName if they set one.
                freshEntry.NewName = existingEntry.NewName;
            }
            else
            {
                // GUID is new in code — it wasn't in the file before.
                added++;
            }
        }

        if (added > 0 || removed > 0)
            LilithsLogger.Info(LOG_SOURCE,
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

            // [CHANGED] Read NewName from [PrefabName] attribute if present.
            //           Fields without the attribute get an empty NewName, which
            //           is fine — they resolve by OriginalName or raw GUID instead.
            var attr = field.GetCustomAttribute<PrefabNameAttribute>();
            string newName = attr?.NewName ?? string.Empty;

            // Note: if the same GUID hash appears twice in one registry class
            // (duplicate entry), the last field wins. This is a data issue in
            // the registry, not a bug here — worth keeping in mind.
            entries[key] = new PrefabNameEntry
            {
                OriginalName = field.Name,
                NewName = newName
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
            LilithsLogger.Debug(LOG_SOURCE, $"Wrote '{typeName}.json' ({entries.Count} entries).");
        }
        catch (Exception ex)
        {
            LilithsLogger.Error(LOG_SOURCE, $"Failed to write '{typeName}.json': {ex.Message}");
        }
    }
}