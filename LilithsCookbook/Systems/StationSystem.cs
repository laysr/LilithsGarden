using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Foundation;
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
//  Pass 1: Patch prefab entities, then call RegisterRecipes()
//          and RegisterGameData().
//  Pass 2: Re-patch WorkstationRecipesBuffer prefab entities
//          (RegisterGameData resets them), patch live User
//          entities for player crafting, and patch live placed
//          station entities via GetAllEntities scan.
//
//  [PERFORMANCE] GetAllEntities scans all ~560K entities once
//                at startup per WorkstationRecipesBuffer station.
//                Acceptable cost for a one-time startup operation.
//                No per-frame cost after startup.
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
        // Patches RefinementstationRecipesBuffer and WorkstationRecipesBuffer prefab
        // entities. RegisterGameData() will reset WorkstationRecipesBuffer live
        // entities after this — prefab patches survive.

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
        // entities. Refinement stations have no live entity patching needed.
        //
        // [PERFORMANCE] GetAllEntities scans ~560K entities once per
        //               WorkstationRecipesBuffer station — no per-frame cost.

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
    /// [PERFORMANCE] One query at startup — no per-frame cost.
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
    /// Patches WorkstationRecipesBuffer on all placed world instances of a station
    /// matching the given PrefabGUID.
    ///
    /// Uses GetAllEntities() because CreateEntityQuery cannot find placed station
    /// entities — they exist in a different ECS chunk and are not returned by
    /// component queries. GetAllEntities() is required to locate them by GUID.
    ///
    /// Called in Pass 2 after RegisterGameData() so placed entities have been
    /// restored from the world save and are patchable.
    ///
    /// [PERFORMANCE] Scans all entities once per WorkstationRecipesBuffer station
    ///               at startup. Acceptable cost for a one-time startup operation.
    /// </summary>
    static void PatchLiveStationEntities(
        PrefabGUID stationGuid,
        List<string> addRecipes,
        List<string> removeRecipes,
        string stationName)
    {
        var em = Heart.EntityManager;
        var allEntities = em.GetAllEntities(Unity.Collections.Allocator.Temp);

        try
        {
            int patched = 0;

            foreach (var entity in allEntities)
            {
                if (!em.HasComponent<Stunlock.Core.PrefabGUID>(entity)) continue;

                var prefabGuid = em.GetComponentData<Stunlock.Core.PrefabGUID>(entity);
                if (!prefabGuid.Equals(stationGuid)) continue;

                if (!em.HasBuffer<WorkstationRecipesBuffer>(entity)) continue;

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
            allEntities.Dispose();
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