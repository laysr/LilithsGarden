// ============================================================
//  ServerSyncPayload — LilithsMind
//  LilithsMind/Network/ServerSyncPayload.cs
//
//  The data contract sent from LilithsHeart to LilithsSoul on
//  client connect. Shared between both projects via LilithsMind
//  as the single source of truth.
//
//  [CHANGED] Migrated from duplicate definitions in:
//              LilithsHeart/Network/ServerSyncPayload.cs
//              LilithsSoul/Network/ServerSyncPayload.cs
//            Both files are now deleted. This is the single
//            definition. Heart and Soul reference it via their
//            existing ProjectReference to LilithsMind.
//
//  Design:
//  ───────
//  Heart serializes this to JSON and sends it in chunks via
//  ChatMessageServerEvent. Soul accumulates chunks, deserializes,
//  and writes to disk as sync.json under:
//      BepInEx/config/LilithsSoul/<ServerIdentity>/sync.json
//
//  The Build() factory method lives in Heart's SyncPayloadCache
//  and not here — LilithsMind has no dependency on Heart config
//  or any V Rising assemblies. This class is a plain DTO.
//
//  [PERFORMANCE] Plain DTO — no ECS types, no Unity dependencies.
//                Serialized once on connect by Heart, deserialized
//                once on receipt by Soul.
// ============================================================

namespace LilithsMind.Network;

/// <summary>
/// The full data bundle Heart sends to a connecting Soul client.
/// Shared contract — do not add Heart-only or Soul-only logic here.
/// </summary>
public sealed class ServerSyncPayload
{
    // ── Identity ────────────────────────────────────────────

    /// <summary>
    /// Server name, sanitized for use as a folder name.
    /// Soul uses this to scope its disk cache per server.
    /// </summary>
    public string ServerIdentity { get; set; } = string.Empty;

    /// <summary>
    /// Short SHA256 hash of the serialized payload.
    /// Soul compares this against its cached sync.json hash
    /// to skip redundant disk writes and re-injection on reconnect.
    /// </summary>
    public string PayloadHash { get; set; } = string.Empty;

    // ── Localization ────────────────────────────────────────

    /// <summary>
    /// Display name overrides keyed by prefab name.
    /// e.g. "Item_BloodEssence_T01" → "Vitae"
    /// Soul injects these into Localization._LocalizedStrings
    /// via LocalizationInjector using NameKey from LilithsMind PrefabDefs.
    /// </summary>
    public Dictionary<string, string> DisplayNameOverrides { get; set; } = new();

    /// <summary>
    /// Tooltip overrides keyed by prefab name.
    /// e.g. "Item_BloodEssence_T01" → "Concentrated life force..."
    /// Soul injects these into Localization._LocalizedStrings
    /// via LocalizationInjector using DescKey from LilithsMind PrefabDefs.
    /// </summary>
    public Dictionary<string, string> TooltipOverrides { get; set; } = new();

    // ── Recipe overrides ────────────────────────────────────

    /// <summary>
    /// Recipe data overrides keyed by recipe prefab name.
    /// Soul patches RecipeData, RecipeRequirementBuffer, RecipeOutputBuffer
    /// and RecipeHashLookupMap on client prefab entities from this dict.
    /// </summary>
    public Dictionary<string, RecipeOverrideData> RecipeOverrides { get; set; } = new();

    /// <summary>
    /// Station recipe overrides keyed by station prefab name.
    /// Soul patches WorkstationRecipesBuffer on matching client-side
    /// station entities so the crafting UI reflects server-side changes.
    /// </summary>
    public Dictionary<string, StationRecipeOverrideData> StationRecipeOverrides { get; set; } = new();

    // ── Player crafting overrides ────────────────────────────

    /// <summary>
    /// Recipe prefab names to add to the client player's
    /// WorkstationRecipesBuffer. Soul patches the local player entity's
    /// buffer so the player crafting menu reflects server-side additions.
    /// </summary>
    public List<string> PlayerRecipesToAdd { get; set; } = new();

    /// <summary>
    /// Recipe prefab names to remove from the client player's
    /// WorkstationRecipesBuffer. Soul patches the local player entity's
    /// buffer so the player crafting menu reflects server-side removals.
    /// </summary>
    public List<string> PlayerRecipesToRemove { get; set; } = new();
}