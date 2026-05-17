using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart;
using LilithsHeart.Systems;
using LilithsHeart.Extensions;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class RecipeSystem
{
    public static void ApplyChanges()
    {
        // Changed: Plugin.CookbookRecipeData -> Plugin.RecipeData
        var config = Plugin.RecipeData;

        if (config == null || config.Recipes.Count == 0)
        {
            LilithsLogger.Info("No recipe changes configured.");
            return;
        }

        var recipeMap = Core.GameDataSystem.RecipeHashLookupMap;
        int changed = 0;

        foreach (var (recipeName, entry) in config.Recipes)
        {
            if (!entry.ChangesEnabled) continue;

            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID guid))
            {
                LilithsLogger.Warning($"Could not resolve recipe: {recipeName}");
                continue;
            }

            if (!Core.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(guid, out Entity recipeEntity))
            {
                LilithsLogger.Warning($"Could not find prefab entity for recipe: {recipeName}");
                continue;
            }

            ApplyRecipeData(recipeEntity, entry);

            if (entry.Requirements != null)
                ApplyRequirements(recipeEntity, entry.Requirements);

            if (entry.Outputs != null)
                ApplyOutputs(recipeEntity, entry.Outputs);

            if (entry.RepairCosts != null)
                ApplyRepairCosts(recipeEntity, entry.RepairCosts);

            if (entry.UnitOutputs != null)
                ApplyUnitOutputs(recipeEntity, entry.UnitOutputs);

            // Changed: Read<CookbookRecipeData> -> Read<RecipeData> using game's RecipeData type
            recipeMap[guid] = recipeEntity.Read<RecipeData>();

            changed++;
            LilithsLogger.Info($"Applied changes to recipe: {recipeName}");
        }

        Core.GameDataSystem.RegisterRecipes();
        Core.GameDataSystem.RegisterItems();
        Core.PrefabCollectionSystem.RegisterGameData();

        LilithsLogger.Info($"LilithsCookbook applied changes to {changed} recipes.");
    }

    static void ApplyRecipeData(Entity recipeEntity, RecipeEntry entry)
    {
        // Changed: With<CookbookRecipeData> -> With<RecipeData> using game's RecipeData type
        recipeEntity.With((ref RecipeData recipeData) =>
        {
            if (entry.CraftDuration.HasValue)
                recipeData.CraftDuration = entry.CraftDuration.Value;

            if (entry.AlwaysUnlocked.HasValue)
                recipeData.AlwaysUnlocked = entry.AlwaysUnlocked.Value;

            if (entry.HideInStation.HasValue)
                recipeData.HideInStation = entry.HideInStation.Value;

            if (entry.IgnoreServerSettings.HasValue)
                recipeData.IgnoreServerSettings = entry.IgnoreServerSettings.Value;

            if (entry.HudSortingOrder.HasValue)
                recipeData.HudSortingOrder = entry.HudSortingOrder.Value;
        });
    }

    static void ApplyRequirements(Entity recipeEntity, List<RecipeRequirement> requirements)
    {
        if (!recipeEntity.Has<RecipeRequirementBuffer>())
            recipeEntity.AddBuffer<RecipeRequirementBuffer>();

        var buffer = recipeEntity.ReadBuffer<RecipeRequirementBuffer>();
        buffer.Clear();

        foreach (var req in requirements)
        {
            if (!PrefabNameResolver.TryResolve(req.Item, out PrefabGUID itemGuid))
            {
                LilithsLogger.Warning($"Could not resolve requirement item: {req.Item}");
                continue;
            }

            buffer.Add(new RecipeRequirementBuffer
            {
                Guid = itemGuid,
                Amount = req.Amount
            });
        }
    }

    static void ApplyOutputs(Entity recipeEntity, List<RecipeOutput> outputs)
    {
        if (!recipeEntity.Has<RecipeOutputBuffer>())
            recipeEntity.AddBuffer<RecipeOutputBuffer>();

        var buffer = recipeEntity.ReadBuffer<RecipeOutputBuffer>();
        buffer.Clear();

        foreach (var output in outputs)
        {
            if (!PrefabNameResolver.TryResolve(output.Item, out PrefabGUID itemGuid))
            {
                LilithsLogger.Warning($"Could not resolve output item: {output.Item}");
                continue;
            }

            buffer.Add(new RecipeOutputBuffer
            {
                Guid = itemGuid,
                Amount = output.Amount
            });
        }
    }

    static void ApplyRepairCosts(Entity recipeEntity, List<RecipeRepairCost> repairCosts)
    {
        if (repairCosts.Count == 0)
        {
            if (recipeEntity.Has<ItemRepairBuffer>())
                recipeEntity.Remove<ItemRepairBuffer>();
            return;
        }

        if (!recipeEntity.Has<ItemRepairBuffer>())
            recipeEntity.AddBuffer<ItemRepairBuffer>();

        var buffer = recipeEntity.ReadBuffer<ItemRepairBuffer>();
        buffer.Clear();

        foreach (var cost in repairCosts)
        {
            if (!PrefabNameResolver.TryResolve(cost.Item, out PrefabGUID itemGuid))
            {
                LilithsLogger.Warning($"Could not resolve repair cost item: {cost.Item}");
                continue;
            }

            buffer.Add(new ItemRepairBuffer
            {
                Guid = itemGuid,
                Stacks = cost.Amount
            });
        }
    }

    static void ApplyUnitOutputs(Entity recipeEntity, List<RecipeUnitOutput> unitOutputs)
    {
        if (unitOutputs.Count == 0)
        {
            if (recipeEntity.Has<RecipeOutputUnitBuffer>())
                recipeEntity.Remove<RecipeOutputUnitBuffer>();
            return;
        }

        if (!recipeEntity.Has<RecipeOutputUnitBuffer>())
            recipeEntity.AddBuffer<RecipeOutputUnitBuffer>();

        var buffer = recipeEntity.ReadBuffer<RecipeOutputUnitBuffer>();
        buffer.Clear();

        foreach (var unit in unitOutputs)
        {
            if (!PrefabNameResolver.TryResolve(unit.Unit, out PrefabGUID unitGuid))
            {
                LilithsLogger.Warning($"Could not resolve unit output: {unit.Unit}");
                continue;
            }

            buffer.Add(new RecipeOutputUnitBuffer
            {
                Guid = unitGuid,
                Stacks = unit.Amount
            });
        }
    }
}