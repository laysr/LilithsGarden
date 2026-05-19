using ProjectM;
using Stunlock.Core;
using Unity.Entities;
using LilithsHeart.Foundation;
using LilithsHeart.Prefabs;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

// [CHANGED] LilithsLogger → HeartLogger throughout.
//           Added using LilithsHeart.Prefabs for PrefabNameResolver.
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

        int changed = 0;

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

            if (!stationEntity.Has<RefinementstationRecipesBuffer>())
            {
                HeartLogger.Warning(LOG_SOURCE, $"Station '{stationName}' does not have a RefinementstationRecipesBuffer, skipping.");
                continue;
            }

            if (entry.AddRecipes.Count > 0)
                AddRecipes(stationEntity, entry.AddRecipes, stationName);

            if (entry.RemoveRecipes.Count > 0)
                RemoveRecipes(stationEntity, entry.RemoveRecipes, stationName);

            changed++;
            HeartLogger.Info(LOG_SOURCE, $"Applied changes to station: '{stationName}'");
        }

        if (changed > 0)
        {
            Heart.GameDataSystem.RegisterRecipes();
            Heart.PrefabCollectionSystem.RegisterGameData();
            HeartLogger.Info(LOG_SOURCE, $"LilithsCookbook applied changes to {changed} station(s).");
        }
        else
        {
            HeartLogger.Info(LOG_SOURCE, "No stations had ChangesEnabled = true, skipping registration.");
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static void AddRecipes(Entity stationEntity, List<string> recipes, string stationName)
    {
        var buffer = stationEntity.ReadBuffer<RefinementstationRecipesBuffer>();

        foreach (var recipeName in recipes)
        {
            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID recipeGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{stationName}] Could not resolve recipe to add: '{recipeName}', skipping.");
                continue;
            }

            // Check for duplicates before adding.
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
                HeartLogger.Info(LOG_SOURCE, $"[{stationName}] Recipe '{recipeName}' already present, skipping.");
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

    static void RemoveRecipes(Entity stationEntity, List<string> recipes, string stationName)
    {
        var buffer = stationEntity.ReadBuffer<RefinementstationRecipesBuffer>();

        foreach (var recipeName in recipes)
        {
            if (!PrefabNameResolver.TryResolve(recipeName, out PrefabGUID recipeGuid))
            {
                HeartLogger.Warning(LOG_SOURCE, $"[{stationName}] Could not resolve recipe to remove: '{recipeName}', skipping.");
                continue;
            }

            bool found = false;

            // Iterate backwards when removing from a DynamicBuffer to keep indices stable.
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
                HeartLogger.Info(LOG_SOURCE, $"[{stationName}] Recipe '{recipeName}' not found in station, nothing to remove.");
        }
    }
}