// ============================================================
//  StationsIndex — LilithsMind
//  LilithsMind/Prefabs/Definitions/StationsIndex.cs
//
//  [CHANGED] Migrated from bare PrefabGUID + [PrefabName] attribute fields
//            to PrefabDef records. Field names match the prefab string exactly.
//            Names sourced from [PrefabName] attributes; null where absent
//            (special/unused stations). All nullable fields shown explicitly.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

namespace LilithsMind.Prefabs.Definitions;

public static class StationsIndex
{
    // ── Crafting Stations ─────────────────────────────────────────────────────

    public static readonly PrefabDef CraftingStation_Player = new()
    {
        Name    = "PlayerCrafting",
        GuidHash = 1420623103,
    };

    public static readonly PrefabDef TM_CraftingStation_SimpleCraftingBench = new()
    {
        Name    = "SimpleWorkbench",
        GuidHash = -1107784271,
        Prefab  = "TM_CraftingStation_SimpleCraftingBench",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_WoodworkingBench = new()
    {
        Name    = "WoodworkingBench",
        GuidHash = -332123372,
        Prefab  = "TM_CraftingStation_WoodworkingBench",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_Leatherworking = new()
    {
        Name    = "LeatherworkingStation",
        GuidHash = 1779320855,
        Prefab  = "TM_CraftingStation_Leatherworking",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_ArtisanTable = new()
    {
        Name    = "ArtisanTable",
        GuidHash = -1718710437,
        Prefab  = "TM_CraftingStation_ArtisanTable",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_JewelcraftingTable = new()
    {
        Name    = "JewelcraftingTable",
        GuidHash = 508953830,
        Prefab  = "TM_CraftingStation_JewelcraftingTable",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_TailorBench = new()
    {
        Name    = "TailoringBench",
        GuidHash = -952755594,
        Prefab  = "TM_CraftingStation_TailorBench",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_AlchemyLab_Small = new()
    {
        Name    = "AlchemyTable",
        GuidHash = 181938440,
        Prefab  = "TM_CraftingStation_AlchemyLab_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_Smithy = new()
    {
        Name    = "Smithy",
        GuidHash = -1840926436,
        Prefab  = "TM_CraftingStation_Smithy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_Anvil = new()
    {
        Name    = "Anvil",
        GuidHash = -437790980,
        Prefab  = "TM_CraftingStation_Anvil",
        NameKey = null,
        DescKey = null,
    };

    // ── Refinement Stations ───────────────────────────────────────────────────

    public static readonly PrefabDef TM_RefinementStation_Sawmill_Small = new()
    {
        Name    = "Sawmill",
        GuidHash = 1094077710,
        Prefab  = "TM_RefinementStation_Sawmill_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Sawmill_Large = new()
    {
        Name    = "AdvancedSawmill",
        GuidHash = -163562336,
        Prefab  = "TM_RefinementStation_Sawmill_Large",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_StonecuttingTable_Small = new()
    {
        Name    = "Grinder",
        GuidHash = -600683642,
        Prefab  = "TM_RefinementStation_StonecuttingTable_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_StonecuttingTable_Large = new()
    {
        Name    = "AdvancedGrinder",
        GuidHash = -178579946,
        Prefab  = "TM_RefinementStation_StonecuttingTable_Large",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Furnace_Small = new()
    {
        Name    = "Furnace",
        GuidHash = -1150411622,
        Prefab  = "TM_RefinementStation_Furnace_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Furnace_Large = new()
    {
        Name    = "AdvancedFurnace",
        GuidHash = -222851985,
        Prefab  = "TM_RefinementStation_Furnace_Large",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_BloodPress_Small = new()
    {
        Name    = "BloodPress",
        GuidHash = -300823465,
        Prefab  = "TM_RefinementStation_BloodPress_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_BloodPress_Large = new()
    {
        Name    = "AdvancedBloodPress",
        GuidHash = -684391635,
        Prefab  = "TM_RefinementStation_BloodPress_Large",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Tannery_Small = new()
    {
        Name    = "Tannery",
        GuidHash = -635885386,
        Prefab  = "TM_RefinementStation_Tannery_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Tannery_Large = new()
    {
        Name    = "AdvancedTannery",
        GuidHash = -1422196107,
        Prefab  = "TM_RefinementStation_Tannery_Large",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Loom_Small = new()
    {
        Name    = "Loom",
        GuidHash = -16328955,
        Prefab  = "TM_RefinementStation_Loom_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Loom_Large = new()
    {
        Name    = "AdvancedLoom",
        GuidHash = 1299929048,
        Prefab  = "TM_RefinementStation_Loom_Large",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_PaperPress_Small = new()
    {
        Name    = "PaperPress",
        GuidHash = -1628971842,
        Prefab  = "TM_RefinementStation_PaperPress_Small",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_GemCutting = new()
    {
        Name    = "GemCuttingTable",
        GuidHash = -21483617,
        Prefab  = "TM_RefinementStation_GemCutting",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_Fabricator = new()
    {
        Name    = "Fabricator",
        GuidHash = -465055967,
        Prefab  = "TM_RefinementStation_Fabricator",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_SalvageStation_Table = new()
    {
        Name    = "Devourer",
        GuidHash = -1719849142,
        Prefab  = "TM_SalvageStation_Table",
        NameKey = null,
        DescKey = null,
    };

    // ── Unit Stations ─────────────────────────────────────────────────────────

    public static readonly PrefabDef TM_UnitStation_VerminNest = new()
    {
        Name    = "VerminNest",
        GuidHash = 150776081,
        Prefab  = "TM_UnitStation_VerminNest",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_UnitStation_Tomb = new()
    {
        Name    = "Tomb",
        GuidHash = 1127059420,
        Prefab  = "TM_UnitStation_Tomb",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_UnitStation_NetherGate = new()
    {
        Name    = "StygianSummoningCircle",
        GuidHash = -218354895,
        Prefab  = "TM_UnitStation_NetherGate",
        NameKey = null,
        DescKey = null,
    };

    // ── Special Stations — Do Not Use ─────────────────────────────────────────

    public static readonly PrefabDef TM_CraftingStation_AncestralForge = new()
    {
        Name    = null,
        GuidHash = 48521126,
        Prefab  = "TM_CraftingStation_AncestralForge",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_BloodMixer = new()
    {
        Name    = null,
        GuidHash = -969931747,
        Prefab  = "TM_CraftingStation_BloodMixer",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_FusionForge = new()
    {
        Name    = null,
        GuidHash = -1286344051,
        Prefab  = "TM_CraftingStation_FusionForge",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_Stables = new()
    {
        Name    = null,
        GuidHash = 472278220,
        Prefab  = "TM_CraftingStation_Stables",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_RefinementStation_TeslaLightningRod = new()
    {
        Name    = null,
        GuidHash = 1311814093,
        Prefab  = "TM_RefinementStation_TeslaLightningRod",
        NameKey = null,
        DescKey = null,
    };

    // ── Unused — Do Not Use ───────────────────────────────────────────────────

    public static readonly PrefabDef TM_CraftingStation_Altar_Frost = new()
    {
        Name    = null,
        GuidHash = -609878016,
        Prefab  = "TM_CraftingStation_Altar_Frost",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_Altar_Spectral = new()
    {
        Name    = null,
        GuidHash = -64110296,
        Prefab  = "TM_CraftingStation_Altar_Spectral",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_Altar_Unholy = new()
    {
        Name    = null,
        GuidHash = -676962218,
        Prefab  = "TM_CraftingStation_Altar_Unholy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_ArtisansCorner = new()
    {
        Name    = null,
        GuidHash = 1121480632,
        Prefab  = "TM_CraftingStation_ArtisansCorner",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_BloodBank = new()
    {
        Name    = null,
        GuidHash = -452732692,
        Prefab  = "TM_CraftingStation_BloodBank",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef TM_CraftingStation_MetalworkStation = new()
    {
        Name    = null,
        GuidHash = 2014944075,
        Prefab  = "TM_CraftingStation_MetalworkStation",
        NameKey = null,
        DescKey = null,
    };
}