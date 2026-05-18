namespace LilithsCookbook.Data;

public class CookbookRecipeData
{
    public Dictionary<string, RecipeEntry> Recipes { get; set; } = new();
}

public class RecipeEntry
{
    public bool ChangesEnabled { get; set; } = false;
    public float? CraftDuration { get; set; }
    public bool? AlwaysUnlocked { get; set; }
    public bool? HideInStation { get; set; }
    public bool? IgnoreServerSettings { get; set; }
    public int? HudSortingOrder { get; set; }
    public List<RecipeRequirement>? Requirements { get; set; }
    public List<RecipeOutput>? Outputs { get; set; }

    // Optional buffer control flags:
    //   null  — not specified, buffer is left untouched
    //   false — remove the buffer entirely
    //   true  — ensure the buffer exists and apply the list below
    public bool? UseRepairCosts { get; set; }
    public List<RecipeRepairCost>? RepairCosts { get; set; }

    public bool? UseUnitOutputs { get; set; }
    public List<RecipeUnitOutput>? UnitOutputs { get; set; }

    public bool? UseRecipeLinks { get; set; }
    public List<string>? RecipeLinks { get; set; }
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
