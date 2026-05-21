// ============================================================
//  ArmorCloakList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ArmorCloakList.cs
//
//  [CHANGED] Migrated from bare PrefabGUID + [PrefabName] attribute fields
//            to PrefabDef records. Field names match the prefab string
//            exactly. Names sourced from [PrefabName] attributes. All
//            nullable fields shown explicitly.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class ArmorCloakList
{
    // ── Main Cloaks ───────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Cloak_Main_ShroudOfTheForest = new()
    {
        Name    = "ShroudOfTheForest",
        Guid    = new(1063517722),
        Prefab  = "Item_Cloak_Main_ShroudOfTheForest",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_Main_T01_Travelers = new()
    {
        Name    = "TravelersWrap",
        Guid    = new(-1819786494),
        Prefab  = "Item_Cloak_Main_T01_Travelers",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_Main_T02_Hunter = new()
    {
        Name    = "HuntersCloak",
        Guid    = new(786585343),
        Prefab  = "Item_Cloak_Main_T02_Hunter",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_Main_T03_Phantom = new()
    {
        Name    = "PhantomsVeil",
        Guid    = new(-227965303),
        Prefab  = "Item_Cloak_Main_T03_Phantom",
        NameKey = null,
        DescKey = null,
    };

    // ── T01 Cloaks ────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Cloak_T01_ChaosArcher = new()
    {
        Name    = "ChaosStitchedDrape",
        Guid    = new(-168044197),
        Prefab  = "Item_Cloak_T01_ChaosArcher",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T01_FrostArrow = new()
    {
        Name    = "GlacialStitchedDrape",
        Guid    = new(-24337506),
        Prefab  = "Item_Cloak_T01_FrostArrow",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T01_Jade = new()
    {
        Name    = "DarkLeatherDrape",
        Guid    = new(1261174372),
        Prefab  = "Item_Cloak_T01_Jade",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T01_Razer = new()
    {
        Name    = "RazerSerpentWrap",
        Guid    = new(-766642494),
        Prefab  = "Item_Cloak_T01_Razer",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T01_UndeadMage = new()
    {
        Name    = "TattersOfTheUndead",
        Guid    = new(-305440546),
        Prefab  = "Item_Cloak_T01_UndeadMage",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T01_VampireHunter = new()
    {
        Name    = "VampireHuntersDrape",
        Guid    = new(1335546377),
        Prefab  = "Item_Cloak_T01_VampireHunter",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T01_ZealousCultist = new()
    {
        Name    = "BloodstainedCloth",
        Guid    = new(150621304),
        Prefab  = "Item_Cloak_T01_ZealousCultist",
        NameKey = null,
        DescKey = null,
    };

    // ── T02 Cloaks ────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Cloak_T02_Cardinal = new()
    {
        Name    = "CardinalsCloak",
        Guid    = new(-1768698241),
        Prefab  = "Item_Cloak_T02_Cardinal",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_HarpyMatriarch = new()
    {
        Name    = "PurpleFeatheredCape",
        Guid    = new(1677983904),
        Prefab  = "Item_Cloak_T02_HarpyMatriarch",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_HolyPaladin = new()
    {
        Name    = "ManaShawl",
        Guid    = new(-2091288477),
        Prefab  = "Item_Cloak_T02_HolyPaladin",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_MilitiaMonk = new()
    {
        Name    = "HermitsShawl",
        Guid    = new(2147390246),
        Prefab  = "Item_Cloak_T02_MilitiaMonk",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_PatchedCloak = new()
    {
        Name    = "ThousandStitchCloak",
        Guid    = new(1275572025),
        Prefab  = "Item_Cloak_T02_PatchedCloak",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_Poloma = new()
    {
        Name    = "TokiFeatheredCape",
        Guid    = new(-589858836),
        Prefab  = "Item_Cloak_T02_Poloma",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_Razer = new()
    {
        Name    = "RazerSerpentCloak",
        Guid    = new(1410262258),
        Prefab  = "Item_Cloak_T02_Razer",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_Tailor = new()
    {
        Name    = "BeatricesScarf",
        Guid    = new(-2081646636),
        Prefab  = "Item_Cloak_T02_Tailor",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_TornRags = new()
    {
        Name    = "WarTornWineCloak",
        Guid    = new(707710831),
        Prefab  = "Item_Cloak_T02_TornRags",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_WildlingBlue = new()
    {
        Name    = "AshfolkCrystalIceCloak",
        Guid    = new(239338934),
        Prefab  = "Item_Cloak_T02_WildlingBlue",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T02_WildlingRed = new()
    {
        Name    = "AshfolkWildfireCloak",
        Guid    = new(-1023114892),
        Prefab  = "Item_Cloak_T02_WildlingRed",
        NameKey = null,
        DescKey = null,
    };

    // ── T03 Cloaks ────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Cloak_T03_CrimsonWard = new()
    {
        Name    = "CrimsonWard",
        Guid    = new(-1755568324),
        Prefab  = "Item_Cloak_T03_CrimsonWard",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T03_Jester = new()
    {
        Name    = "RoyalVeilOfTheJester",
        Guid    = new(379281083),
        Prefab  = "Item_Cloak_T03_Jester",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T03_Razer = new()
    {
        Name    = "RazerSerpentMantle",
        Guid    = new(136740861),
        Prefab  = "Item_Cloak_T03_Razer",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T03_Royal = new()
    {
        Name    = "RoyalMantleOfAshfolkKings",
        Guid    = new(584164197),
        Prefab  = "Item_Cloak_T03_Royal",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Cloak_T03_UnholyShroud = new()
    {
        Name    = "TailOfTheArchfiend",
        Guid    = new(1863126275),
        Prefab  = "Item_Cloak_T03_UnholyShroud",
        NameKey = null,
        DescKey = null,
    };

    // ── DLC ───────────────────────────────────────────────────────────────────

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Cloak_T0X_PMK01 = new()
    {
        Name    = "AlucardCloak",
        Guid    = new(-1177172544),
        Prefab  = "Item_Cloak_T0X_PMK01",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Sinister Evolution
    public static readonly PrefabDef Item_Cloak_T01_PlagueMaster = new()
    {
        Name    = "ChemicalSoakedDrape",
        Guid    = new(-1514540144),
        Prefab  = "Item_Cloak_T01_PlagueMaster",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Sinister Evolution
    public static readonly PrefabDef Item_Cloak_T02_PlagueMaster = new()
    {
        Name    = "ChemicalSoakedCloak",
        Guid    = new(-1324340002),
        Prefab  = "Item_Cloak_T02_PlagueMaster",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Sinister Evolution
    public static readonly PrefabDef Item_Cloak_T03_PlagueMaster = new()
    {
        Name    = "ChemicalSoakedRegalia",
        Guid    = new(821609569),
        Prefab  = "Item_Cloak_T03_PlagueMaster",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Dracula's Relics
    public static readonly PrefabDef Item_Cloak_T01_Dracula = new()
    {
        Name    = "ImmortalKingsDrape",
        Guid    = new(1284160983),
        Prefab  = "Item_Cloak_T01_Dracula",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Dracula's Relics
    public static readonly PrefabDef Item_Cloak_T02_Dracula = new()
    {
        Name    = "ImmortalKingsCloak",
        Guid    = new(-1067360120),
        Prefab  = "Item_Cloak_T02_Dracula",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Dracula's Relics
    public static readonly PrefabDef Item_Cloak_T03_Dracula = new()
    {
        Name    = "ImmortalKingsMantle",
        Guid    = new(-1814109557),
        Prefab  = "Item_Cloak_T03_Dracula",
        NameKey = null,
        DescKey = null,
    };
}