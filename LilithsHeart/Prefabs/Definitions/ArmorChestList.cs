// ============================================================
//  ArmorChestList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ArmorChestList.cs
//
//  [CHANGED] Migrated from bare PrefabGUID + [PrefabName] attribute fields
//            to PrefabDef records. Field names match the prefab string
//            exactly. Names sourced from [PrefabName] attributes. All
//            nullable fields shown explicitly.
//
//  NOTE: Source file was named "ArorChestList" — corrected to "ArmorChestList"
//        here. Rename the source file to match if not already done.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class ArmorChestList
{
    public static readonly PrefabDef Item_Chest_T01_Bone = new()
    {
        Name    = "BoneguardChestguard",
        Guid    = new(329301090),
        Prefab  = "Item_Chest_T01_Bone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T02_BoneReinforced = new()
    {
        Name    = "PlatedBoneguardChestguard",
        Guid    = new(-958936382),
        Prefab  = "Item_Chest_T02_BoneReinforced",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T03_Cloth = new()
    {
        Name    = "NightstalkerVest",
        Guid    = new(-957963240),
        Prefab  = "Item_Chest_T03_Cloth",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T04_Copper_Brute = new()
    {
        Name    = "MarauderVest",
        Guid    = new(-112921782),
        Prefab  = "Item_Chest_T04_Copper_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T04_Copper_Rogue = new()
    {
        Name    = "ShadewalkerVest",
        Guid    = new(763326246),
        Prefab  = "Item_Chest_T04_Copper_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T04_Copper_Scholar = new()
    {
        Name    = "WarlockVest",
        Guid    = new(-2100321922),
        Prefab  = "Item_Chest_T04_Copper_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T04_Copper_Warrior = new()
    {
        Name    = "GrimRangerVest",
        Guid    = new(1809631067),
        Prefab  = "Item_Chest_T04_Copper_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T05_Cotton = new()
    {
        Name    = "HollowfangChestguard",
        Guid    = new(-604941435),
        Prefab  = "Item_Chest_T05_Cotton",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T06_Iron_Brute = new()
    {
        Name    = "CrimsonTemplarChestguard",
        Guid    = new(-1641042717),
        Prefab  = "Item_Chest_T06_Iron_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T06_Iron_Rogue = new()
    {
        Name    = "DuskwatcherChestguard",
        Guid    = new(-69916288),
        Prefab  = "Item_Chest_T06_Iron_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T06_Iron_Scholar = new()
    {
        Name    = "DarkMagusChestguard",
        Guid    = new(-2127687996),
        Prefab  = "Item_Chest_T06_Iron_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T06_Iron_Warrior = new()
    {
        Name    = "BloodHunterChestguard",
        Guid    = new(-2102875089),
        Prefab  = "Item_Chest_T06_Iron_Warrior",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T07_Silk = new()
    {
        Name    = "DawnthornChestguard",
        Guid    = new(-930514044),
        Prefab  = "Item_Chest_T07_Silk",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T08_DarkSilver_Brute = new()
    {
        Name    = "GrimKnightChestguard",
        Guid    = new(-1279475298),
        Prefab  = "Item_Chest_T08_DarkSilver_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T08_DarkSilver_Rogue = new()
    {
        Name    = "ShadowmoonChestguard",
        Guid    = new(1871735757),
        Prefab  = "Item_Chest_T08_DarkSilver_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T08_DarkSilver_Scholar = new()
    {
        Name    = "MaleficerChestguard",
        Guid    = new(-919709436),
        Prefab  = "Item_Chest_T08_DarkSilver_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T08_DarkSilver_Warrior = new()
    {
        Name    = "DreadPlateChestguard",
        Guid    = new(750788905),
        Prefab  = "Item_Chest_T08_DarkSilver_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── Dracula Set ───────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Chest_T09_Dracula = new()
    {
        Name    = "DraculasChestguard",
        Guid    = new(1055898174),
        Prefab  = "Item_Chest_T09_Dracula",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T09_Dracula_Brute = new()
    {
        Name    = "DraculasGrimChestguard",
        Guid    = new(1033753207),
        Prefab  = "Item_Chest_T09_Dracula_Brute",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T09_Dracula_Rogue = new()
    {
        Name    = "DraculasShadowChestguard",
        Guid    = new(933057100),
        Prefab  = "Item_Chest_T09_Dracula_Rogue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T09_Dracula_Scholar = new()
    {
        Name    = "DraculasMaleficerChestguard",
        Guid    = new(114259912),
        Prefab  = "Item_Chest_T09_Dracula_Scholar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T09_Dracula_Warrior = new()
    {
        Name    = "DraculasDreadChestguard",
        Guid    = new(1392314162),
        Prefab  = "Item_Chest_T09_Dracula_Warrior",
        NameKey = null,
        DescKey = null,
    };

    // ── Cosmetics ─────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Chest_T0X_Cosmetic_Dress01 = new()
    {
        Name    = "MidnightBallGown",
        Guid    = new(-511360389),
        Prefab  = "Item_Chest_T0X_Cosmetic_Dress01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T0X_Cosmetic_Suit01 = new()
    {
        Name    = "MidnightNoblermanSuit",
        Guid    = new(538326235),
        Prefab  = "Item_Chest_T0X_Cosmetic_Suit01",
        NameKey = null,
        DescKey = null,
    };

    // ── DLC ───────────────────────────────────────────────────────────────────

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Chest_T0X_PMK01 = new()
    {
        Name    = "AlucardCoat",
        Guid    = new(1712262077),
        Prefab  = "Item_Chest_T0X_PMK01",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Chest_T0X_PMK02 = new()
    {
        Name    = "ShanoasGown",
        Guid    = new(896678280),
        Prefab  = "Item_Chest_T0X_PMK02",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Chest_T0X_PMK03 = new()
    {
        Name    = "SomaCruzsCoat",
        Guid    = new(-1349059251),
        Prefab  = "Item_Chest_T0X_PMK03",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Eternal Dominance Pack
    public static readonly PrefabDef Item_Chest_T0X_BlackfangSultan = new()
    {
        Name    = "OpulentNightRaiment",
        Guid    = new(-247737453),
        Prefab  = "Item_Chest_T0X_BlackfangSultan",
        NameKey = null,
        DescKey = null,
    };

    // ── Unused ────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Armor_Chest_Base = new()
    {
        Name    = null,
        Guid    = new(1328680870),
        Prefab  = "Item_Armor_Chest_Base",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T00_StartingRags = new()
    {
        Name    = null,
        Guid    = new(-1723445833),
        Prefab  = "Item_Chest_T00_StartingRags",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T0X_TransmogTest = new()
    {
        Name    = null,
        Guid    = new(-625033436),
        Prefab  = "Item_Chest_T0X_TransmogTest",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Chest_T0X_VampireKnight = new()
    {
        Name    = null,
        Guid    = new(1953885108),
        Prefab  = "Item_Chest_T0X_VampireKnight",
        NameKey = null,
        DescKey = null,
    };
}