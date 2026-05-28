// ============================================================
//  RecipeOverrideData — LilithsMind
//  LilithsMind/Network/RecipeOverrideData.cs
//
//  DTOs describing a single recipe's overridden state as sent
//  inside ServerSyncPayload.RecipeOverrides.
//
//  [CHANGED] Migrated from duplicate definitions in:
//              LilithsHeart/Network/RecipeSlotData.cs
//              LilithsSoul/Network/RecipeSlotData.cs
//            Both files are now deleted. This is the single
//            definition shared between Heart and Soul.
//
//  Only fields the client HUD actually reads are included:
//    • CraftDuration  — displayed in the craft button
//    • Requirements   — ingredient list shown before crafting
//    • Outputs        — output item list shown in the recipe card
//
//  Soul uses these to patch RecipeData, RecipeRequirementBuffer,
//  and RecipeOutputBuffer on client-side prefab entities so the
//  UI reflects what Heart enforces server-side.
//
//  [PERFORMANCE] Plain DTOs — no ECS types, no Unity dependencies.
//                Deserialized once on connect as part of the
//                larger ServerSyncPayload.
// ============================================================

namespace LilithsMind.Network;

/// <summary>
/// A single ingredient or output slot in a recipe override.
/// </summary>
public sealed class RecipeSlotData
{
    /// <summary>
    /// Prefab name of the item.
    /// e.g. "Item_Ingredient_Mineral_CopperIngot"
    /// Resolved to a PrefabGUID by Soul via PrefabCollectionSystem
    /// on the client at patch time.
    /// </summary>
    public string Item { get; set; } = string.Empty;

    /// <summary>Stack count for this slot.</summary>
    public int Amount { get; set; }
}

/// <summary>
/// Full override data for a single recipe.
/// Keyed by recipe prefab name in ServerSyncPayload.RecipeOverrides.
/// </summary>
public sealed class RecipeOverrideData
{
    /// <summary>
    /// Craft duration in seconds.
    /// Patched into RecipeData.CraftDuration on the client prefab entity.
    /// Displayed in the craft button.
    /// </summary>
    public float CraftDuration { get; set; }

    /// <summary>
    /// Ingredient requirements.
    /// Patched into RecipeRequirementBuffer on the client prefab entity.
    /// </summary>
    public List<RecipeSlotData> Requirements { get; set; } = new();

    /// <summary>
    /// Output items.
    /// Patched into RecipeOutputBuffer on the client prefab entity.
    /// </summary>
    public List<RecipeSlotData> Outputs { get; set; } = new();
}