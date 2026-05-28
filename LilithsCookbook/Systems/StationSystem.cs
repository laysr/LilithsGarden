using ProjectM;
using ProjectM.Tiles;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Foundation;
using LilithsHeart.Services;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

// ============================================================
//  StationSystem — LilithsCookbook
//
//  Applies crafting station recipe changes from stations.json.
//
//  Supports two buffer types:
//    • RefinementstationRecipesBuffer — refining stations
//      (Furnace, Grinder, etc.) where production is automatic.
//    • WorkstationRecipesBuffer — crafting stations (Simple
//      Workbench, etc.) and the player entity crafting menu.
//      Buffer type is detected automatically per station entry.
//
//  Two-pass approach:
//  ──────────────────
//  Pass 1: Patch all prefab entities (all buffer types).
//  Registration: RegisterRecipes() + RegisterGameData().
//  Pass 2: Patch live entities only — User entities for player
//          crafting, placed station entities for WorkstationRecipesBuffer.
//
//  Why two passes?
//  ───────────────
//  RegisterGameData() resets WorkstationRecipesBuffer on all live
//  entities. Pass 1 prefab patches survive this reset. Pass 2 live
//  entity patches happen after registration so they persist.
//
//  Why CreateEntityQuery was insufficient:
//  ────────────────────────────────────────
//  Placed WorkstationRecipesBuffer station entities have the Prefab
//  ECS tag, which causes CreateEntityQuery to exclude them by default.
//  Pass 2 uses an explicit Prefab + TileModel filter to find them.
//
//  [PERFORMANCE] All ECS operations run once at startup only.
//                No per-frame cost after initialization.
// ============================================================

public static class StationSystem
{
    private const string LOG_SOURCE = "LilithsCookbook.StationSystem";

    public static void ApplyChanges()
    {
        var config = CookbookPlugin.StationData;

        if (config == null || config.Stations.Count == 0)
        {
            HeartLogger.Info(LOG_SOURCE, "No station changes configured.");
            return;
        }

        int enabled = config.Stations.Count(kvp => kvp.Value.ChangesEnabled);
        if (enabled == 0)
        {
            HeartLogger.Info(LOG_SOURCE, "No stations had ChangesEnabled = true, skipping.");
            return;
        }

        // ── Pass 1: Patch all prefab entities ─────────────────────────────────
        // Patches RefinementstationRecipesBuffer and WorkstationRecipesBuffer
        // prefab entities. RegisterGameData() resets WorkstationRecipesBuffer on
        // live entities after this — prefab patches survive unaffected.

        foreach (var (stationName, entry) in config.Stations)
        {
            if (!entry.ChangesEnabled) continue;

            if (!PrefabNameResolver.TryResolve(stationName, out PrefabGUID guid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"Could not resolve station: '{stationName}'");
                continue;
            }

            if (!Heart.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(guid, out Entity stationEntity))
            {
                HeartLogger.Warning(LOG_SOURCE, $"Could not find prefab entity for station: '{stationName}'");
                continue;
            }

            bool hasRefinement  = stationEntity.Has<RefinementstationRecipesBuffer>();
            bool hasWorkstation = stationEntity.Has<WorkstationRecipesBuffer>();

            if (!hasRefinement && !hasWorkstation)
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"'{stationName}' has neither RefinementstationRecipesBuffer nor " +
                    $"WorkstationRecipesBuffer — skipping.");
                continue;
            }

            if (entry.AddRecipes.Count > 0)
            {
                if (hasRefinement)
                    AddRefinementRecipes(stationEntity, entry.AddRecipes, stationName);
                else
                    AddWorkstationRecipes(stationEntity, entry.AddRecipes, stationName);
            }

            if (entry.RemoveRecipes.Count > 0)
            {
                if (hasRefinement)
                    RemoveRefinementRecipes(stationEntity, entry.RemoveRecipes, stationName);
                else
                    RemoveWorkstationRecipes(stationEntity, entry.RemoveRecipes, stationName);
            }

            HeartLogger.Info(LOG_SOURCE, $"[Pass 1] Patched prefab: '{stationName}'");
        }

        // ── Registration ──────────────────────────────────────────────────────
        // RegisterGameData() resets WorkstationRecipesBuffer on all live entities.
        // Live entity patching must happen after this call.
        Heart.GameDataSystem.RegisterRecipes();
        Heart.PrefabCollectionSystem.RegisterGameData();

        // ── Pass 2: Patch all live entities ───────────────────────────────────
        // Patches live User entities (player crafting menu) and placed station
        // entities. Refinement stations require no live entity patching.

        int changed = 0;

        foreach (var (stationName, entry) in config.Stations)
        {
            if (!entry.ChangesEnabled) continue;

            if (!PrefabNameResolver.TryResolve(stationName, out PrefabGUID guid)) continue;

            if (!Heart.PrefabCollectionSystem._PrefabGuidToEntityMap.TryGetValue(guid, out Entity stationEntity)) continue;

            bool hasWorkstation = stationEntity.Has<WorkstationRecipesBuffer>();
            bool isPlayerEntity = stationEntity.Has<ProjectM.Network.User>();

            // Refinement stations need no live entity patching.
            if (!hasWorkstation) continue;

            if (isPlayerEntity)
            {
                PatchLiveUserEntities(entry.AddRecipes, entry.RemoveRecipes, stationName);
                Heart.RegisterPlayerRecipeChanges(entry.AddRecipes, entry.RemoveRecipes);
                HeartLogger.Info(LOG_SOURCE,
                    $"[{stationName}] Registered {entry.AddRecipes.Count} add(s) and " +
                    $"{entry.RemoveRecipes.Count} remove(s) with Heart for Soul sync.");
            }
            else
            {
                PatchLiveStationEntities(guid, entry.AddRecipes, entry.RemoveRecipes, stationName);
                Heart.RegisterStationRecipeChanges(stationName, entry.AddRecipes, entry.RemoveRecipes);
            }

            changed++;
        }

        HeartLogger.Info(LOG_SOURCE, $"LilithsCookbook applied changes to {changed} station(s).");
    }

    // ── Live User entity patching ─────────────────────────────────────────────

    /// <summary>
    /// Patches WorkstationRecipesBuffer on all live User entities.
    /// Called in Pass 2 after RegisterGameData() so changes persist.
    ///
    /// [PERFORMANCE] One targeted query at startup — no per-frame cost.
    /// </summary>
    static void PatchLiveUserEntities(
        List<string> addRecipes,
        List<string> removeRecipes,
        string stationName)
    {
        var em = Heart.EntityManager;

        var query = em.CreateEntityQuery(
            ComponentType.ReadWrite<WorkstationRecipesBuffer>(),
            ComponentType.ReadOnly<ProjectM.Network.User>()
        );

        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        try
        {
            HeartLogger.Info(LOG_SOURCE,
                $"[{stationName}] Patching {entities.Length} live User entity(s).");

            foreach (var userEntity in entities)
            {
                if (addRecipes.Count > 0)
                    AddWorkstationRecipes(userEntity, addRecipes, stationName);

                if (removeRecipes.Count > 0)
                    RemoveWorkstationRecipes(userEntity, removeRecipes, stationName);
            }
        }
        finally
        {
            entities.Dispose();
        }
    }

    // ── Live placed station entity patching ───────────────────────────────────

    /// <summary>
    /// Patches WorkstationRecipesBuffer on all placed world instances of a
    /// station matching the given PrefabGUID.
    ///
    /// Why Prefab + TileModel filter:
    /// ───────────────────────────────
    /// Placed WorkstationRecipesBuffer station entities have the Unity ECS
    /// Prefab tag, which causes CreateEntityQuery to exclude them by default.
    /// Adding ComponentType.ReadOnly&lt;Prefab&gt;() opts the query back in to
    /// entities with that tag. TileModel confirms the entity is a placed
    /// building rather than an inventory container or other entity type.
    ///
    /// [PERFORMANCE] One targeted query per WorkstationRecipesBuffer station
    ///               at startup — no per-frame cost.
    /// </summary>
    static void PatchLiveStationEntities(
        PrefabGUID stationGuid,
        List<string> addRecipes,
        List<string> removeRecipes,
        string stationName)
    {
        var em = Heart.EntityManager;

        // Prefab tag is required to include placed station entities which Unity
        // ECS excludes from queries by default. TileModel confirms building entity.
        var query = em.CreateEntityQuery(
            ComponentType.ReadWrite<WorkstationRecipesBuffer>(),
            ComponentType.ReadOnly<TileModel>(),
            ComponentType.ReadOnly<Unity.Entities.Prefab>()
        );

        var entities = query.ToEntityArray(Unity.Collections.Allocator.Temp);

        try
        {
            int patched = 0;

            foreach (var entity in entities)
            {
                if (!em.HasComponent<Stunlock.Core.PrefabGUID>(entity)) continue;

                var prefabGuid = em.GetComponentData<Stunlock.Core.PrefabGUID>(entity);
                if (!prefabGuid.Equals(stationGuid)) continue;

                if (addRecipes.Count > 0)
                    AddWorkstationRecipes(entity, addRecipes, stationName);

                if (removeRecipes.Count > 0)
                    RemoveWorkstationRecipes(entity, removeRecipes, stationName);

                patched++;
            }

            HeartLogger.Info(LOG_SOURCE,
                $"[{stationName}] Patched {patched} live station entity(s).");
        }
        finally
        {
            entities.Dispose();
        }
    }

    // ── RefinementstationRecipesBuffer helpers ────────────────────────────────

    static void AddRefinementRecipes(Entity stationEntity, List<string> recipes, string stationName)
    {
        var buffer = stationEntity.ReadBuffer<RefinementstationRecipesBuffer>();

        foreach (var recipeName in recipes)
        {
            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID recipeGuid))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"[{stationName}] Could not resolve recipe to add: '{recipeName}', skipping.");
                continue;
            }

            bool alreadyExists = false;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].RecipeGuid.Equals(recipeGuid))
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                HeartLogger.Info(LOG_SOURCE,
                    $"[{stationName}] Recipe '{recipeName}' already present, skipping.");
                continue;
            }

            buffer.Add(new RefinementstationRecipesBuffer
            {
                RecipeGuid = recipeGuid,
                Disabled   = false,
                Unlocked   = true
            });

            HeartLogger.Info(LOG_SOURCE, $"[{stationName}] Added recipe '{recipeName}'.");
        }
    }

    static void RemoveRefinementRecipes(Entity stationEntity, List<string> recipes, string stationName)
    {
        var buffer = stationEntity.ReadBuffer<RefinementstationRecipesBuffer>();

        foreach (var recipeName in recipes)
        {
            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID recipeGuid))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"[{stationName}] Could not resolve recipe to remove: '{recipeName}', skipping.");
                continue;
            }

            bool found = false;
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                if (buffer[i].RecipeGuid.Equals(recipeGuid))
                {
                    buffer.RemoveAt(i);
                    HeartLogger.Info(LOG_SOURCE, $"[{stationName}] Removed recipe '{recipeName}'.");
                    found = true;
                    break;
                }
            }

            if (!found)
                HeartLogger.Info(LOG_SOURCE,
                    $"[{stationName}] Recipe '{recipeName}' not found in station, nothing to remove.");
        }
    }

    // ── WorkstationRecipesBuffer helpers ──────────────────────────────────────

    static void AddWorkstationRecipes(Entity stationEntity, List<string> recipes, string stationName)
    {
        var buffer = stationEntity.ReadBuffer<WorkstationRecipesBuffer>();

        foreach (var recipeName in recipes)
        {
            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID recipeGuid))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"[{stationName}] Could not resolve recipe to add: '{recipeName}', skipping.");
                continue;
            }

            bool alreadyExists = false;
            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i].RecipeGuid.Equals(recipeGuid))
                {
                    alreadyExists = true;
                    break;
                }
            }

            if (alreadyExists)
            {
                HeartLogger.Info(LOG_SOURCE,
                    $"[{stationName}] Recipe '{recipeName}' already present, skipping.");
                continue;
            }

            buffer.Add(new WorkstationRecipesBuffer { RecipeGuid = recipeGuid });
            HeartLogger.Info(LOG_SOURCE, $"[{stationName}] Added recipe '{recipeName}'.");
        }
    }

    static void RemoveWorkstationRecipes(Entity stationEntity, List<string> recipes, string stationName)
    {
        var buffer = stationEntity.ReadBuffer<WorkstationRecipesBuffer>();

        foreach (var recipeName in recipes)
        {
            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID recipeGuid))
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"[{stationName}] Could not resolve recipe to remove: '{recipeName}', skipping.");
                continue;
            }

            bool found = false;
            for (int i = buffer.Length - 1; i >= 0; i--)
            {
                if (buffer[i].RecipeGuid.Equals(recipeGuid))
                {
                    buffer.RemoveAt(i);
                    HeartLogger.Info(LOG_SOURCE, $"[{stationName}] Removed recipe '{recipeName}'.");
                    found = true;
                    break;
                }
            }

            if (!found)
                HeartLogger.Info(LOG_SOURCE,
                    $"[{stationName}] Recipe '{recipeName}' not found in station, nothing to remove.");
        }
    }
}