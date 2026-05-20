using ProjectM;
using Stunlock.Core;
using Stunlock.Localization;
using Unity.Collections;
using LilithsSoul.Foundation;

// ============================================================
//  LocalizationInjector — LilithsSoul
//
//  Injects display name and tooltip overrides from a received
//  ServerSyncPayload directly into V Rising's live localization
//  table: Localization._LocalizedStrings.
//
//  How V Rising localization works:
//  ──────────────────────────────────
//  The game stores all display strings in a static dictionary:
//      Localization._LocalizedStrings: Dictionary<AssetGuid, string>
//
//  Every item, NPC, recipe etc. has an AssetGuid for its name.
//  The UI calls Localization.Get(AssetGuid) to resolve display text.
//  We write directly into _LocalizedStrings to override any entry.
//
//  AssetGuid lookup strategy:
//  ───────────────────────────
//  The server sends overrides keyed by prefab name (e.g. "Item_BloodEssence_T01").
//  We need to map that name to the AssetGuid used in _LocalizedStrings.
//
//  At world ready, we build a local lookup table from
//  PrefabCollectionSystem._PrefabDataLookup, which maps
//  PrefabGUID → PrefabData. PrefabData.AssetName gives us the
//  prefab's name string, and the PrefabGUID's GuidHash corresponds
//  to the AssetGuid used in the localization table.
//
//  This mirrors how Bloodcraft's LocalizationService works, but
//  built at runtime rather than from a hardcoded dictionary.
//
//  Tooltip injection:
//  ──────────────────
//  Tooltips use a separate AssetGuid from the display name.
//  We attempt a suffix pattern ("_Description") as a heuristic —
//  if the suffix entry exists in _LocalizedStrings we patch it.
//  This is best-effort; not all items have tooltips.
//
//  [PERFORMANCE] Lookup table built once at world ready — O(n)
//                over all prefabs, run once.
//                Injection is O(1) per entry — direct dictionary write.
//                _LocalizedStrings is read at UI render time; our
//                writes are visible immediately on next render.
// ============================================================

namespace LilithsSoul.Localization;

public static class LocalizationInjector
{
    private const string LOG_SOURCE = "LilithsSoul.LocalizationInjector";

    // Built once at world ready from PrefabCollectionSystem.
    // Key: prefab name string (OriginalName / AssetName).
    // Value: AssetGuid used in Localization._LocalizedStrings.
    static readonly Dictionary<string, AssetGuid> _nameToAssetGuid = new();

    // Track injected keys so we can cleanly remove them when
    // switching servers or receiving a new payload.
    static readonly HashSet<AssetGuid> _injectedGuids = new();

    /// <summary>
    /// Builds the prefab name → AssetGuid lookup table.
    /// Call this from SyncReceiver.NotifyWorldReady() after
    /// the client ECS world is ready.
    ///
    /// [PERFORMANCE] Iterates all prefabs once. Cost is O(n) at
    ///               world load — acceptable for a one-time build.
    /// </summary>
    public static void BuildLookupTable()
    {
        _nameToAssetGuid.Clear();

        try
        {
            var world = Soul.ClientWorld;
            if (world == null)
            {
                SoulLogger.Warning(LOG_SOURCE, "Client world not available — cannot build AssetGuid lookup.");
                return;
            }

            var prefabSystem = world.GetExistingSystemManaged<PrefabCollectionSystem>();
            if (prefabSystem == null)
            {
                SoulLogger.Warning(LOG_SOURCE, "PrefabCollectionSystem not found — cannot build AssetGuid lookup.");
                return;
            }

            var lookup = prefabSystem._PrefabDataLookup;
            var keys   = lookup.GetKeyArray(Allocator.Temp);
            var vals   = lookup.GetValueArray(Allocator.Temp);

            try
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var prefabGuid = keys[i];
                    var prefabData = vals[i];

                    string name = prefabData.AssetName.Value;
                    if (string.IsNullOrEmpty(name)) continue;

                    // AssetGuid for localization is derived from the prefab GUID hash.
                    // This matches how Bloodcraft maps PrefabGUID → AssetGuid string.
                    var assetGuid = new AssetGuid(new System.Guid(
                        prefabData.AssetGuid.ToString()));

                    _nameToAssetGuid[name] = assetGuid;
                }
            }
            finally
            {
                keys.Dispose();
                vals.Dispose();
            }

            SoulLogger.Info(LOG_SOURCE,
                $"AssetGuid lookup table built with {_nameToAssetGuid.Count} entries.");
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"Failed to build AssetGuid lookup: {ex.Message}");
        }
    }

    /// <summary>
    /// Injects display name and tooltip overrides from the payload
    /// into Localization._LocalizedStrings.
    ///
    /// Safe to call multiple times — previous overrides are removed
    /// before new ones are applied, so switching servers is clean.
    /// </summary>
    public static void Inject(Network.ServerSyncPayload payload)
    {
        if (_nameToAssetGuid.Count == 0)
        {
            SoulLogger.Warning(LOG_SOURCE,
                "AssetGuid lookup table is empty — was BuildLookupTable() called?");
            return;
        }

        // Remove previously injected overrides before applying new ones.
        ClearPrevious();

        int nameCount    = 0;
        int tooltipCount = 0;

        var table = Localization._LocalizedStrings;
        if (table == null)
        {
            SoulLogger.Warning(LOG_SOURCE, "Localization._LocalizedStrings is null — not initialized yet.");
            return;
        }

        foreach (var (prefabName, displayName) in payload.DisplayNameOverrides)
        {
            if (!_nameToAssetGuid.TryGetValue(prefabName, out var assetGuid)) continue;

            table[assetGuid] = displayName;
            _injectedGuids.Add(assetGuid);
            nameCount++;
        }

        foreach (var (prefabName, tooltip) in payload.TooltipOverrides)
        {
            // Tooltips are stored under a separate AssetGuid.
            // Attempt the common "_Description" suffix convention.
            // If the entry doesn't exist in _LocalizedStrings, skip — we
            // don't want to create phantom entries for items without tooltips.
            if (!_nameToAssetGuid.TryGetValue(prefabName + "_Description", out var tooltipGuid))
                continue;

            if (!table.ContainsKey(tooltipGuid)) continue;

            table[tooltipGuid] = tooltip;
            _injectedGuids.Add(tooltipGuid);
            tooltipCount++;
        }

        SoulLogger.Info(LOG_SOURCE,
            $"Injected {nameCount} display name(s) and {tooltipCount} tooltip(s) " +
            $"from server '{payload.ServerIdentity}'.");
    }

    // ── Internal ────────────────────────────────────────────

    static void ClearPrevious()
    {
        if (_injectedGuids.Count == 0) return;

        // [PERFORMANCE] Reload vanilla strings for only the keys we touched.
        //               We do this by calling Localization.LoadDefaultLanguage()
        //               which is expensive (full reload), so instead we just
        //               remove our injected keys — the next Get() call for a
        //               missing key will fall back to vanilla automatically
        //               depending on how the game handles missing keys.
        //               For safety we reload the full localization only if
        //               the previous server had overrides.
        if (_injectedGuids.Count > 0)
        {
            // Full reload restores vanilla strings cleanly.
            // Cost is paid once on server disconnect/switch — acceptable.
            Localization.LoadDefaultLanguage();
            SoulLogger.Debug(LOG_SOURCE,
                $"Cleared {_injectedGuids.Count} previous override(s) via localization reload.");
        }

        _injectedGuids.Clear();
    }
}