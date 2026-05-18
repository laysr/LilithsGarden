using System.Text.Json;
using System.Text.Json.Serialization;
using ProjectM;
using LilithsHeart;
using LilithsHeart.Systems;
using LilithsCookbook.Data;

namespace LilithsCookbook.Systems;

public static class CookbookGenerator
{
    private const string LOG_SOURCE = "LilithsCookbook.CookbookGenerator";

    // ── Path constants (shared with CookbookLoader via CookbookPlugin) ────────

    public static readonly string RootDir = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsHeart",
        "LilithsCookbook"
    );

    public static readonly string RecipesDir  = Path.Combine(RootDir, "Recipes");
    public static readonly string StationsDir = Path.Combine(RootDir, "Stations");

    static readonly string ExampleRecipesPath  = Path.Combine(RecipesDir,  "example-recipes.json");
    static readonly string ExampleStationsPath = Path.Combine(StationsDir, "example-stations.json");
    static readonly string AllRecipesPath      = Path.Combine(RecipesDir,  "all-recipes.json");

    static readonly JsonSerializerOptions _writeOptions = new()
    {
        WriteIndented = true,
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
    /// If GenerateAllRecipes is enabled in CookbookConfig, iterates all recipe
    /// prefab entities, serializes their current vanilla state to all-recipes.json,
    /// then disables the setting so it does not run again on the next boot.
    /// </summary>
    public static void GenerateAllRecipesIfRequested()
    {
        if (!CookbookConfig.GenerateAllRecipes)
            return;

        LilithsLogger.Info(LOG_SOURCE, "GenerateAllRecipes is enabled — generating all-recipes.json...");

        try
        {
            var entries   = new Dictionary<string, RecipeEntry>();
            var prefabMap = Heart.PrefabCollectionSystem._PrefabGuidToEntityMap;

            foreach (var kvp in prefabMap)
            {
                var guid   = kvp.Key;
                var entity = kvp.Value;

                if (!entity.TryGetComponent<RecipeData>(out var recipeData))
                    continue;

                // Prefer human-readable name; fall back to raw GUID integer.
                if (!PrefabNameResolver.TryResolveName(guid, out string recipeName))
                    recipeName = guid._Value.ToString();

                var entry = new RecipeEntry
                {
                    ChangesEnabled       = false,
                    CraftDuration        = recipeData.CraftDuration,
                    AlwaysUnlocked       = recipeData.AlwaysUnlocked,
                    HideInStation        = recipeData.HideInStation,
                    IgnoreServerSettings = recipeData.IgnoreServerSettings,
                    HudSortingOrder      = recipeData.HudSortingOrder
                };

                // Requirements — always present on valid recipe entities.
                if (entity.TryGetBuffer<RecipeRequirementBuffer>(out var reqBuffer))
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

                // Outputs — always present on valid recipe entities.
                if (entity.TryGetBuffer<RecipeOutputBuffer>(out var outBuffer))
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

                // Repair costs — only on equipment recipes. Skip if buffer is absent or empty.
                if (entity.TryGetBuffer<ItemRepairBuffer>(out var repairBuffer) && repairBuffer.Length > 0)
                {
                    entry.RepairCosts = new List<RecipeRepairCost>(repairBuffer.Length);
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

                // Unit outputs — only on unit-spawning recipes. Skip if buffer is absent or empty.
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

                // Recipe links — only on grouped recipes. Skip if buffer is absent or empty.
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
            LilithsLogger.Info(LOG_SOURCE, $"all-recipes.json written with {entries.Count} entries.");
        }
        catch (Exception ex)
        {
            LilithsLogger.Error(LOG_SOURCE, $"Failed to generate all-recipes.json: {ex.Message}");
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
        // Recipe_Weapon_Sword_T04_Copper_Reinforced is used as the primary example
        // since we have its full prefab dump. A second minimal entry demonstrates
        // that omitted fields are simply left at their vanilla values.
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
                    // Only fields you specify are applied — omitted fields keep their vanilla values.
                    // Set UseRepairCosts = false to strip repair costs entirely.
                    // Set UseUnitOutputs = false to strip unit outputs entirely.
                    // Set UseRecipeLinks = false to strip recipe links entirely.
                }
            }
        };

        WriteJson(ExampleRecipesPath, data);
        LilithsLogger.Info(LOG_SOURCE, "Generated example-recipes.json.");
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
        LilithsLogger.Info(LOG_SOURCE, "Generated example-stations.json.");
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
            LilithsLogger.Error(LOG_SOURCE, $"Failed to write {Path.GetFileName(path)}: {ex.Message}");
        }
    }
}