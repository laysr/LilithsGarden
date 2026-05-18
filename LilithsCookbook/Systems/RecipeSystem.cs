using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart;
using LilithsHeart.Systems;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class RecipeSystem
{
    private const string LOG_SOURCE = "LilithsCookbook.RecipeSystem";

    public static void ApplyChanges()
    {
        var config = CookbookPlugin.RecipeData;

        if (config == null || config.Recipes.Count == 0)
        {
            LilithsLogger.Info(LOG_SOURCE, "No recipe changes configured.");
            return;
        }

        var recipeMap = Heart.GameDataSystem.RecipeHashLookupMap;
        int changed = 0;

        foreach (var (recipeName, entry) in config.Recipes)
        {
            if (!entry.ChangesEnabled) continue;

            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID guid))
            {
                LilithsLogger.Warning(LOG_SOURCE, $"Could not resolve recipe: '{recipeName}'");
                continue;
            }

            if (!Heart.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(guid, out Entity recipeEntity))
            {
                LilithsLogger.Warning(LOG_SOURCE, $"Could not find prefab entity for recipe: '{recipeName}'");
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

            // Optional buffer flags:
            //   null  -> not specified, skip entirely
            //   false -> remove the buffer
            //   true  -> ensure buffer exists and apply the list
            if (entry.UseRepairCosts.HasValue)
                ApplyOptionalBuffer(recipeEntity, entry.UseRepairCosts.Value, entry.RepairCosts, recipeName,
                    ApplyRepairCosts);

            if (entry.UseUnitOutputs.HasValue)
                ApplyOptionalBuffer(recipeEntity, entry.UseUnitOutputs.Value, entry.UnitOutputs, recipeName,
                    ApplyUnitOutputs);

            if (entry.UseRecipeLinks.HasValue)
                ApplyOptionalBuffer(recipeEntity, entry.UseRecipeLinks.Value, entry.RecipeLinks, recipeName,
                    ApplyRecipeLinks);

            // Keep the game's recipe lookup map in sync after modifying the entity.
            recipeMap[guid] = recipeEntity.Read<RecipeData>();

            changed++;
            LilithsLogger.Info(LOG_SOURCE, $"Applied changes to recipe: '{recipeName}'");
        }

        if (changed > 0)
        {
            Heart.GameDataSystem.RegisterRecipes();
            Heart.GameDataSystem.RegisterItems();
            Heart.PrefabCollectionSystem.RegisterGameData();
            LilithsLogger.Info(LOG_SOURCE, $"LilithsCookbook applied changes to {changed} recipe(s).");
        }
        else
        {
            LilithsLogger.Info(LOG_SOURCE, "No recipes had ChangesEnabled = true, skipping registration.");
        }
    }

    // ── Apply helpers ─────────────────────────────────────────────────────────

    static void ApplyRecipeData(Entity recipeEntity, RecipeEntry entry)
    {
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

    static void ApplyRequirements(Entity recipeEntity, List<RecipeRequirement> requirements, string recipeName)
    {
        if (!recipeEntity.TryGetBuffer<RecipeRequirementBuffer>(out var buffer))
            buffer = recipeEntity.AddBuffer<RecipeRequirementBuffer>();

        buffer.Clear();

        foreach (var req in requirements)
        {
            if (!PrefabNameResolver.TryResolve(req.Item, out PrefabGUID itemGuid))
            {
                LilithsLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve requirement item: '{req.Item}', skipping.");
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
                LilithsLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve output item: '{output.Item}', skipping.");
                continue;
            }

            buffer.Add(new RecipeOutputBuffer { Guid = itemGuid, Amount = output.Amount });
        }
    }

    // ── Optional buffer handler ───────────────────────────────────────────────

    /// <summary>
    /// Generic handler for optional buffers controlled by a bool flag.
    /// If enabled is false, removes the buffer. If true, delegates to the apply action.
    /// </summary>
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
            LilithsLogger.Warning(LOG_SOURCE, $"[{recipeName}] Flag set to true but list is null, skipping.");
            return;
        }

        applyAction(recipeEntity, list, recipeName);
    }

    static void RemoveBuffer<T>(Entity recipeEntity, string recipeName) where T : class
    {
        // Match list type to its corresponding ECS buffer type for removal.
        if (typeof(T) == typeof(List<RecipeRepairCost>))
        {
            if (recipeEntity.Has<ItemRepairBuffer>())
                recipeEntity.Remove<ItemRepairBuffer>();
            else
                LilithsLogger.Info(LOG_SOURCE, $"[{recipeName}] ItemRepairBuffer already absent, nothing to remove.");
        }
        else if (typeof(T) == typeof(List<RecipeUnitOutput>))
        {
            if (recipeEntity.Has<RecipeOutputUnitBuffer>())
                recipeEntity.Remove<RecipeOutputUnitBuffer>();
            else
                LilithsLogger.Info(LOG_SOURCE, $"[{recipeName}] RecipeOutputUnitBuffer already absent, nothing to remove.");
        }
        else if (typeof(T) == typeof(List<string>))
        {
            if (recipeEntity.Has<RecipeLinkBuffer>())
                recipeEntity.Remove<RecipeLinkBuffer>();
            else
                LilithsLogger.Info(LOG_SOURCE, $"[{recipeName}] RecipeLinkBuffer already absent, nothing to remove.");
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
                LilithsLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve repair cost item: '{cost.Item}', skipping.");
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
                LilithsLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve unit output: '{unit.Unit}', skipping.");
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

        foreach (var link in recipeLinks)
        {
            if (!PrefabNameResolver.TryResolve(link, out PrefabGUID linkGuid))
            {
                LilithsLogger.Warning(LOG_SOURCE, $"[{recipeName}] Could not resolve recipe link: '{link}', skipping.");
                continue;
            }

            buffer.Add(new RecipeLinkBuffer { Guid = linkGuid });
        }
    }
}
