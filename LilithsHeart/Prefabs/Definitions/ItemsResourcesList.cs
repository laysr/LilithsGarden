// ============================================================
//  ItemsResourcesList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ItemsResourcesList.cs
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

public static class ItemsResourcesList
{
    // ── Blood Essence ─────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_BloodEssence_T01 = new()
    {
        Name    = "BloodEssence",
        Guid    = new(862477668),
        Prefab  = "Item_BloodEssence_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_BloodEssence_T02_Greater = new()
    {
        Name    = "GreaterBloodEssence",
        Guid    = new(271594022),
        Prefab  = "Item_BloodEssence_T02_Greater",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_BloodEssence_T03_Primal = new()
    {
        Name    = "PrimalBloodEssence",
        Guid    = new(1566989408),
        Prefab  = "Item_BloodEssence_T03_Primal",
        NameKey = null,
        DescKey = null,
    };

    // ── Stygian Shards ────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_NetherShard_T01 = new()
    {
        Name    = "StygianShard",
        Guid    = new(2103989354),
        Prefab  = "Item_NetherShard_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_NetherShard_T02 = new()
    {
        Name    = "GreaterStygianShard",
        Guid    = new(576389135),
        Prefab  = "Item_NetherShard_T02",
        NameKey = null,
        DescKey = null,
    };

    // ── Plants ────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Plant_PlantFiber = new()
    {
        Name    = "PlantFiber",
        Guid    = new(-1409142667),
        Prefab  = "Item_Ingredient_Plant_PlantFiber",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Pollen = new()
    {
        Name    = "Pollen",
        Guid    = new(855691699),
        Prefab  = "Item_Ingredient_Pollen",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_BloodRose = new()
    {
        Name    = "BloodRose",
        Guid    = new(1726420644),
        Prefab  = "Item_Ingredient_Plant_BloodRose",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_FireBlossom = new()
    {
        Name    = "FireBlossom",
        Guid    = new(455638025),
        Prefab  = "Item_Ingredient_Plant_FireBlossom",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_MourningLily = new()
    {
        Name    = "MourningLily",
        Guid    = new(-363718499),
        Prefab  = "Item_Ingredient_Plant_MourningLily",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_SnowFlower = new()
    {
        Name    = "Snowflower",
        Guid    = new(106516056),
        Prefab  = "Item_Ingredient_Plant_SnowFlower",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_HellsClarion = new()
    {
        Name    = "HellsClarion",
        Guid    = new(813370507),
        Prefab  = "Item_Ingredient_Plant_HellsClarion",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_Cotton = new()
    {
        Name    = "Cotton",
        Guid    = new(362941759),
        Prefab  = "Item_Ingredient_Plant_Cotton",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_GhostShroom = new()
    {
        Name    = "GhostShroom",
        Guid    = new(-164367832),
        Prefab  = "Item_Ingredient_Plant_GhostShroom",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_PlagueBrier = new()
    {
        Name    = "PlagueBrier",
        Guid    = new(1474643910),
        Prefab  = "Item_Ingredient_Plant_PlagueBrier",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_Sunflower = new()
    {
        Name    = "Sunflower",
        Guid    = new(1105981714),
        Prefab  = "Item_Ingredient_Plant_Sunflower",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_SacredGrapes = new()
    {
        Name    = "SacredGrapes",
        Guid    = new(88009216),
        Prefab  = "Item_Ingredient_Plant_SacredGrapes",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_CorruptedFlower = new()
    {
        Name    = "CorruptedFlower",
        Guid    = new(-926928101),
        Prefab  = "Item_Ingredient_Plant_CorruptedFlower",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plant_BleedingHeart = new()
    {
        Name    = "BleedingHeart",
        Guid    = new(-135435342),
        Prefab  = "Item_Ingredient_Plant_BleedingHeart",
        NameKey = null,
        DescKey = null,
    };

    // ── Wood ──────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Wood_Standard = new()
    {
        Name    = "Wood",
        Guid    = new(-1593377811),
        Prefab  = "Item_Ingredient_Wood_Standard",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Wood_Hallow = new()
    {
        Name    = "HallowWood",
        Guid    = new(-2014020548),
        Prefab  = "Item_Ingredient_Wood_Hallow",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Wood_Gloom = new()
    {
        Name    = "GloomWood",
        Guid    = new(-1740500585),
        Prefab  = "Item_Ingredient_Wood_Gloom",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Wood_Cursed = new()
    {
        Name    = "CursedWood",
        Guid    = new(608397239),
        Prefab  = "Item_Ingredient_Wood_Cursed",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Wood_CorruptedOak = new()
    {
        Name    = "CorruptedOak",
        Guid    = new(440780337),
        Prefab  = "Item_Ingredient_Wood_CorruptedOak",
        NameKey = null,
        DescKey = null,
    };

    // ── Hide ──────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_RuggedHide = new()
    {
        Name    = "RuggedHide",
        Guid    = new(-1222725729),
        Prefab  = "Item_Ingredient_RuggedHide",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_ThickHide = new()
    {
        Name    = "ThickHide",
        Guid    = new(-2047402903),
        Prefab  = "Item_Ingredient_ThickHide",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_PristineHide = new()
    {
        Name    = "PristineHide",
        Guid    = new(1658596502),
        Prefab  = "Item_Ingredient_PristineHide",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_BatHide = new()
    {
        Name    = "BatHide",
        Guid    = new(1262845777),
        Prefab  = "Item_Ingredient_BatHide",
        NameKey = null,
        DescKey = null,
    };

    // ── Other Raw Resources ───────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Bone = new()
    {
        Name    = "Bone",
        Guid    = new(1821405450),
        Prefab  = "Item_Ingredient_Bone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Silkworm = new()
    {
        Name    = "Silkworm",
        Guid    = new(-11246506),
        Prefab  = "Item_Ingredient_Silkworm",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_MutantGrease = new()
    {
        Name    = "MutantGrease",
        Guid    = new(-1527315816),
        Prefab  = "Item_Ingredient_MutantGrease",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Stone = new()
    {
        Name    = "Stone",
        Guid    = new(-1531666018),
        Prefab  = "Item_Ingredient_Stone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Clay = new()
    {
        Name    = "Clay",
        Guid    = new(317317590),
        Prefab  = "Item_Ingredient_Clay",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_TechScrap = new()
    {
        Name    = "TechScrap",
        Guid    = new(834864259),
        Prefab  = "Item_Ingredient_TechScrap",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_BloodCrystal = new()
    {
        Name    = "BloodCrystal",
        Guid    = new(-1913156733),
        Prefab  = "Item_Ingredient_BloodCrystal",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Emery = new()
    {
        Name    = "Emery",
        Guid    = new(-1578565561),
        Prefab  = "Item_Ingredient_Emery",
        NameKey = null,
        DescKey = null,
    };

    // ── Ores & Minerals ───────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Mineral_CopperOre = new()
    {
        Name    = "CopperOre",
        Guid    = new(-1805325497),
        Prefab  = "Item_Ingredient_Mineral_CopperOre",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_SulfurOre = new()
    {
        Name    = "SulphurOre",
        Guid    = new(1501868129),
        Prefab  = "Item_Ingredient_Mineral_SulfurOre",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_IronOre = new()
    {
        Name    = "IronOre",
        Guid    = new(1332980397),
        Prefab  = "Item_Ingredient_Mineral_IronOre",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_Quartz = new()
    {
        Name    = "Quartz",
        Guid    = new(-1583485601),
        Prefab  = "Item_Ingredient_Mineral_Quartz",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_SilverOre = new()
    {
        Name    = "SilverOre",
        Guid    = new(1686577386),
        Prefab  = "Item_Ingredient_Mineral_SilverOre",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_GhostCrystal = new()
    {
        Name    = "GhostCrystal",
        Guid    = new(-1748835106),
        Prefab  = "Item_Ingredient_Mineral_GhostCrystal",
        NameKey = null,
        DescKey = null,
    };

    // ── Thread & Fabric ───────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Thread_Coarse = new()
    {
        Name    = "CoarseThread",
        Guid    = new(-1562867444),
        Prefab  = "Item_Ingredient_Thread_Coarse",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Thread_Wool = new()
    {
        Name    = "WoolThread",
        Guid    = new(1872733144),
        Prefab  = "Item_Ingredient_Thread_Wool",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_CottonYarn = new()
    {
        Name    = "CottonYarn",
        Guid    = new(444400639),
        Prefab  = "Item_Ingredient_CottonYarn",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_GhostYarn = new()
    {
        Name    = "GhostYarn",
        Guid    = new(2106123809),
        Prefab  = "Item_Ingredient_GhostYarn",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Cloth = new()
    {
        Name    = "Cloth",
        Guid    = new(-700774739),
        Prefab  = "Item_Ingredient_Cloth",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_CarpetRoll = new()
    {
        Name    = "CarpetRoll",
        Guid    = new(1046366876),
        Prefab  = "Item_Ingredient_CarpetRoll",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Silk = new()
    {
        Name    = "Silk",
        Guid    = new(702067317),
        Prefab  = "Item_Ingredient_Silk",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_ShadowWeave = new()
    {
        Name    = "ShadowWeave",
        Guid    = new(-1458997116),
        Prefab  = "Item_Ingredient_ShadowWeave",
        NameKey = null,
        DescKey = null,
    };

    // ── Wood Refined ──────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Sawdust = new()
    {
        Name    = "Sawdust",
        Guid    = new(-1979905169),
        Prefab  = "Item_Ingredient_Sawdust",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Plank = new()
    {
        Name    = "Plank",
        Guid    = new(-1017402979),
        Prefab  = "Item_Ingredient_Plank",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_CorruptedSap = new()
    {
        Name    = "CorruptedSap",
        Guid    = new(2012771684),
        Prefab  = "Item_Ingredient_CorruptedSap",
        NameKey = null,
        DescKey = null,
    };

    // ── Leather ───────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Leather = new()
    {
        Name    = "Leather",
        Guid    = new(-1907572080),
        Prefab  = "Item_Ingredient_Leather",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_ThickLeather = new()
    {
        Name    = "ThickLeather",
        Guid    = new(-305160765),
        Prefab  = "Item_Ingredient_ThickLeather",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_PristineLeather = new()
    {
        Name    = "PristineLeather",
        Guid    = new(-2043983118),
        Prefab  = "Item_Ingredient_PristineLeather",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_BatLeather = new()
    {
        Name    = "BatLeather",
        Guid    = new(-1886460367),
        Prefab  = "Item_Ingredient_BatLeather",
        NameKey = null,
        DescKey = null,
    };

    // ── Stone Refined ─────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Stonedust = new()
    {
        Name    = "StoneDust",
        Guid    = new(1388962120),
        Prefab  = "Item_Ingredient_Stonedust",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_StoneBrick = new()
    {
        Name    = "StoneBrick",
        Guid    = new(1788016417),
        Prefab  = "Item_Ingredient_StoneBrick",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gemdust = new()
    {
        Name    = "GemDust",
        Guid    = new(820932258),
        Prefab  = "Item_Ingredient_Gemdust",
        NameKey = null,
        DescKey = null,
    };

    // ── Metals & Alloys ───────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Mineral_CopperIngot = new()
    {
        Name    = "CopperIngot",
        Guid    = new(-1237019921),
        Prefab  = "Item_Ingredient_Mineral_CopperIngot",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_Sulfur = new()
    {
        Name    = "Sulfur",
        Guid    = new(880699252),
        Prefab  = "Item_Ingredient_Mineral_Sulfur",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_IronBar = new()
    {
        Name    = "IronBar",
        Guid    = new(-1750550553),
        Prefab  = "Item_Ingredient_Mineral_IronBar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Glass = new()
    {
        Name    = "Glass",
        Guid    = new(-1233716303),
        Prefab  = "Item_Ingredient_Glass",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_SilverBar = new()
    {
        Name    = "SilverBar",
        Guid    = new(-1787563914),
        Prefab  = "Item_Ingredient_Mineral_SilverBar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_RadiumAlloy = new()
    {
        Name    = "RadiumAlloy",
        Guid    = new(2116142390),
        Prefab  = "Item_Ingredient_RadiumAlloy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_GoldBar = new()
    {
        Name    = "GoldBar",
        Guid    = new(-1027710236),
        Prefab  = "Item_Ingredient_Mineral_GoldBar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Spectraldust = new()
    {
        Name    = "SpectralDust",
        Guid    = new(-2130812821),
        Prefab  = "Item_Ingredient_Spectraldust",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_DarkSilverBar = new()
    {
        Name    = "DarkSilverBar",
        Guid    = new(-762000259),
        Prefab  = "Item_Ingredient_Mineral_DarkSilverBar",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Emberglass = new()
    {
        Name    = "Emberglass",
        Guid    = new(-1715039285),
        Prefab  = "Item_Ingredient_Emberglass",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gravedust = new()
    {
        Name    = "GraveDust",
        Guid    = new(-608131642),
        Prefab  = "Item_Ingredient_Gravedust",
        NameKey = null,
        DescKey = null,
    };

    // ── Fish Byproducts ───────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_FishBone = new()
    {
        Name    = "Fishbone",
        Guid    = new(424158416),
        Prefab  = "Item_Ingredient_FishBone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fishoil = new()
    {
        Name    = "FishOil",
        Guid    = new(-242277891),
        Prefab  = "Item_Ingredient_Fishoil",
        NameKey = null,
        DescKey = null,
    };

    // ── Components ────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Research_Paper = new()
    {
        Name    = "Paper",
        Guid    = new(780044299),
        Prefab  = "Item_Ingredient_Research_Paper",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Research_Scroll = new()
    {
        Name    = "Scroll",
        Guid    = new(2065714452),
        Prefab  = "Item_Ingredient_Research_Scroll",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Research_Schematic = new()
    {
        Name    = "Schematic",
        Guid    = new(2085163661),
        Prefab  = "Item_Ingredient_Research_Schematic",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_SculpturedWood = new()
    {
        Name    = "SculpturedWood",
        Guid    = new(-793913499),
        Prefab  = "Item_Ingredient_SculpturedWood",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_PaintingFrame = new()
    {
        Name    = "PaintingFrame",
        Guid    = new(1553481703),
        Prefab  = "Item_Ingredient_PaintingFrame",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_ReinforcedPlank = new()
    {
        Name    = "ReinforcedPlank",
        Guid    = new(-1397591435),
        Prefab  = "Item_Ingredient_ReinforcedPlank",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_StoneBody = new()
    {
        Name    = "StoneBody",
        Guid    = new(689272399),
        Prefab  = "Item_Ingredient_StoneBody",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Whetstone = new()
    {
        Name    = "Whetstone",
        Guid    = new(1252507075),
        Prefab  = "Item_Ingredient_Whetstone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Scourgestone = new()
    {
        Name    = "Scourgestone",
        Guid    = new(1005440012),
        Prefab  = "Item_Ingredient_Scourgestone",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Mineral_Obsidian = new()
    {
        Name    = "Obsidian",
        Guid    = new(-543524210),
        Prefab  = "Item_Ingredient_Mineral_Obsidian",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_OnyxTear = new()
    {
        Name    = "OnyxTear",
        Guid    = new(-651878258),
        Prefab  = "Item_Ingredient_OnyxTear",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_CopperWires = new()
    {
        Name    = "CopperWires",
        Guid    = new(-456161884),
        Prefab  = "Item_Ingredient_CopperWires",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Battery = new()
    {
        Name    = "DepletedBattery",
        Guid    = new(1270271716),
        Prefab  = "Item_Ingredient_Battery",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_BatteryCharged = new()
    {
        Name    = "ChargedBattery",
        Guid    = new(-412448857),
        Prefab  = "Item_Ingredient_BatteryCharged",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_PowerCore = new()
    {
        Name    = "PowerCore",
        Guid    = new(-1190647720),
        Prefab  = "Item_Ingredient_PowerCore",
        NameKey = null,
        DescKey = null,
    };

    // ── Currency ──────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_GoldenJewelry = new()
    {
        Name    = "GoldenJewelry",
        Guid    = new(-1749304196),
        Prefab  = "Item_Ingredient_GoldenJewelry",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Coin_Copper = new()
    {
        Name    = "CopperCoin",
        Guid    = new(28625845),
        Prefab  = "Item_Ingredient_Coin_Copper",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Coin_Silver = new()
    {
        Name    = "SilverCoin",
        Guid    = new(-949672483),
        Prefab  = "Item_Ingredient_Coin_Silver",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Coin_Royal = new()
    {
        Name    = "GoldsunCoin",
        Guid    = new(-571562864),
        Prefab  = "Item_Ingredient_Coin_Royal",
        NameKey = null,
        DescKey = null,
    };

    // ── Fish ──────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Fish_BloodSnapper_T02 = new()
    {
        Name    = "BloodSnapper",
        Guid    = new(-1779269313),
        Prefab  = "Item_Ingredient_Fish_BloodSnapper_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_Corrupted_T03 = new()
    {
        Name    = "CorruptedFish",
        Guid    = new(2069171407),
        Prefab  = "Item_Ingredient_Fish_Corrupted_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_FatGoby_T01 = new()
    {
        Name    = "FatGoby",
        Guid    = new(-1642545082),
        Prefab  = "Item_Ingredient_Fish_FatGoby_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_FierceStinger_T01 = new()
    {
        Name    = "FierceStinger",
        Guid    = new(447901086),
        Prefab  = "Item_Ingredient_Fish_FierceStinger_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_GoldenRiverBass_T03 = new()
    {
        Name    = "GoldenRiverBass",
        Guid    = new(67930804),
        Prefab  = "Item_Ingredient_Fish_GoldenRiverBass_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_RainbowTrout_T01 = new()
    {
        Name    = "RainbowTrout",
        Guid    = new(-149778795),
        Prefab  = "Item_Ingredient_Fish_RainbowTrout_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_SageFish_T02 = new()
    {
        Name    = "Sagefish",
        Guid    = new(736318803),
        Prefab  = "Item_Ingredient_Fish_SageFish_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_SwampDweller_T03 = new()
    {
        Name    = "SwampDweller",
        Guid    = new(177845365),
        Prefab  = "Item_Ingredient_Fish_SwampDweller_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Fish_TwilightSnapper_T02 = new()
    {
        Name    = "TwilightSnapper",
        Guid    = new(-570287766),
        Prefab  = "Item_Ingredient_Fish_TwilightSnapper_T02",
        NameKey = null,
        DescKey = null,
    };

    // ── Gems ──────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Ingredient_Gem_Amethyst_T01 = new()
    {
        Name    = "CrudeAmethyst",
        Guid    = new(225530658),
        Prefab  = "Item_Ingredient_Gem_Amethyst_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Amethyst_T02 = new()
    {
        Name    = "RegularAmethyst",
        Guid    = new(-1962855117),
        Prefab  = "Item_Ingredient_Gem_Amethyst_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Amethyst_T03 = new()
    {
        Name    = "FlawlessAmethyst",
        Guid    = new(1705028227),
        Prefab  = "Item_Ingredient_Gem_Amethyst_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Emerald_T01 = new()
    {
        Name    = "CrudeEmerald",
        Guid    = new(-237441421),
        Prefab  = "Item_Ingredient_Gem_Emerald_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Emerald_T02 = new()
    {
        Name    = "RegularEmerald",
        Guid    = new(357608868),
        Prefab  = "Item_Ingredient_Gem_Emerald_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Emerald_T03 = new()
    {
        Name    = "FlawlessEmerald",
        Guid    = new(1898237421),
        Prefab  = "Item_Ingredient_Gem_Emerald_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Miststone_T01 = new()
    {
        Name    = "CrudeMiststone",
        Guid    = new(-1129708579),
        Prefab  = "Item_Ingredient_Gem_Miststone_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Miststone_T02 = new()
    {
        Name    = "RegularMiststone",
        Guid    = new(802050789),
        Prefab  = "Item_Ingredient_Gem_Miststone_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Miststone_T03 = new()
    {
        Name    = "FlawlessMiststone",
        Guid    = new(-1963826510),
        Prefab  = "Item_Ingredient_Gem_Miststone_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Ruby_T01 = new()
    {
        Name    = "CrudeRuby",
        Guid    = new(287586809),
        Prefab  = "Item_Ingredient_Gem_Ruby_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Ruby_T02 = new()
    {
        Name    = "RegularRuby",
        Guid    = new(51046573),
        Prefab  = "Item_Ingredient_Gem_Ruby_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Ruby_T03 = new()
    {
        Name    = "FlawlessRuby",
        Guid    = new(74811721),
        Prefab  = "Item_Ingredient_Gem_Ruby_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Sapphire_T01 = new()
    {
        Name    = "CrudeSapphire",
        Guid    = new(552409457),
        Prefab  = "Item_Ingredient_Gem_Sapphire_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Sapphire_T02 = new()
    {
        Name    = "RegularSapphire",
        Guid    = new(131015492),
        Prefab  = "Item_Ingredient_Gem_Sapphire_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Sapphire_T03 = new()
    {
        Name    = "FlawlessSapphire",
        Guid    = new(-1147920398),
        Prefab  = "Item_Ingredient_Gem_Sapphire_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Topaz_T01 = new()
    {
        Name    = "CrudeTopaz",
        Guid    = new(867351268),
        Prefab  = "Item_Ingredient_Gem_Topaz_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Topaz_T02 = new()
    {
        Name    = "RegularTopaz",
        Guid    = new(-2118441460),
        Prefab  = "Item_Ingredient_Gem_Topaz_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Ingredient_Gem_Topaz_T03 = new()
    {
        Name    = "FlawlessTopaz",
        Guid    = new(-2051574178),
        Prefab  = "Item_Ingredient_Gem_Topaz_T03",
        NameKey = null,
        DescKey = null,
    };
}