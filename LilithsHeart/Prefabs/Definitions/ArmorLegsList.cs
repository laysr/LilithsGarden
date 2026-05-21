// ============================================================
//  ArmorLegsList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ArmorLegsList.cs
//
//  [CHANGED] Migrated from bare PrefabGUID + [PrefabName] attribute fields
//            to PrefabDef records. Field names match the prefab string
//            exactly. Names sourced from [PrefabName] attributes and
//            comments where no attribute was present. All nullable fields
//            shown explicitly.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class ArmorLegsList
{
    public static readonly PrefabDef Item_Legs_T01_Bone = new()
    {
        Name    = "BoneguardLeggings",
        Guid    = new(1355823667),
        Prefab  = "Item_Legs_T01_Bone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T02_BoneReinforced = new()
    {
        Name    = "PlatedBoneguardLeggings",
        Guid    = new(-2036196416),
        Prefab  = "Item_Legs_T02_BoneReinforced",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T03_Cloth = new()
    {
        Name    = "NightstalkerLeggings",
        Guid    = new(1925394440),
        Prefab  = "Item_Legs_T03_Cloth",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T04_Copper_Brute = new()
    {
        Name    = "MarauderLeggings",
        Guid    = new(-2095610608),
        Prefab  = "Item_Legs_T04_Copper_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T04_Copper_Rogue = new()
    {
        Name    = "ShadewalkerLeggings",
        Guid    = new(-90702575),
        Prefab  = "Item_Legs_T04_Copper_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T04_Copper_Scholar = new()
    {
        Name    = "WarlockLeggings",
        Guid    = new(1458279255),
        Prefab  = "Item_Legs_T04_Copper_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T04_Copper_Warrior = new()
    {
        Name    = "GrimRangerLeggings",
        Guid    = new(-1993947781),
        Prefab  = "Item_Legs_T04_Copper_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T05_Cotton = new()
    {
        Name    = "HollowfangLeggings",
        Guid    = new(12127911),
        Prefab  = "Item_Legs_T05_Cotton",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T06_Iron_Brute = new()
    {
        Name    = "CrimsonTemplarLeggings",
        Guid    = new(680112231),
        Prefab  = "Item_Legs_T06_Iron_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T06_Iron_Rogue = new()
    {
        Name    = "DuskwatcherLeggings",
        Guid    = new(744344540),
        Prefab  = "Item_Legs_T06_Iron_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T06_Iron_Scholar = new()
    {
        Name    = "DarkMagusLeggings",
        Guid    = new(-454576348),
        Prefab  = "Item_Legs_T06_Iron_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T06_Iron_Warrior = new()
    {
        Name    = "BloodHunterLeggings",
        Guid    = new(206495029),
        Prefab  = "Item_Legs_T06_Iron_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T07_Silk = new()
    {
        Name    = "DawnthornLeggings",
        Guid    = new(-1555051415),
        Prefab  = "Item_Legs_T07_Silk",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T08_DarkSilver_Brute = new()
    {
        Name    = "GrimKnightLeggings",
        Guid    = new(-1385786654),
        Prefab  = "Item_Legs_T08_DarkSilver_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T08_DarkSilver_Rogue = new()
    {
        Name    = "ShadowmoonLeggings",
        Guid    = new(-262114802),
        Prefab  = "Item_Legs_T08_DarkSilver_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T08_DarkSilver_Scholar = new()
    {
        Name    = "MaleficerLeggings",
        Guid    = new(703230071),
        Prefab  = "Item_Legs_T08_DarkSilver_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T08_DarkSilver_Warrior = new()
    {
        Name    = "DreadPlateLeggings",
        Guid    = new(-481041545),
        Prefab  = "Item_Legs_T08_DarkSilver_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── Dracula Set ───────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Legs_T09_Dracula = new()
    {
        Name    = "DraculasLeggings",
        Guid    = new(125611165),
        Prefab  = "Item_Legs_T09_Dracula",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T09_Dracula_Brute = new()
    {
        Name    = "DraculasGrimLeggings",
        Guid    = new(993033515),
        Prefab  = "Item_Legs_T09_Dracula_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T09_Dracula_Rogue = new()
    {
        Name    = "DraculasShadowLeggings",
        Guid    = new(-345596442),
        Prefab  = "Item_Legs_T09_Dracula_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T09_Dracula_Scholar = new()
    {
        Name    = "DraculasMaleficerLeggings",
        Guid    = new(1592149279),
        Prefab  = "Item_Legs_T09_Dracula_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T09_Dracula_Warrior = new()
    {
        Name    = "DraculasDreadLeggings",
        Guid    = new(205207385),
        Prefab  = "Item_Legs_T09_Dracula_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── Cosmetics ─────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Legs_T0X_Cosmetic_Suit01 = new()
    {
        Name    = "MidnightNoblermanPants",
        Guid    = new(213736942),
        Prefab  = "Item_Legs_T0X_Cosmetic_Suit01",
        NameKey = null,
        DescKey = null,
    };

    // ── DLC ───────────────────────────────────────────────────────────────────

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Legs_T0X_PMK01 = new()
    {
        Name    = "AlucardLeggings",
        Guid    = new(-1564372276),
        Prefab  = "Item_Legs_T0X_PMK01",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Legs_T0X_PMK02 = new()
    {
        Name    = "ShanoasLeggings",
        Guid    = new(-536717606),
        Prefab  = "Item_Legs_T0X_PMK02",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Legs_T0X_PMK03 = new()
    {
        Name    = "SomaCruzsLeggings",
        Guid    = new(1811913705),
        Prefab  = "Item_Legs_T0X_PMK03",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Eternal Dominance Pack
    public static readonly PrefabDef Item_Legs_T0X_BlackfangSultan = new()
    {
        Name    = "OpulentNightLeggings",
        Guid    = new(-1558814807),
        Prefab  = "Item_Legs_T0X_BlackfangSultan",
        NameKey = null,
        DescKey = null,
    };

    // ── Unused ────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Armor_Legs_Base = new()
    {
        Name    = null,
        Guid    = new(269771183),
        Prefab  = "Item_Armor_Legs_Base",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T00_StartingRags = new()
    {
        Name    = null,
        Guid    = new(725607617),
        Prefab  = "Item_Legs_T00_StartingRags",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T0X_VampireKnight = new()
    {
        Name    = null,
        Guid    = new(1966590385),
        Prefab  = "Item_Legs_T0X_VampireKnight",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Legs_T0X_TransmogTest = new()
    {
        Name    = null,
        Guid    = new(1217578824),
        Prefab  = "Item_Legs_T0X_TransmogTest",
        NameKey = null,
        DescKey = null,
    };
}