// ============================================================
//  ArmorBootsList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ArmorBootsList.cs
//
//  [CHANGED] Migrated from bare PrefabGUID fields to PrefabDef records.
//            Field names match the prefab string exactly. Names sourced
//            from original comments and [PrefabName] attributes where
//            present. All nullable fields shown explicitly.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class ArmorBootsList
{
    public static readonly PrefabDef Item_Boots_T01_Bone = new()
    {
        Name    = "BoneguardBoots",
        Guid    = new(711062517),
        Prefab  = "Item_Boots_T01_Bone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T02_BoneReinforced = new()
    {
        Name    = "PlatedBoneguardBoots",
        Guid    = new(1241831522),
        Prefab  = "Item_Boots_T02_BoneReinforced",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T03_Cloth = new()
    {
        Name    = "NightstalkerBoots",
        Guid    = new(-1354920908),
        Prefab  = "Item_Boots_T03_Cloth",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T04_Copper_Brute = new()
    {
        Name    = "MarauderBoots",
        Guid    = new(-1359494169),
        Prefab  = "Item_Boots_T04_Copper_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T04_Copper_Rogue = new()
    {
        Name    = "ShadewalkerBoots",
        Guid    = new(1101206623),
        Prefab  = "Item_Boots_T04_Copper_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T04_Copper_Scholar = new()
    {
        Name    = "WarlockBoots",
        Guid    = new(1204352435),
        Prefab  = "Item_Boots_T04_Copper_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T04_Copper_Warrior = new()
    {
        Name    = "GrimRangerBoots",
        Guid    = new(-15390086),
        Prefab  = "Item_Boots_T04_Copper_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T05_Cotton = new()
    {
        Name    = "HollowfangBoots",
        Guid    = new(-1837769884),
        Prefab  = "Item_Boots_T05_Cotton",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T06_Iron_Brute = new()
    {
        Name    = "CrimsonTemplarBoots",
        Guid    = new(-1329744719),
        Prefab  = "Item_Boots_T06_Iron_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T06_Iron_Rogue = new()
    {
        Name    = "DuskwatcherBoots",
        Guid    = new(51576788),
        Prefab  = "Item_Boots_T06_Iron_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T06_Iron_Scholar = new()
    {
        Name    = "DarkMagusBoots",
        Guid    = new(138060378),
        Prefab  = "Item_Boots_T06_Iron_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T06_Iron_Warrior = new()
    {
        Name    = "BloodHunterBoots",
        Guid    = new(666433583),
        Prefab  = "Item_Boots_T06_Iron_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T07_Silk = new()
    {
        Name    = "DawnthorneBoots",
        Guid    = new(560446510),
        Prefab  = "Item_Boots_T07_Silk",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T08_DarkSilver_Brute = new()
    {
        Name    = "GrimKnightBoots",
        Guid    = new(-1023762087),
        Prefab  = "Item_Boots_T08_DarkSilver_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T08_DarkSilver_Rogue = new()
    {
        Name    = "ShadowmoonBoots",
        Guid    = new(-1921018689),
        Prefab  = "Item_Boots_T08_DarkSilver_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T08_DarkSilver_Scholar = new()
    {
        Name    = "MaleficerScholarBoots",
        Guid    = new(1469185034),
        Prefab  = "Item_Boots_T08_DarkSilver_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T08_DarkSilver_Warrior = new()
    {
        Name    = "DreadPlateBoots",
        Guid    = new(1395895315),
        Prefab  = "Item_Boots_T08_DarkSilver_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── Dracula Set ───────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Boots_T09_Dracula = new()
    {
        Name    = "DraculasBoots",
        Guid    = new(1400688919),
        Prefab  = "Item_Boots_T09_Dracula",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T09_Dracula_Brute = new()
    {
        Name    = "DraculasGrimBoots",
        Guid    = new(1646489863),
        Prefab  = "Item_Boots_T09_Dracula_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T09_Dracula_Rogue = new()
    {
        Name    = "DraculasShadowBoots",
        Guid    = new(1855323424),
        Prefab  = "Item_Boots_T09_Dracula_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T09_Dracula_Scholar = new()
    {
        Name    = "DraculasMaleficerBoots",
        Guid    = new(1531721602),
        Prefab  = "Item_Boots_T09_Dracula_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T09_Dracula_Warrior = new()
    {
        Name    = "DraculasDreadBoots",
        Guid    = new(-382349289),
        Prefab  = "Item_Boots_T09_Dracula_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── DLC ───────────────────────────────────────────────────────────────────

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Boots_T0X_PMK01 = new()
    {
        Name    = "AlucardBoots",
        Guid    = new(-1214309698),
        Prefab  = "Item_Boots_T0X_PMK01",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Boots_T0X_PMK03 = new()
    {
        Name    = "SomaCruzsBoots",
        Guid    = new(-14368032),
        Prefab  = "Item_Boots_T0X_PMK03",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Eternal Dominance Pack
    public static readonly PrefabDef Item_Boots_T0X_BlackfangSultan = new()
    {
        Name    = "OpulentNightSabatons",
        Guid    = new(540608060),
        Prefab  = "Item_Boots_T0X_BlackfangSultan",
        NameKey = null,
        DescKey = null,
    };

    // ── Unused ────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Boots_T00_StartingRags = new()
    {
        Name    = null,
        Guid    = new(-2137364987),
        Prefab  = "Item_Boots_T00_StartingRags",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Boots_T0X_VampireKnight = new()
    {
        Name    = null,
        Guid    = new(830032282),
        Prefab  = "Item_Boots_T0X_VampireKnight",
        NameKey = null,
        DescKey = null,
    };
}