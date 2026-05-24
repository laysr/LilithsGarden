using System.Reflection;
using ProjectM;
using Stunlock.Core;
using Stunlock.Localization;
using Unity.Collections;
using LilithsMind.Prefabs;
using LilithsSoul.Foundation;

// ============================================================
//  LocalizationInjector — LilithsSoul
//  LilithsSoul/Services/LocalizationInjector.cs
//
//  Injects display name and tooltip overrides from a received
//  ServerSyncPayload into V Rising's live localization table:
//      Stunlock.Localization.Localization._LocalizedStrings
//
//  How V Rising localization works:
//  ──────────────────────────────────
//  The game stores all display strings in a static dictionary:
//      Localization._LocalizedStrings: Dictionary<AssetGuid, string>
//
//  Every item, NPC, recipe etc. has an AssetGuid for its name
//  and tooltip. The UI calls Localization.Get(AssetGuid) to
//  resolve display text. We write directly into _LocalizedStrings
//  to override any entry.
//
//  Lookup strategy:
//  ─────────────────
//  Two dictionaries are built once at world ready:
//
//    • _nameToDisplayGuid — prefab Name/Prefab string → display
//                           name AssetGuid, sourced from
//                           PrefabData.AssetGuid via _PrefabDataLookup.
//
//    • _nameToTooltipGuid — prefab Name/Prefab string → tooltip
//                           AssetGuid, sourced from DescKey fields
//                           in LilithsMind definition classes.
//
//  [CHANGED] LilithsMind definition classes are now iterated in a
//            single pass during BuildLookupTable(). Previously
//            BuildDisplayNameLookup() called BuildGuidHashToNameMap()
//            which iterated all definitions once, then BuildTooltipLookup()
//            iterated them again — two full reflection walks for the same
//            data. Now a single ScanDefinitions() pass builds both the
//            guid→name map and the tooltip lookup simultaneously.
//            BuildGuidHashToNameMap() is removed as a result.
//
//  AssetGuid construction:
//  ───────────────────────
//  We parse the raw hex string directly as four big-endian Int32s
//  rather than using System.Guid.ToByteArray() which returns
//  mixed-endian bytes on Windows, byte-swapping the first three
//  components and producing keys that don't match the table.
//
//  [PERFORMANCE] Single reflection pass over LilithsMind at world
//                ready — O(n) over all definition fields, paid once.
//                Display name lookup still O(n) over _PrefabDataLookup,
//                also paid once. All subsequent lookups are O(1).
//                ClearPrevious calls LoadDefaultLanguage() once per
//                server switch — cost paid once, not per-frame.
// ============================================================

namespace LilithsSoul.Services;

public static class LocalizationInjector
{
    private const string LOG_SOURCE = "LilithsSoul.LocalizationInjector";
    private const string PrefabNamespace = "LilithsMind.Prefabs.Definitions";

    // Built once at world ready.
    // Keyed by both Name (admin-facing) and Prefab string (game asset name)
    // so either format sent from the server resolves correctly.
    static readonly Dictionary<string, AssetGuid> _nameToDisplayGuid = new();
    static readonly Dictionary<string, AssetGuid> _nameToTooltipGuid = new();

    // Track all injected keys so we can cleanly remove them on server switch.
    static readonly HashSet<AssetGuid> _injectedGuids = new();

    // ── Public API ───────────────────────────────────────────

    /// <summary>
    /// Builds both lookup tables. Called from SyncReceiver.NotifyWorldReady()
    /// once PrefabCollectionSystem is available.
    ///
    /// [CHANGED] LilithsMind definitions are now scanned once via ScanDefinitions(),
    ///           which builds both the guid→name map and the tooltip lookup in a
    ///           single reflection pass. Previously two separate passes were made.
    ///
    /// [PERFORMANCE] Reflection and ECS iteration run once at world ready.
    ///               No per-frame cost after initialization.
    /// </summary>
    public static void BuildLookupTable()
    {
        _nameToDisplayGuid.Clear();
        _nameToTooltipGuid.Clear();

        try
        {
            var world = Soul.ClientWorld;
            if (world == null)
            {
                SoulLogger.Warning(LOG_SOURCE,
                    "Client world not available — cannot build AssetGuid lookup.");
                return;
            }

            var prefabSystem = world.GetExistingSystemManaged<PrefabCollectionSystem>();
            if (prefabSystem == null)
            {
                SoulLogger.Warning(LOG_SOURCE,
                    "PrefabCollectionSystem not found — cannot build AssetGuid lookup.");
                return;
            }

            // [CHANGED] Single pass over LilithsMind definitions builds both
            //           the guid→name map (fed into BuildDisplayNameLookup) and
            //           the tooltip lookup. Replaces the previous two-pass approach.
            var guidHashToName = ScanDefinitions();

            BuildDisplayNameLookup(prefabSystem, guidHashToName);
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE,
                $"Failed to build AssetGuid lookup: {ex.Message}");
        }
    }

    /// <summary>
    /// Injects display name and tooltip overrides from the payload into
    /// Localization._LocalizedStrings. Safe to call multiple times —
    /// previous overrides are cleared before new ones are applied.
    /// </summary>
    public static void Inject(Network.ServerSyncPayload payload)
    {
        if (_nameToDisplayGuid.Count == 0)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "Display name lookup is empty — was BuildLookupTable() called?");
            return;
        }

        ClearPrevious();

        var table = Localization._LocalizedStrings;
        if (table == null)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "Localization._LocalizedStrings is null — not initialized yet.");
            return;
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Attempting injection — display lookup has {_nameToDisplayGuid.Count} entries, " +
            $"tooltip lookup has {_nameToTooltipGuid.Count} entries, " +
            $"payload has {payload.DisplayNameOverrides.Count} display name(s) " +
            $"and {payload.TooltipOverrides.Count} tooltip(s).");

        int nameCount    = 0;
        int tooltipCount = 0;

        foreach (var (prefabName, displayName) in payload.DisplayNameOverrides)
        {
            if (!_nameToDisplayGuid.TryGetValue(prefabName, out var assetGuid)) continue;

            table[assetGuid] = displayName;
            _injectedGuids.Add(assetGuid);
            nameCount++;
        }

        foreach (var (prefabName, tooltip) in payload.TooltipOverrides)
        {
            if (!_nameToTooltipGuid.TryGetValue(prefabName, out var tooltipGuid)) continue;

            table[tooltipGuid] = tooltip;
            _injectedGuids.Add(tooltipGuid);
            tooltipCount++;
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Injected {nameCount} display name(s) and {tooltipCount} tooltip(s) " +
            $"from server '{payload.ServerIdentity}'.");
    }

    // ── Internal ─────────────────────────────────────────────

    /// <summary>
    /// Single reflection pass over all LilithsMind definition classes.
    /// Builds _nameToTooltipGuid for entries with a DescKey, and returns
    /// a guid hash → Name map for use by BuildDisplayNameLookup().
    ///
    /// [CHANGED] Replaces the two separate calls to BuildGuidHashToNameMap()
    ///           and BuildTooltipLookup(). Previously both methods iterated
    ///           all definition fields independently — now it's one pass.
    ///
    /// [PERFORMANCE] GetTypes() and GetFields() are cached by the runtime
    ///               after the first call. Cost paid once at world ready.
    /// </summary>
    static Dictionary<int, string> ScanDefinitions()
    {
        var guidHashToName = new Dictionary<int, string>();
        var mindAssembly   = typeof(PrefabDef).Assembly;

        var definitionTypes = mindAssembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                t.IsAbstract &&
                t.IsSealed &&
                t.Namespace == PrefabNamespace)
            .ToList();

        if (definitionTypes.Count == 0)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "No definition classes found in LilithsMind — tooltip and name lookups will be empty.");
            return guidHashToName;
        }

        int tooltipCount = 0;

        foreach (var type in definitionTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PrefabDef));

            foreach (var field in fields)
            {
                var def = (PrefabDef)field.GetValue(null)!;

                // Build guid→name map for display name lookup.
                if (def.Name is not null)
                    guidHashToName[def.GuidHash] = def.Name;

                // Build tooltip lookup for entries with a DescKey.
                if (def.DescKey is null) continue;

                var tooltipGuid = GuidFromHexString(def.DescKey);

                _nameToTooltipGuid[def.Prefab] = tooltipGuid;

                if (def.Name is not null)
                    _nameToTooltipGuid[def.Name] = tooltipGuid;

                tooltipCount++;
            }
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Definition scan complete — {guidHashToName.Count} named entry/entries, " +
            $"{tooltipCount} tooltip entry/entries, " +
            $"from {definitionTypes.Count} definition class(es).");

        return guidHashToName;
    }

    /// <summary>
    /// Builds _nameToDisplayGuid from PrefabData.AssetGuid via _PrefabDataLookup.
    /// Receives the guid→name map from ScanDefinitions() so no second reflection
    /// pass is needed.
    ///
    /// [PERFORMANCE] O(n) over _PrefabDataLookup — run once at world ready.
    /// </summary>
    static void BuildDisplayNameLookup(
        PrefabCollectionSystem prefabSystem,
        Dictionary<int, string> guidHashToName)
    {
        var lookup = prefabSystem._PrefabDataLookup;
        var keys   = lookup.GetKeyArray(Allocator.Temp);
        var vals   = lookup.GetValueArray(Allocator.Temp);

        try
        {
            for (int i = 0; i < keys.Length; i++)
            {
                var prefabData = vals[i];
                string prefab  = prefabData.AssetName.Value;

                if (string.IsNullOrEmpty(prefab)) continue;

                var guidStr = prefabData.AssetGuid.ToString();
                if (string.IsNullOrEmpty(guidStr)) continue;

                var assetGuid = GuidFromHexString(guidStr);

                // Key by Prefab string (e.g. "Item_BloodEssence_T01").
                _nameToDisplayGuid[prefab] = assetGuid;

                // Also key by Name if LilithsMind has an entry for this prefab,
                // using the guid→name map built by ScanDefinitions().
                if (guidHashToName.TryGetValue(keys[i]._Value, out var name))
                    _nameToDisplayGuid[name] = assetGuid;
            }
        }
        finally
        {
            keys.Dispose();
            vals.Dispose();
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Display name lookup built with {_nameToDisplayGuid.Count} entries.");
    }

    /// <summary>
    /// Parses a GUID string into an AssetGuid by reading four big-endian Int32s
    /// directly from the hex characters, bypassing System.Guid.ToByteArray().
    /// ToByteArray() on Windows returns mixed-endian bytes — the first three
    /// GUID components are little-endian — producing keys that don't match
    /// Localization._LocalizedStrings entries.
    /// </summary>
    static AssetGuid GuidFromHexString(string guidString)
    {
        var hex = guidString.Replace("-", "");

        int a = Convert.ToInt32(hex[ 0.. 8], 16);
        int b = Convert.ToInt32(hex[ 8..16], 16);
        int c = Convert.ToInt32(hex[16..24], 16);
        int d = Convert.ToInt32(hex[24..32], 16);

        return new AssetGuid(a, b, c, d);
    }

    static void ClearPrevious()
    {
        if (_injectedGuids.Count == 0) return;

        Localization.LoadDefaultLanguage();

        SoulLogger.Debug(LOG_SOURCE,
            $"Cleared {_injectedGuids.Count} previous override(s) " +
            "via localization reload.");

        _injectedGuids.Clear();
    }
}