// ============================================================
//  ArmorHeadList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/ArmorHeadList.cs
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

public static class ArmorHeadList
{
    public static readonly PrefabDef Item_Headgear_ArcMageCrown = new()
    {
        Name    = "CrownOfTheArchmage",
        Guid    = new(-2128818978),
        Prefab  = "Item_Headgear_ArcMageCrown",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_AshfolkCrown = new()
    {
        Name    = "AshfolkCrown",
        Guid    = new(-1988816037),
        Prefab  = "Item_Headgear_AshfolkCrown",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_AshfolkHelmet = new()
    {
        Name    = "AshfolkHelmet",
        Guid    = new(-1607893829),
        Prefab  = "Item_Headgear_AshfolkHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_BearTrophy = new()
    {
        Name    = "BearHead",
        Guid    = new(714007172),
        Prefab  = "Item_Headgear_BearTrophy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_BoneMask = new()
    {
        Name    = "BoneguardMask",
        Guid    = new(-2111388989),
        Prefab  = "Item_Headgear_BoneMask",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_Bonnet = new()
    {
        Name    = "Bonnet",
        Guid    = new(-152150271),
        Prefab  = "Item_Headgear_Bonnet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_DeerTrophy = new()
    {
        Name    = "DeerHead",
        Guid    = new(1707139699),
        Prefab  = "Item_Headgear_DeerTrophy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_FootmansHelmet = new()
    {
        Name    = "FootmansHelmet",
        Guid    = new(-353076115),
        Prefab  = "Item_Headgear_FootmansHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_GeneralHelmet = new()
    {
        Name    = "UndeadGeneralHelmet",
        Guid    = new(409678749),
        Prefab  = "Item_Headgear_GeneralHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_KnightsHelmet = new()
    {
        Name    = "KnightsHelmet",
        Guid    = new(-1818243335),
        Prefab  = "Item_Headgear_KnightsHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_MaidCap = new()
    {
        Name    = "MaidsCap",
        Guid    = new(-1721887666),
        Prefab  = "Item_Headgear_MaidCap",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_MaidScarf = new()
    {
        Name    = "MaidsScarf",
        Guid    = new(-1460281233),
        Prefab  = "Item_Headgear_MaidScarf",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_MilitiaHelmet = new()
    {
        Name    = "MilitiaHelmet",
        Guid    = new(417648894),
        Prefab  = "Item_Headgear_MilitiaHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_NecromancerMitre = new()
    {
        Name    = "NecromancersMitre",
        Guid    = new(607559019),
        Prefab  = "Item_Headgear_NecromancerMitre",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_NightlurkerTrophy = new()
    {
        Name    = "NightlurkerHead",
        Guid    = new(-2073081569),
        Prefab  = "Item_Headgear_NightlurkerTrophy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_PaladinsHelmet = new()
    {
        Name    = "PaladinsHelmet",
        Guid    = new(1780339680),
        Prefab  = "Item_Headgear_PaladinsHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_PilgrimsHat = new()
    {
        Name    = "PilgrimsHat",
        Guid    = new(-1071187362),
        Prefab  = "Item_Headgear_PilgrimsHat",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_PopeMitre = new()
    {
        Name    = "Mitre",
        Guid    = new(-548847761),
        Prefab  = "Item_Headgear_PopeMitre",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_RazerHood = new()
    {
        Name    = "RazerHood",
        Guid    = new(-1797796642),
        Prefab  = "Item_Headgear_RazerHood",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_RustedHelmet = new()
    {
        Name    = "RustedHelmet",
        Guid    = new(1364460757),
        Prefab  = "Item_Headgear_RustedHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_RustedMilitiaHelmet = new()
    {
        Name    = "RustedMilitiaHelmet",
        Guid    = new(764480170),
        Prefab  = "Item_Headgear_RustedMilitiaHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_Scarecrow = new()
    {
        Name    = "ScarecrowMask",
        Guid    = new(403967307),
        Prefab  = "Item_Headgear_Scarecrow",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_SlaveHelmet = new()
    {
        Name    = "SlaveHelmet",
        Guid    = new(1587354182),
        Prefab  = "Item_Headgear_SlaveHelmet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_StartingRags = new()
    {
        Name    = "SkullMask",
        Guid    = new(-2125696865),
        Prefab  = "Item_Headgear_StartingRags",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_Strawhat = new()
    {
        Name    = "StrawHat",
        Guid    = new(1375804543),
        Prefab  = "Item_Headgear_Strawhat",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_TopHat = new()
    {
        Name    = "TopHat",
        Guid    = new(690259405),
        Prefab  = "Item_Headgear_TopHat",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_VampireHunterHat = new()
    {
        Name    = "VampireHunterHat",
        Guid    = new(974739126),
        Prefab  = "Item_Headgear_VampireHunterHat",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_VampireHunterHat02 = new()
    {
        Name    = "VampireHunterHat02",
        Guid    = new(-1785271534),
        Prefab  = "Item_Headgear_VampireHunterHat02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_WerewolfTrophy = new()
    {
        Name    = "WerewolfHead",
        Guid    = new(-2020831626),
        Prefab  = "Item_Headgear_WerewolfTrophy",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_Headgear_WolfTrophy01 = new()
    {
        Name    = "WolfHead",
        Guid    = new(-1169471531),
        Prefab  = "Item_Headgear_WolfTrophy01",
        NameKey = null,
        DescKey = null,
    };

    // ── DLC ───────────────────────────────────────────────────────────────────

    // DLC: Eternal Dominance Pack
    public static readonly PrefabDef Item_Headgear_BlackfangSultan = new()
    {
        Name    = "OpulentNightVisage",
        Guid    = new(1729289046),
        Prefab  = "Item_Headgear_BlackfangSultan",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Dracula's Relics
    public static readonly PrefabDef Item_Headgear_DraculaHelmet = new()
    {
        Name    = "ImmortalKingsGreathelm",
        Guid    = new(238268650),
        Prefab  = "Item_Headgear_DraculaHelmet",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Sinister Evolution
    public static readonly PrefabDef Item_Headgear_Plaguemaster = new()
    {
        Name    = "PlagueDoctorMask",
        Guid    = new(-262204844),
        Prefab  = "Item_Headgear_Plaguemaster",
        NameKey = null,
        DescKey = null,
    };

    // ── Unused ────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_Armor_Headgear_Base = new()
    {
        Name    = "HeadgearBase",
        Guid    = new(-1905547794),
        Prefab  = "Item_Armor_Headgear_Base",
        NameKey = null,
        DescKey = null,
    };
}