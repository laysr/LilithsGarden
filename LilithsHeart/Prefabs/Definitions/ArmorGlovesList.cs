// ============================================================
//  ArmorGlovesList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ArmorGlovesList.cs
//
//  [CHANGED] Migrated from bare PrefabGUID fields to PrefabDef records.
//            Field names match the prefab string exactly. Names sourced
//            from original comments. All nullable fields shown explicitly.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class ArmorGlovesList
{
    public static readonly PrefabDef Item_Gloves_T01_Bone = new()
    {
        Name    = "BoneguardGloves",
        Guid    = new(-2029933415),
        Prefab  = "Item_Gloves_T01_Bone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T02_BoneReinforced = new()
    {
        Name    = "PlatedBoneguardGloves",
        Guid    = new(1746537832),
        Prefab  = "Item_Gloves_T02_BoneReinforced",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T03_Cloth = new()
    {
        Name    = "NightstalkerGloves",
        Guid    = new(-1183157751),
        Prefab  = "Item_Gloves_T03_Cloth",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T04_Copper_Brute = new()
    {
        Name    = "MarauderGloves",
        Guid    = new(-258808647),
        Prefab  = "Item_Gloves_T04_Copper_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T04_Copper_Rogue = new()
    {
        Name    = "ShadewalkerGloves",
        Guid    = new(181112381),
        Prefab  = "Item_Gloves_T04_Copper_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T04_Copper_Scholar = new()
    {
        Name    = "WarlockGloves",
        Guid    = new(-399521517),
        Prefab  = "Item_Gloves_T04_Copper_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T04_Copper_Warrior = new()
    {
        Name    = "GrimRangerGloves",
        Guid    = new(-752418019),
        Prefab  = "Item_Gloves_T04_Copper_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T05_Cotton = new()
    {
        Name    = "HollowfangGloves",
        Guid    = new(-406808302),
        Prefab  = "Item_Gloves_T05_Cotton",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T06_Iron_Brute = new()
    {
        Name    = "CrimsonTemplarGloves",
        Guid    = new(-327754127),
        Prefab  = "Item_Gloves_T06_Iron_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T06_Iron_Rogue = new()
    {
        Name    = "DuskwatcherGloves",
        Guid    = new(322804535),
        Prefab  = "Item_Gloves_T06_Iron_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T06_Iron_Scholar = new()
    {
        Name    = "DarkMagusGloves",
        Guid    = new(1247389106),
        Prefab  = "Item_Gloves_T06_Iron_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T06_Iron_Warrior = new()
    {
        Name    = "BloodHunterGloves",
        Guid    = new(1067300584),
        Prefab  = "Item_Gloves_T06_Iron_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T07_Silk = new()
    {
        Name    = "DawnthorneGloves",
        Guid    = new(2055058719),
        Prefab  = "Item_Gloves_T07_Silk",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T08_DarkSilver_Brute = new()
    {
        Name    = "GrimKnightGloves",
        Guid    = new(998240678),
        Prefab  = "Item_Gloves_T08_DarkSilver_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T08_DarkSilver_Rogue = new()
    {
        Name    = "ShadowmoonGloves",
        Guid    = new(-1752332712),
        Prefab  = "Item_Gloves_T08_DarkSilver_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T08_DarkSilver_Scholar = new()
    {
        Name    = "MaleficerScholarGloves",
        Guid    = new(1508214166),
        Prefab  = "Item_Gloves_T08_DarkSilver_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T08_DarkSilver_Warrior = new()
    {
        Name    = "DreadPlateGloves",
        Guid    = new(-1296203752),
        Prefab  = "Item_Gloves_T08_DarkSilver_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── Dracula Set ───────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Gloves_T09_Dracula = new()
    {
        Name    = "DraculasGloves",
        Guid    = new(-204401621),
        Prefab  = "Item_Gloves_T09_Dracula",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T09_Dracula_Brute = new()
    {
        Name    = "DraculasGrimGloves",
        Guid    = new(1039083725),
        Prefab  = "Item_Gloves_T09_Dracula_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T09_Dracula_Rogue = new()
    {
        Name    = "DraculasShadowGloves",
        Guid    = new(-1826382550),
        Prefab  = "Item_Gloves_T09_Dracula_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T09_Dracula_Scholar = new()
    {
        Name    = "DraculasMaleficerGloves",
        Guid    = new(-1899539896),
        Prefab  = "Item_Gloves_T09_Dracula_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T09_Dracula_Warrior = new()
    {
        Name    = "DraculasDreadGloves",
        Guid    = new(1982551454),
        Prefab  = "Item_Gloves_T09_Dracula_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── DLC ───────────────────────────────────────────────────────────────────

    // DLC: Eternal Dominance Pack
    public static readonly PrefabDef Item_Gloves_T0X_BlackfangSultan = new()
    {
        Name    = "OpulentNightVambraces",
        Guid    = new(-749828559),
        Prefab  = "Item_Gloves_T0X_BlackfangSultan",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Gloves_T0X_PMK01 = new()
    {
        Name    = "AlucardGloves",
        Guid    = new(-1522497513),
        Prefab  = "Item_Gloves_T0X_PMK01",
        NameKey = null,
        DescKey = null,
    };

    // ── Unused ────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Gloves_T00_StartingRags = new()
    {
        Name    = null,
        Guid    = new(1216450741),
        Prefab  = "Item_Gloves_T00_StartingRags",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Armor_Gloves_Base = new()
    {
        Name    = null,
        Guid    = new(-786493143),
        Prefab  = "Item_Armor_Gloves_Base",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Gloves_T0X_VampireKnight = new()
    {
        Name    = null,
        Guid    = new(-745793193),
        Prefab  = "Item_Gloves_T0X_VampireKnight",
        NameKey = null,
        DescKey = null,
    };
}