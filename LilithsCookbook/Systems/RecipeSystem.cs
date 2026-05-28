using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Foundation;
using LilithsMind.Network;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

// ============================================================
//  RecipeSystem — LilithsCookbook
//
//  Applies recipe changes from recipes.json to server-side ECS
//  prefab entities and RecipeHashLookupMap, then registers
//  overrides with Heart for Soul client sync.
//
//  Why RecipeHashLookupMap must be written directly:
//  ──────────────────────────────────────────────────
//  The map is populated from baked scene data at startup and is
//  NOT updated by RegisterRecipes() from live entity components.
//  The crafting system reads CraftDuration and other scalar fields
//  from the map — not the entity — so both must be kept in sync.
//
//  [PERFORMANCE] ApplyChanges() runs once at startup. All ECS
//                writes and map updates are one-time costs.
// ============================================================
public static class RecipeSystem
{
    private const string LOG_SOURCE = "LilithsCookbook.RecipeSystem";

    public static void ApplyChanges()
    {
        var config = CookbookPlugin.RecipeData;

        if (config == null || config.Recipes.Count == 0)
        {
            HeartLogger.Info(LOG_SOURCE, "No recipe changes configured.");
            return;
        }

        int changed = 0;

        // [PERFORMANCE] Dict pre-sized to config count — avoids
        // rehashing during the apply loop.
        var soulOverrides = new Dictionary<string, RecipeOverrideData>(config.Recipes.Count);

        foreach (var (recipeName, entry) in config.Recipes)
        {
            if (!entry.ChangesEnabled) continue;

            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID guid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"Could not resolve recipe: '{recipeName}'");
                continue;
            }

            if (!Heart.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(guid, out Entity recipeEntity))
            {
                HeartLogger.Warning(LOG_SOURCE, $"Could not find prefab entity for recipe: '{recipeName}'");
                continue;
            }

            if (entry.CraftDuration.HasValue       ||
                entry.AlwaysUnlocked.HasValue       ||
                entry.HideInStation.HasValue        ||
                entry.IgnoreServerSettings.HasValue ||
                entry.HudSortingOrder.HasValue)
            {
                ApplyRecipeData(recipeEntity, entry, guid);
            }

            if (entry.Requirements != null)
                ApplyRequirements(recipeEntity, entry.Requirements, recipeName);

            if (entry.Outputs != null)
                ApplyOutputs(recipeEntity, entry.Outputs, recipeName);

            if (entry.UseRepairCosts.HasValue)
                ApplyOptionalBuffer(recipeEntity, entry.UseRepairCosts.Value, entry.RepairCosts, recipeName,
                    ApplyRepairCosts);

            if (entry.UseUnitOutputs.HasValue)
                ApplyOptionalBuffer(recipeEntity, entry.UseUnitOutputs.Value, entry.UnitOutputs, recipeName,
                    ApplyUnitOutputs);

            if (entry.UseRecipeLinks.HasValue)
                ApplyOptionalBuffer(recipeEntity, entry.UseRecipeLinks.Value, entry.RecipeLinks, recipeName,
                    ApplyRecipeLinks);

            changed++;

            soulOverrides[recipeName] = BuildSoulOverride(recipeEntity);
        }

        if (changed > 0)
        {
            Heart.GameDataSystem.RegisterRecipes();
            HeartLogger.Info(LOG_SOURCE, $"LilithsCookbook applied changes to {changed} recipe(s).");

            Heart.RegisterRecipeOverrides(soulOverrides);
            HeartLogger.Info(LOG_SOURCE,
                $"Registered {soulOverrides.Count} recipe override(s) with Heart for Soul sync.");
        }
        else
        {
            HeartLogger.Info(LOG_SOURCE, "No recipes had ChangesEnabled = true, skipping registration.");
        }
    }

    // ── Soul override builder ─────────────────────────────────

    /// <summary>
    /// Builds a RecipeOverrideData by reading the current ECS entity state
    /// after all changes have been applied. Reading from ECS rather than the
    /// config entry ensures the override reflects what was actually committed
    /// (e.g. a requirement item that failed to resolve won't appear).
    ///
    /// [PERFORMANCE] Called once per changed recipe at startup only.
    /// </summary>
    static RecipeOverrideData BuildSoulOverride(Entity recipeEntity)
    {
        var result = new RecipeOverrideData();

        if (recipeEntity.TryGetComponent<RecipeData>(out var recipeData))
            result.CraftDuration = recipeData.CraftDuration;

        if (recipeEntity.TryGetBuffer<RecipeRequirementBuffer>(out var reqBuffer))
        {
            result.Requirements = new List<RecipeSlotData>(reqBuffer.Length);
            for (int i = 0; i < reqBuffer.Length; i++)
            {
                var req = reqBuffer[i];
                PrefabNameResolver.TryResolveName(req.Guid, out string itemName);
                result.Requirements.Add(new RecipeSlotData
                {
                    Item   = string.IsNullOrEmpty(itemName) ? req.Guid._Value.ToString() : itemName,
                    Amount = req.Amount
                });
            }
        }

        if (recipeEntity.TryGetBuffer<RecipeOutputBuffer>(out var outBuffer))
        {
            result.Outputs = new List<RecipeSlotData>(outBuffer.Length);
            for (int i = 0; i < outBuffer.Length; i++)
            {
                var output = outBuffer[i];
                PrefabNameResolver.TryResolveName(output.Guid, out string itemName);
                result.Outputs.Add(new RecipeSlotData
                {
                    Item   = string.IsNullOrEmpty(itemName) ? output.Guid._Value.ToString() : itemName,
                    Amount = output.Amount
                });
            }
        }

        return result;
    }

    // ── Per-field apply ───────────────────────────────────────

    /// <summary>
    /// Applies scalar RecipeData fields to both the prefab entity component
    /// and directly into RecipeHashLookupMap.
    ///
    /// RecipeHashLookupMap is populated from baked scene data at startup and
    /// is not updated by RegisterRecipes() from live entity components. The
    /// crafting system reads CraftDuration and other scalar fields from the
    /// map, not the entity — so both must be written to ensure changes apply.
    ///
    /// [PERFORMANCE] One map read + one map write per changed recipe at
    ///               startup only — no per-frame cost.
    /// </summary>
    static void ApplyRecipeData(Entity recipeEntity, RecipeEntry entry, PrefabGUID guid)
    {
        // ── Write to prefab entity ────────────────────────────
        var data = recipeEntity.Read<RecipeData>();

        if (entry.CraftDuration.HasValue)       data.CraftDuration        = entry.CraftDuration.Value;
        if (entry.AlwaysUnlocked.HasValue)       data.AlwaysUnlocked       = entry.AlwaysUnlocked.Value;
        if (entry.HideInStation.HasValue)        data.HideInStation        = entry.HideInStation.Value;
        if (entry.IgnoreServerSettings.HasValue) data.IgnoreServerSettings = entry.IgnoreServerSettings.Value;
        if (entry.HudSortingOrder.HasValue)      data.HudSortingOrder      = entry.HudSortingOrder.Value;

        recipeEntity.Write(data);

        // ── Write directly into RecipeHashLookupMap ───────────
        // The map is a NativeParallelHashMap — readonly means the map
        // reference can't be reassigned, but its contents are mutable.
        var map = Heart.GameDataSystem.RecipeHashLookupMap;
        if (map.TryGetValue(guid, out var mapEntry))
        {
            if (entry.CraftDuration.HasValue)       mapEntry.CraftDuration        = entry.CraftDuration.Value;
            if (entry.AlwaysUnlocked.HasValue)       mapEntry.AlwaysUnlocked       = entry.AlwaysUnlocked.Value;
            if (entry.HideInStation.HasValue)        mapEntry.HideInStation        = entry.HideInStation.Value;
            if (entry.IgnoreServerSettings.HasValue) mapEntry.IgnoreServerSettings = entry.IgnoreServerSettings.Value;
            if (entry.HudSortingOrder.HasValue)      mapEntry.HudSortingOrder      = entry.HudSortingOrder.Value;
            map[guid] = mapEntry;

            HeartLogger.Debug(LOG_SOURCE,
                $"Updated RecipeHashLookupMap for '{guid._Value}': " +
                $"CraftDuration={mapEntry.CraftDuration}");
        }
        else
        {
            HeartLogger.Warning(LOG_SOURCE,
                $"Recipe GUID {guid._Value} not found in RecipeHashLookupMap — scalar fields may not apply.");
        }
    }

    static void ApplyRequirements(Entity recipeEntity, List<RecipeRequirement> requirements, string recipeName)
    {
        if (!recipeEntity.TryGetBuffer<RecipeRequirementBuffer>(out var buffer))
            buffer = recipeEntity.AddBuffer<RecipeRequirementBuffer>();

        buffer.Clear();

        foreach (var req in requirements)
        {
            if (!PrefabNameResolver.TryResolve(req.Item, out PrefabGUID itemGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve requirement item: '{req.Item}', skipping.");
                continue;
            }

            buffer.Add(new RecipeRequirementBuffer { Guid = itemGuid, Amount = req.Amount });
        }
    }

    static void ApplyOutputs(Entity recipeEntity, List<RecipeOutput> outputs, string recipeName)
    {
        if (!recipeEntity.TryGetBuffer<RecipeOutputBuffer>(out var buffer))
            buffer = recipeEntity.AddBuffer<RecipeOutputBuffer>();

        buffer.Clear();

        foreach (var output in outputs)
        {
            if (!PrefabNameResolver.TryResolve(output.Item, out PrefabGUID itemGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve output item: '{output.Item}', skipping.");
                continue;
            }

            buffer.Add(new RecipeOutputBuffer { Guid = itemGuid, Amount = output.Amount });
        }
    }

    static void ApplyOptionalBuffer<T>(
        Entity recipeEntity,
        bool enabled,
        T? list,
        string recipeName,
        Action<Entity, T, string> applyAction)
        where T : class
    {
        if (!enabled)
        {
            RemoveBuffer<T>(recipeEntity, recipeName);
            return;
        }

        if (list == null)
        {
            HeartLogger.Warning(LOG_SOURCE, $"[{recipeName}] Flag set to true but list is null, skipping.");
            return;
        }

        applyAction(recipeEntity, list, recipeName);
    }

    static void RemoveBuffer<T>(Entity recipeEntity, string recipeName) where T : class
    {
        if (typeof(T) == typeof(List<RecipeRepairCost>))
        {
            if (recipeEntity.Has<ItemRepairBuffer>())
                recipeEntity.Remove<ItemRepairBuffer>();
            else
                HeartLogger.Info(LOG_SOURCE, $"[{recipeName}] ItemRepairBuffer already absent, nothing to remove.");
        }
        else if (typeof(T) == typeof(List<RecipeUnitOutput>))
        {
            if (recipeEntity.Has<RecipeOutputUnitBuffer>())
                recipeEntity.Remove<RecipeOutputUnitBuffer>();
            else
                HeartLogger.Info(LOG_SOURCE, $"[{recipeName}] RecipeOutputUnitBuffer already absent, nothing to remove.");
        }
        else if (typeof(T) == typeof(List<string>))
        {
            if (recipeEntity.Has<RecipeLinkBuffer>())
                recipeEntity.Remove<RecipeLinkBuffer>();
            else
                HeartLogger.Info(LOG_SOURCE, $"[{recipeName}] RecipeLinkBuffer already absent, nothing to remove.");
        }
    }

    static void ApplyRepairCosts(Entity recipeEntity, List<RecipeRepairCost> repairCosts, string recipeName)
    {
        if (!recipeEntity.TryGetBuffer<ItemRepairBuffer>(out var buffer))
            buffer = recipeEntity.AddBuffer<ItemRepairBuffer>();

        buffer.Clear();

        foreach (var cost in repairCosts)
        {
            if (!PrefabNameResolver.TryResolve(cost.Item, out PrefabGUID itemGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve repair cost item: '{cost.Item}', skipping.");
                continue;
            }

            buffer.Add(new ItemRepairBuffer { Guid = itemGuid, Stacks = cost.Amount });
        }
    }

    static void ApplyUnitOutputs(Entity recipeEntity, List<RecipeUnitOutput> unitOutputs, string recipeName)
    {
        if (!recipeEntity.TryGetBuffer<RecipeOutputUnitBuffer>(out var buffer))
            buffer = recipeEntity.AddBuffer<RecipeOutputUnitBuffer>();

        buffer.Clear();

        foreach (var unit in unitOutputs)
        {
            if (!PrefabNameResolver.TryResolve(unit.Unit, out PrefabGUID unitGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve unit output: '{unit.Unit}', skipping.");
                continue;
            }

            buffer.Add(new RecipeOutputUnitBuffer { Guid = unitGuid, Stacks = unit.Amount });
        }
    }

    static void ApplyRecipeLinks(Entity recipeEntity, List<string> recipeLinks, string recipeName)
    {
        if (!recipeEntity.TryGetBuffer<RecipeLinkBuffer>(out var buffer))
            buffer = recipeEntity.AddBuffer<RecipeLinkBuffer>();

        buffer.Clear();

        foreach (var linkName in recipeLinks)
        {
            if (!PrefabNameResolver.TryResolve(linkName, out PrefabGUID linkGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve recipe link: '{linkName}', skipping.");
                continue;
            }

            buffer.Add(new RecipeLinkBuffer { Guid = linkGuid });
        }
    }
}