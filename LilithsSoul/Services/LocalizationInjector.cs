using System.Reflection;
using Stunlock.Core;
using Stunlock.Localization;
using LilithsMind.Prefabs;
using LilithsSoul.Foundation;
using LilithsMind.Network;

// ============================================================
//  LocalizationInjector — LilithsSoul
//  LilithsSoul/Services/LocalizationInjector.cs
//
//  Injects display name and tooltip overrides from a received
//  ServerSyncPayload into V Rising's live localization table:
//      Stunlock.Localization.Localization._LocalizedStrings
//
//  How V Rising localization works (confirmed via DnSpy):
//  ───────────────────────────────────────────────────────
//  _LocalizedStrings is Dictionary<AssetGuid, string>.
//  The UI calls Localization.Get(AssetGuid) to resolve text.
//  Each prefab has two localization keys stored in PrefabDef:
//    NameKey — AssetGuid for the display name string
//    DescKey — AssetGuid for the tooltip/description string
//
//  Example (Blood Essence):
//    NameKey = "37e872e1-4aa1-4f0a-8e2e-a67883b5a645" → "Blood Essence"
//    DescKey = "662215a0-2197-4b43-b91e-c0e546f6a369" → "An energy source..."
//
//  Injection:
//    _LocalizedStrings[ParseAssetGuid(NameKey)] = "Vitae"
//    _LocalizedStrings[ParseAssetGuid(DescKey)] = "Custom tooltip"
//
//  [CHANGED] Complete rewrite. Previous implementation used
//  PrefabData.AssetGuid from _PrefabDataLookup as the injection
//  key — this was wrong. AssetGuid is the prefab's identity GUID,
//  not its localization key. The UI reads from _LocalizedStrings
//  using NameKey/DescKey AssetGuids, not the prefab identity GUID.
//
//  The correct source for localization keys is LilithsMind PrefabDef
//  entries — NameKey and DescKey fields. No ECS iteration needed.
//
//  Lookup strategy:
//  ─────────────────
//  BuildLookupTable() scans LilithsMind definition classes once
//  at world ready and builds two dictionaries:
//    _nameToNameGuid — prefab Name/Prefab string → NameKey AssetGuid
//    _nameToDescGuid — prefab Name/Prefab string → DescKey AssetGuid
//
//  Entries with null NameKey or DescKey are skipped — partial
//  definitions cannot inject what they don't have.
//
//  [PERFORMANCE] Reflection over LilithsMind assembly runs once
//                at world ready — O(n) over all definition fields.
//                No ECS iteration. No PrefabCollectionSystem needed.
//                Injection is O(1) per entry — direct dict write.
//                ClearPrevious calls LoadDefaultLanguage() once per
//                server switch — reads from disk, paid once only.
// ============================================================

namespace LilithsSoul.Services;

public static class LocalizationInjector
{
    private const string LOG_SOURCE = "LilithsSoul.LocalizationInjector";

    private const string PrefabNamespace = "LilithsMind.Prefabs.Definitions";

    // Built once at world ready from LilithsMind definitions.
    // Keyed by both Name (admin-facing) and Prefab string (game asset name).
    static readonly Dictionary<string, AssetGuid> _nameToNameGuid = new();
    static readonly Dictionary<string, AssetGuid> _nameToDescGuid = new();

    // Track injected AssetGuids so we can cleanly remove them on server switch.
    static readonly HashSet<AssetGuid> _injectedGuids = new();

    // ── Public API ───────────────────────────────────────────

    /// <summary>
    /// Builds both lookup tables from LilithsMind definition classes.
    /// Called by SyncReceiver.NotifyWorldReady() — no ECS access needed,
    /// so safe to call as soon as BepInEx has loaded LilithsMind.
    ///
    /// Safe to call multiple times — clears and rebuilds each call.
    ///
    /// [PERFORMANCE] O(n) reflection over LilithsMind definitions.
    ///               Run once at world ready. No per-frame cost.
    /// </summary>
    public static void BuildLookupTable()
    {
        _nameToNameGuid.Clear();
        _nameToDescGuid.Clear();

        var mindAssembly = typeof(PrefabDef).Assembly;

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
                "No definition classes found in LilithsMind — localization lookup will be empty.");
            return;
        }

        int nameCount = 0;
        int descCount = 0;

        foreach (var type in definitionTypes)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(PrefabDef));

            foreach (var field in fields)
            {
                var def = (PrefabDef)field.GetValue(null)!;

                // ── Display name ──────────────────────────────
                if (def.NameKey is not null)
                {
                    var nameGuid = ParseAssetGuid(def.NameKey);

                    // Key by Prefab string (e.g. "Item_BloodEssence_T01").
                    _nameToNameGuid[def.Prefab] = nameGuid;

                    // Also key by Name if present (e.g. "BloodEssence").
                    if (def.Name is not null)
                        _nameToNameGuid[def.Name] = nameGuid;

                    nameCount++;
                }

                // ── Tooltip / description ─────────────────────
                if (def.DescKey is not null)
                {
                    var descGuid = ParseAssetGuid(def.DescKey);

                    _nameToDescGuid[def.Prefab] = descGuid;

                    if (def.Name is not null)
                        _nameToDescGuid[def.Name] = descGuid;

                    descCount++;
                }
            }
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Lookup tables built — {nameCount} name key(s), {descCount} desc key(s) " +
            $"from {definitionTypes.Count} definition class(es).");
    }

    /// <summary>
    /// Injects display name and tooltip overrides from the payload into
    /// Localization._LocalizedStrings. Safe to call multiple times —
    /// previous overrides are cleared via LoadDefaultLanguage() before
    /// new ones are applied.
    ///
    /// Silently skips entries whose key is not in the LilithsMind lookup —
    /// this means the admin used a prefab name that has no NameKey/DescKey
    /// in LilithsMind yet. A warning is logged per skipped entry.
    /// </summary>
    public static void Inject(ServerSyncPayload payload)
    {
        ClearPrevious();

        var table = Localization._LocalizedStrings;
        if (table == null)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "Localization._LocalizedStrings is null — not initialized yet.");
            return;
        }

        int nameCount    = 0;
        int tooltipCount = 0;
        int skipped      = 0;

        foreach (var (prefabName, displayName) in payload.DisplayNameOverrides)
        {
            if (!_nameToNameGuid.TryGetValue(prefabName, out var assetGuid))
            {
                SoulLogger.Warning(LOG_SOURCE,
                    $"No NameKey found for '{prefabName}' — " +
                    "add NameKey to its PrefabDef in LilithsMind to enable injection.");
                skipped++;
                continue;
            }

            table[assetGuid] = displayName;
            _injectedGuids.Add(assetGuid);
            nameCount++;
        }

        foreach (var (prefabName, tooltip) in payload.TooltipOverrides)
        {
            if (!_nameToDescGuid.TryGetValue(prefabName, out var assetGuid))
            {
                SoulLogger.Warning(LOG_SOURCE,
                    $"No DescKey found for '{prefabName}' — " +
                    "add DescKey to its PrefabDef in LilithsMind to enable injection.");
                skipped++;
                continue;
            }

            table[assetGuid] = tooltip;
            _injectedGuids.Add(assetGuid);
            tooltipCount++;
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Injected {nameCount} display name(s) and {tooltipCount} tooltip(s) " +
            $"from server '{payload.ServerIdentity}'. " +
            $"{skipped} skipped (missing LilithsMind NameKey/DescKey).");
    }

    // ── Internal ─────────────────────────────────────────────

    /// <summary>
    /// Removes previously injected overrides by reloading the vanilla
    /// localization table. Called before applying a new server's payload.
    ///
    /// [PERFORMANCE] LoadDefaultLanguage() reads from disk — O(file size).
    ///               Called once per server switch, never per-frame.
    /// </summary>
    static void ClearPrevious()
    {
        if (_injectedGuids.Count == 0) return;

        Localization.LoadDefaultLanguage();
        _injectedGuids.Clear();

        SoulLogger.Debug(LOG_SOURCE, "Previous overrides cleared via localization reload.");
    }

    /// <summary>
    /// Parses a GUID string into an AssetGuid using the game's own
    /// FromString() method — guaranteed to match the internal format.
    ///
    /// [CHANGED] Replaced manual hex parsing with AssetGuid.FromString().
    ///           Our previous manual implementation produced wrong keys
    ///           (HasKey returned false). FromString() is the game's own
    ///           parser and produces exactly the keys in _LocalizedStrings.
    /// </summary>
    static AssetGuid ParseAssetGuid(string guidString)
        => AssetGuid.FromString(guidString);
}