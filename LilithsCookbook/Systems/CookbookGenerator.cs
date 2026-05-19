using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectM;
using LilithsHeart.Config;
using LilithsHeart.Foundation;
using LilithsHeart.Prefabs;
using LilithsCookbook.Config;   // [CHANGED] was LilithsCookbook.Systems
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class CookbookGenerator
{
    private const string LOG_SOURCE = "LilithsCookbook.CookbookGenerator";

    // All suite config lives under BepInEx/config/LilithsHeart/ — child
    // modules do not create their own subfolder. Recipes and Stations are
    // category directories directly under the Heart root, consistent with
    // how Localization/ and Names/ are structured.
    public static readonly string RecipesDir  = HeartPaths.DataDir("Recipes");
    public static readonly string StationsDir = HeartPaths.DataDir("Stations");

    static readonly string ExampleRecipesPath  = Path.Combine(RecipesDir,  "example-recipes.json");
    static readonly string ExampleStationsPath = Path.Combine(StationsDir, "example-stations.json");
    static readonly string AllRecipesPath      = Path.Combine(RecipesDir,  "all-recipes.json");

    static readonly JsonSerializerOptions _writeOptions = new()
    {
        WriteIndented          = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    // ── Initialization (no ECS — safe to call from Plugin.Load) ─────────────

    /// <summary>
    /// Creates config directories and writes example files if they don't exist.
    /// Call this from CookbookPlugin.Load() before Heart is ready.
    /// </summary>
    public static void Initialize()
    {
        Directory.CreateDirectory(RecipesDir);
        Directory.CreateDirectory(StationsDir);

        if (!File.Exists(ExampleRecipesPath))
            WriteExampleRecipes();

        if (!File.Exists(ExampleStationsPath))
            WriteExampleStations();
    }

    // ── ECS generation (call after Heart.OnInitialized) ──────────────────────

    /// <summary>
    /// If GenerateAllRecipes is enabled in CookbookConfig, iterates all entries in
    /// GameDataSystem.RecipeHashLookupMap, serializes their current vanilla state to
    /// all-recipes.json, then disables the setting so it does not run on next boot.
    ///
    /// We use RecipeHashLookupMap instead of iterating the full PrefabGuidToEntityMap
    /// because the prefab map contains all entity types and HasComponent checks on
    /// non-recipe entities are unreliable in this context.
    /// </summary>
    public static void GenerateAllRecipesIfRequested()
    {
        if (!CookbookConfig.GenerateAllRecipes) return;

        // [CHANGED] LilithsLogger → HeartLogger throughout this file.
        HeartLogger.Info(LOG_SOURCE, "GenerateAllRecipes is enabled — generating all-recipes.json...");

        try
        {
            var recipeMap = Heart.GameDataSystem.RecipeHashLookupMap;
            var entries   = new Dictionary<string, RecipeEntry>(recipeMap.Count());

            foreach (var kvp in recipeMap)
            {
                var entity = kvp.Value;

                if (!PrefabNameResolver.TryResolveName(kvp.Key, out string recipeName))
                    recipeName = kvp.Key.GuidHash.ToString();

                var entry = new RecipeEntry { ChangesEnabled = false };

                if (entity.Has<RecipeData>())
                {
                    var data = entity.Read<RecipeData>();
                    entry.CraftDuration        = data.CraftDuration;
                    entry.AlwaysUnlocked       = data.AlwaysUnlocked;
                    entry.HideInStation        = data.HideInStation;
                    entry.IgnoreServerSettings = data.IgnoreServerSettings;
                    entry.HudSortingOrder      = data.HudSortingOrder;
                }

                if (entity.TryGetBuffer<RecipeRequirementBuffer>(out var reqBuffer) && reqBuffer.Length > 0)
                {
                    entry.Requirements = new List<RecipeRequirement>(reqBuffer.Length);
                    for (int i = 0; i < reqBuffer.Length; i++)
                    {
                        var req = reqBuffer[i];
                        PrefabNameResolver.TryResolveName(req.Guid, out string itemName);
                        entry.Requirements.Add(new RecipeRequirement
                        {
                            Item   = string.IsNullOrEmpty(itemName) ? req.Guid._Value.ToString() : itemName,
                            Amount = req.Amount
                        });
                    }
                }

                if (entity.TryGetBuffer<RecipeOutputBuffer>(out var outBuffer) && outBuffer.Length > 0)
                {
                    entry.Outputs = new List<RecipeOutput>(outBuffer.Length);
                    for (int i = 0; i < outBuffer.Length; i++)
                    {
                        var output = outBuffer[i];
                        PrefabNameResolver.TryResolveName(output.Guid, out string itemName);
                        entry.Outputs.Add(new RecipeOutput
                        {
                            Item   = string.IsNullOrEmpty(itemName) ? output.Guid._Value.ToString() : itemName,
                            Amount = output.Amount
                        });
                    }
                }

                if (entity.TryGetBuffer<ItemRepairBuffer>(out var repairBuffer) && repairBuffer.Length > 0)
                {
                    entry.UseRepairCosts = true;
                    entry.RepairCosts    = new List<RecipeRepairCost>(repairBuffer.Length);
                    for (int i = 0; i < repairBuffer.Length; i++)
                    {
                        var cost = repairBuffer[i];
                        PrefabNameResolver.TryResolveName(cost.Guid, out string itemName);
                        entry.RepairCosts.Add(new RecipeRepairCost
                        {
                            Item   = string.IsNullOrEmpty(itemName) ? cost.Guid._Value.ToString() : itemName,
                            Amount = cost.Stacks
                        });
                    }
                }

                if (entity.TryGetBuffer<RecipeOutputUnitBuffer>(out var unitBuffer) && unitBuffer.Length > 0)
                {
                    entry.UnitOutputs = new List<RecipeUnitOutput>(unitBuffer.Length);
                    for (int i = 0; i < unitBuffer.Length; i++)
                    {
                        var unit = unitBuffer[i];
                        PrefabNameResolver.TryResolveName(unit.Guid, out string unitName);
                        entry.UnitOutputs.Add(new RecipeUnitOutput
                        {
                            Unit   = string.IsNullOrEmpty(unitName) ? unit.Guid._Value.ToString() : unitName,
                            Amount = unit.Stacks
                        });
                    }
                }

                if (entity.TryGetBuffer<RecipeLinkBuffer>(out var linkBuffer) && linkBuffer.Length > 0)
                {
                    entry.RecipeLinks = new List<string>(linkBuffer.Length);
                    for (int i = 0; i < linkBuffer.Length; i++)
                    {
                        var link = linkBuffer[i];
                        PrefabNameResolver.TryResolveName(link.Guid, out string linkName);
                        entry.RecipeLinks.Add(
                            string.IsNullOrEmpty(linkName) ? link.Guid._Value.ToString() : linkName
                        );
                    }
                }

                entries[recipeName] = entry;
            }

            var data = new CookbookRecipeData { Recipes = entries };
            WriteJson(AllRecipesPath, data);
            HeartLogger.Info(LOG_SOURCE, $"all-recipes.json written with {entries.Count} entries.");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to generate all-recipes.json: {ex.Message}");
        }
        finally
        {
            // Always reset the flag — even on failure — to prevent
            // a failed generation from retrying on every subsequent boot.
            CookbookConfig.DisableGenerateAllRecipes();
        }
    }

    // ── Example file writers ──────────────────────────────────────────────────

    static void WriteExampleRecipes()
    {
        var data = new CookbookRecipeData
        {
            Recipes = new Dictionary<string, RecipeEntry>
            {
                ["Recipe_Weapon_Sword_T04_Copper_Reinforced"] = new RecipeEntry
                {
                    ChangesEnabled       = false,
                    CraftDuration        = 4f,
                    AlwaysUnlocked       = false,
                    HideInStation        = false,
                    IgnoreServerSettings = false,
                    HudSortingOrder      = 0,
                    Requirements = new List<RecipeRequirement>
                    {
                        new() { Item = "Item_Weapon_Sword_T03_Copper",     Amount = 1 },
                        new() { Item = "Item_Ingredient_Gem_Sapphire_T01", Amount = 1 },
                        new() { Item = "Item_Ingredient_Whetstone",        Amount = 3 },
                        new() { Item = "Item_Ingredient_Leather",          Amount = 1 }
                    },
                    Outputs = new List<RecipeOutput>
                    {
                        new() { Item = "Item_Weapon_Sword_T04_Copper_Reinforced", Amount = 1 }
                    },
                    UseRepairCosts = true,
                    RepairCosts = new List<RecipeRepairCost>
                    {
                        new() { Item = "Item_Ingredient_Mineral_CopperIngot", Amount = 8 },
                        new() { Item = "Item_Ingredient_Whetstone",           Amount = 8 },
                        new() { Item = "Item_Ingredient_Plank",               Amount = 8 }
                    }
                },
                ["Recipe_Armor_Boots_T01_Bone"] = new RecipeEntry
                {
                    ChangesEnabled = false,
                    CraftDuration  = 2f
                }
            }
        };

        WriteJson(ExampleRecipesPath, data);
        HeartLogger.Info(LOG_SOURCE, "Generated example-recipes.json.");
    }

    static void WriteExampleStations()
    {
        var data = new CookbookStationData
        {
            Stations = new Dictionary<string, StationEntry>
            {
                ["TM_Blacksmith_Stations_Standard"] = new StationEntry
                {
                    ChangesEnabled = false,
                    AddRecipes     = new List<string> { "Recipe_Weapon_Sword_T04_Copper_Reinforced" },
                    RemoveRecipes  = new List<string>()
                }
            }
        };

        WriteJson(ExampleStationsPath, data);
        HeartLogger.Info(LOG_SOURCE, "Generated example-stations.json.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static void WriteJson<T>(string path, T data)
    {
        try
        {
            var json = JsonSerializer.Serialize(data, _writeOptions);
            File.WriteAllText(path, json);
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to write {Path.GetFileName(path)}: {ex.Message}");
        }
    }
}