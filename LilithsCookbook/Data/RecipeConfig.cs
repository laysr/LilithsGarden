namespace LilithsCookbook.Data;

public class RecipeConfig
{
    public Dictionary<string, RecipeEntry> Recipes { get; set; } = new();
}

public class RecipeEntry
{
    public bool Enabled { get; set; } = false;
    public float? CraftDuration { get; set; }
    public bool? AlwaysUnlocked { get; set; }
    public bool? HideInStation { get; set; }
    public bool? IgnoreServerSettings { get; set; }
    public int? HudSortingOrder { get; set; }
    public List<RecipeRequirement>? Requirements { get; set; }
    public List<RecipeOutput>? Outputs { get; set; }
    public List<RecipeRepairCost>? RepairCosts { get; set; }
    public List<RecipeUnitOutput>? UnitOutputs { get; set; }
}

public class RecipeRequirement
{
    public string Item { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class RecipeOutput
{
    public string Item { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class RecipeRepairCost
{
    public string Item { get; set; } = string.Empty;
    public int Amount { get; set; }
}

public class RecipeUnitOutput
{
    public string Unit { get; set; } = string.Empty;
    public int Amount { get; set; }
}