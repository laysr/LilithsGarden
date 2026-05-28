using System.Reflection;
using Stunlock.Core;
using LilithsHeart.Foundation;
using LilithsMind.Prefabs;

// ============================================================
//  PrefabNameResolver — LilithsHeart
//  LilithsHeart/Prefabs/PrefabNameResolver.cs
//
//  Resolves prefab names to PrefabGUIDs and vice versa by
//  scanning the PrefabDef definition classes in LilithsMind
//  directly at startup. No JSON files are read or written.
//
//  [CHANGED] Complete rewrite. Previously read Names/*.json
//            files written by PrefabNameExporter and deserialized
//            into PrefabNameEntry records. That pipeline is gone —
//            LilithsMind definition classes are now the single
//            source of truth. PrefabNameExporter and PrefabNameEntry
//            are both deleted as a result.
//
//  [CHANGED] Now scans typeof(PrefabDef).Assembly (LilithsMind)
//            via reflection to find all static PrefabDef fields
//            across all definition classes. Same reflection pattern
//            as the old PrefabNameExporter used, repurposed for
//            runtime lookup building instead of JSON export.
//
//  Lookup strategy:
//  ─────────────────
//  Three dictionaries are built once at Initialize():
//    • _nameToGuid      — Name → PrefabGUID  (e.g. "BanditBag" → guid)
//    • _prefabToGuid    — Prefab → PrefabGUID (e.g. "Item_NewBag_T01" → guid)
//    • _guidToName      — int → Name          (reverse lookup for generators)
//
//  Name takes priority over Prefab in all forward lookups —
//  admin-facing names are preferred over internal asset names.
//  Falls back to Prefab string if Name is null.
//
//  [PERFORMANCE] Reflection runs once at world ready.
//                All three dictionaries are built in a single pass
//                over the definition fields — O(n) where n is the
//                total number of PrefabDef entries across all classes.
//                All subsequent lookups are O(1) dictionary reads.
//                No file I/O occurs at any point.
// ============================================================

namespace LilithsHeart.Foundation;

public static class PrefabNameResolver
{
    private const string LOG_SOURCE = "LilithsHeart.PrefabNameResolver";

    private const string PrefabNamespace = "LilithsMind.Prefabs.Definitions";

    public static readonly PrefabGUID Empty = new(0);

    // Forward lookups — used when loading config files by name.
    static readonly Dictionary<string, PrefabGUID> _nameToGuid   = new();
    static readonly Dictionary<string, PrefabGUID> _prefabToGuid = new();

    // Reverse lookup — used by anything that has a GUID and needs
    // a human-readable name (logging, command output, config generation).
    // Prefers Name over Prefab string when both exist.
    static readonly Dictionary<int, string> _guidToName = new();

    public static void Initialize()
    {
        // Scan LilithsMind's assembly for all definition classes.
        // [PERFORMANCE] typeof(PrefabDef).Assembly resolves once.
        //               GetTypes() is cached by the runtime after
        //               the first call on a given assembly.
        var mindAssembly = typeof(PrefabDef).Assembly;

        var definitionTypes = mindAssembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                t.IsAbstract &&     // static classes are abstract + sealed in IL
                t.IsSealed &&
                t.Namespace == PrefabNamespace)
            .ToList();

        if (definitionTypes.Count == 0)
        {
            HeartLogger.Warning(LOG_SOURCE,
                "No definition classes found in LilithsMind — resolver will be empty.");
            return;
        }

        int total = 0;

        foreach (var type in definitionTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PrefabDef));

            foreach (var field in fields)
            {
                var def  = (PrefabDef)field.GetValue(null)!;
                var guid = new PrefabGUID(def.GuidHash);

                // Forward: Name → GUID (preferred admin-facing name).
                if (def.Name is not null)
                    _nameToGuid[def.Name] = guid;

                // Forward: Prefab string → GUID (exact game asset name fallback).
                if (!string.IsNullOrEmpty(def.Prefab))
                    _prefabToGuid[def.Prefab] = guid;

                // Reverse: GUID int → display name.
                // Prefers Name over Prefab — admin names take priority
                // in generated output so files round-trip cleanly.
                _guidToName[def.GuidHash] = def.Name ?? def.Prefab;

                total++;
            }
        }

        HeartLogger.Info(LOG_SOURCE,
            $"Initialized with {total} entries from {definitionTypes.Count} definition class(es).");
    }

    // ── Forward lookup (name → GUID) ────────────────────────

    /// <summary>
    /// Resolves a prefab name string to a PrefabGUID.
    /// Checks Name first (admin-facing), then Prefab string (game asset name).
    /// Returns false and PrefabGUID(0) if not found.
    /// </summary>
    public static bool TryResolve(string name, out PrefabGUID guid)
    {
        if (_nameToGuid.TryGetValue(name, out guid))
            return true;

        if (_prefabToGuid.TryGetValue(name, out guid))
            return true;

        guid = Empty;
        HeartLogger.Warning(LOG_SOURCE, $"Could not resolve prefab name: '{name}'");
        return false;
    }

    // ── Reverse lookup (GUID → name) ────────────────────────

    /// <summary>
    /// Resolves a PrefabGUID to a human-readable name.
    /// Prefers Name (admin-defined) over Prefab string.
    /// Returns false and empty string if the GUID has no known entry.
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