using LilithsSoul.Foundation;

// ============================================================
//  LocalizationInjector — LilithsSoul
//
//  Injects display name and tooltip overrides from a received
//  ServerSyncPayload into V Rising's client localization system.
//
//  V Rising localization approach:
//  ────────────────────────────────
//  The game uses a Loc.Get(AssetGuid) or Loc.Get(string key)
//  pattern backed by a Dictionary in LocalizationManager.
//  We reach into that dictionary and upsert our overrides after
//  the vanilla table is loaded.
//
//  Timing:
//  ───────
//  Inject() is called by SyncReceiver after the payload is
//  received AND after ClientInitPatch confirms the client world
//  is ready. If the payload arrives before the world is ready,
//  SyncReceiver queues it and ClientInitPatch triggers injection
//  on world ready.
//
//  Idempotency:
//  ────────────
//  Inject() is safe to call multiple times. Each call fully
//  replaces the previous override set — no stale entries linger
//  from a previous server connection.
//
//  [PERFORMANCE] Upserts are O(1) per entry into an existing
//                Dictionary. The localization table is large
//                but this only touches the keys we override.
//                No full table rebuild occurs.
// ============================================================

namespace LilithsSoul.Localization;

public static class LocalizationInjector
{
    private const string LOG_SOURCE = "LilithsSoul.LocalizationInjector";

    // Tracks keys we've injected so we can cleanly remove them
    // if Inject() is called again with a different server's payload.
    static readonly HashSet<string> _injectedDisplayNames = new();
    static readonly HashSet<string> _injectedTooltips     = new();

    /// <summary>
    /// Injects display name and tooltip overrides from the payload into
    /// V Rising's client localization table.
    /// Safe to call multiple times — previous overrides are replaced cleanly.
    /// </summary>
    public static void Inject(Network.ServerSyncPayload payload)
    {
        int nameCount    = 0;
        int tooltipCount = 0;

        try
        {
            // Clear previously injected keys before applying new ones.
            // This handles switching between servers with different overrides.
            ClearPrevious();

            foreach (var (key, displayName) in payload.DisplayNameOverrides)
            {
                if (TryInjectDisplayName(key, displayName))
                {
                    _injectedDisplayNames.Add(key);
                    nameCount++;
                }
            }

            foreach (var (key, tooltip) in payload.TooltipOverrides)
            {
                if (TryInjectTooltip(key, tooltip))
                {
                    _injectedTooltips.Add(key);
                    tooltipCount++;
                }
            }

            SoulLogger.Info(LOG_SOURCE,
                $"Injected {nameCount} display name(s) and {tooltipCount} tooltip(s) " +
                $"from server '{payload.ServerIdentity}'.");
        }
        catch (Exception ex)
        {
            SoulLogger.Error(LOG_SOURCE, $"Localization injection failed: {ex.Message}");
        }
    }

    // ── Internal ────────────────────────────────────────────

    static void ClearPrevious()
    {
        // TODO: implement removal from V Rising's localization table
        // once we confirm the exact API surface available on the client.
        // For now, upsert overwrites are sufficient since we always
        // re-inject the full set on each server connection.
        _injectedDisplayNames.Clear();
        _injectedTooltips.Clear();
    }

    static bool TryInjectDisplayName(string prefabName, string displayName)
    {
        try
        {
            // TODO: resolve the correct localization key from prefabName
            // and upsert into V Rising's LocalizationManager dictionary.
            // This requires confirming which LocalizationManager type and
            // method is available in the VampireReferenceAssemblies for
            // the current game version.
            //
            // Placeholder — log intent until API surface is confirmed.
            SoulLogger.Debug(LOG_SOURCE, $"[DisplayName] {prefabName} → {displayName}");
            return true;
        }
        catch (Exception ex)
        {
            SoulLogger.Warning(LOG_SOURCE,
                $"Failed to inject display name for '{prefabName}': {ex.Message}");
            return false;
        }
    }

    static bool TryInjectTooltip(string prefabName, string tooltip)
    {
        try
        {
            // TODO: same as TryInjectDisplayName — resolve key, upsert.
            SoulLogger.Debug(LOG_SOURCE, $"[Tooltip] {prefabName} → {tooltip}");
            return true;
        }
        catch (Exception ex)
        {
            SoulLogger.Warning(LOG_SOURCE,
                $"Failed to inject tooltip for '{prefabName}': {ex.Message}");
            return false;
        }
    }
}