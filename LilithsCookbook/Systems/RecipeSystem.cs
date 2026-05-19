using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Foundation;
using LilithsHeart.Prefabs;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

// [CHANGED] LilithsLogger → HeartLogger throughout.
//           Added using LilithsHeart.Prefabs for PrefabNameResolver.
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

        var recipeMap = Heart.GameDataSystem.RecipeHashLookupMap;
        int changed   = 0;

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
                ApplyRecipeData(recipeEntity, entry);
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

            // Changes are applied in-place via Write() calls in each Apply* method.
            // No write-back to recipeMap needed — RegisterRecipes() re-reads from ECS.
            changed++;
        }

        if (changed > 0)
        {
            Heart.GameDataSystem.RegisterRecipes();
            HeartLogger.Info(LOG_SOURCE, $"LilithsCookbook applied changes to {changed} recipe(s).");
        }
        else
        {
            HeartLogger.Info(LOG_SOURCE, "No recipes had ChangesEnabled = true, skipping registration.");
        }
    }

    // ── Per-field apply ───────────────────────────────────────────────────────

    static void ApplyRecipeData(Entity recipeEntity, RecipeEntry entry)
    {
        var data = recipeEntity.Read<RecipeData>();

        if (entry.CraftDuration.HasValue)       data.CraftDuration        = entry.CraftDuration.Value;
        if (entry.AlwaysUnlocked.HasValue)       data.AlwaysUnlocked       = entry.AlwaysUnlocked.Value;
        if (entry.HideInStation.HasValue)        data.HideInStation        = entry.HideInStation.Value;
        if (entry.IgnoreServerSettings.HasValue) data.IgnoreServerSettings = entry.IgnoreServerSettings.Value;
        if (entry.HudSortingOrder.HasValue)      data.HudSortingOrder      = entry.HudSortingOrder.Value;

        recipeEntity.Write(data);
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

    // ── Optional buffer handler ───────────────────────────────────────────────

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

    // ── Per-buffer apply methods ──────────────────────────────────────────────

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