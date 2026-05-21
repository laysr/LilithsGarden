// ============================================================
//  AccessoryList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/AccessoryList.cs
//
//  [CHANGED] Migrated from bare PrefabGUID + [PrefabName] attribute fields
//            to PrefabDef records. Field names match the prefab string
//            exactly. All nullable fields shown explicitly.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class AccessoryList
{
    // ── Rings ─────────────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_MagicSource_BloodKey_T01 = new()
    {
        Name    = "BloodKey",
        Guid    = new(1655869633),
        Prefab  = "Item_MagicSource_BloodKey_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T01_BoneRing = new()
    {
        Name    = "BoneRing",
        Guid    = new(1557814269),
        Prefab  = "Item_MagicSource_General_T01_BoneRing",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T02_BloodBoneRing = new()
    {
        Name    = "BloodBoneRing",
        Guid    = new(-652069131),
        Prefab  = "Item_MagicSource_General_T02_BloodBoneRing",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T03_GravediggerRing = new()
    {
        Name    = "GravediggerRing",
        Guid    = new(-1588051702),
        Prefab  = "Item_MagicSource_General_T03_GravediggerRing",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T04_Duskwatcher = new()
    {
        Name    = "RingOfTheDuskwatcher",
        Guid    = new(-809059551),
        Prefab  = "Item_MagicSource_General_T04_Duskwatcher",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T04_EmberChain = new()
    {
        Name    = "RingOfTheDawnrunner",
        Guid    = new(50824544),
        Prefab  = "Item_MagicSource_General_T04_EmberChain",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T04_FrozenEye = new()
    {
        Name    = "RingOfTheWarlock",
        Guid    = new(336922685),
        Prefab  = "Item_MagicSource_General_T04_FrozenEye",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T04_MistSignet = new()
    {
        Name    = "RingOfTheSpellweaver",
        Guid    = new(-886916793),
        Prefab  = "Item_MagicSource_General_T04_MistSignet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T04_RubyRing = new()
    {
        Name    = "RingOfTheWarrior",
        Guid    = new(341837267),
        Prefab  = "Item_MagicSource_General_T04_RubyRing",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T04_SorcererRing = new()
    {
        Name    = "RingOfTheSorcerer",
        Guid    = new(-1184863500),
        Prefab  = "Item_MagicSource_General_T04_SorcererRing",
        NameKey = null,
        DescKey = null,
    };

    // ── Pendants & Amulets ────────────────────────────────────────────────────

    public static readonly PrefabDef Item_MagicSource_General_T05_Relic = new()
    {
        Name    = "ScourgestonePendant",
        Guid    = new(-650855520),
        Prefab  = "Item_MagicSource_General_T05_Relic",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T06_AmethystPendant = new()
    {
        Name    = "PendantOfTheSorcerer",
        Guid    = new(199425997),
        Prefab  = "Item_MagicSource_General_T06_AmethystPendant",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T06_EmeraldNecklace = new()
    {
        Name    = "PendantOfTheDawnrunner",
        Guid    = new(-1046748791),
        Prefab  = "Item_MagicSource_General_T06_EmeraldNecklace",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T06_MistStoneNecklace = new()
    {
        Name    = "PendantOfTheSpellweaver",
        Guid    = new(1012837641),
        Prefab  = "Item_MagicSource_General_T06_MistStoneNecklace",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T06_RubyPendant = new()
    {
        Name    = "PendantOfTheWarrior",
        Guid    = new(-425306671),
        Prefab  = "Item_MagicSource_General_T06_RubyPendant",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T06_SapphirePendant = new()
    {
        Name    = "PendantOfTheWarlock",
        Guid    = new(-651554566),
        Prefab  = "Item_MagicSource_General_T06_SapphirePendant",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T06_TopazAmulet = new()
    {
        Name    = "PendantOfTheDuskwatcher",
        Guid    = new(610958202),
        Prefab  = "Item_MagicSource_General_T06_TopazAmulet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T07_BloodwineAmulet = new()
    {
        Name    = "BloodMerlotAmulet",
        Guid    = new(991396285),
        Prefab  = "Item_MagicSource_General_T07_BloodwineAmulet",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T08_Blood = new()
    {
        Name    = "AmuletOfTheCrimsonCommander",
        Guid    = new(-104934480),
        Prefab  = "Item_MagicSource_General_T08_Blood",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T08_Chaos = new()
    {
        Name    = "AmuletOfTheWickedProphet",
        Guid    = new(-175650376),
        Prefab  = "Item_MagicSource_General_T08_Chaos",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T08_Frost = new()
    {
        Name    = "AmuletOfTheArchWarlock",
        Guid    = new(1380368392),
        Prefab  = "Item_MagicSource_General_T08_Frost",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T08_Illusion = new()
    {
        Name    = "AmuletOfTheMasterSpellweaver",
        Guid    = new(-1306155896),
        Prefab  = "Item_MagicSource_General_T08_Illusion",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T08_Storm = new()
    {
        Name    = "AmuletOfTheBlademaster",
        Guid    = new(-296161379),
        Prefab  = "Item_MagicSource_General_T08_Storm",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_General_T08_Unholy = new()
    {
        Name    = "AmuletOfTheUnyieldingCharger",
        Guid    = new(-1004351840),
        Prefab  = "Item_MagicSource_General_T08_Unholy",
        NameKey = null,
        DescKey = null,
    };

    // ── Soul Shards ───────────────────────────────────────────────────────────

    public static readonly PrefabDef Item_MagicSource_SoulShard_Solarus = new()
    {
        Name    = "SoulShardOfSolarus",
        Guid    = new(-21943750),
        Prefab  = "Item_MagicSource_SoulShard_Solarus",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_SoulShard_Monster = new()
    {
        Name    = "SoulShardOfTheMonster",
        Guid    = new(-1581189572),
        Prefab  = "Item_MagicSource_SoulShard_Monster",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_SoulShard_Manticore = new()
    {
        Name    = "SoulShardOfTheWingedHorror",
        Guid    = new(-1260254082),
        Prefab  = "Item_MagicSource_SoulShard_Manticore",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_SoulShard_Morgana = new()
    {
        Name    = "SoulShardOfTheSerpent",
        Guid    = new(1286615355),
        Prefab  = "Item_MagicSource_SoulShard_Morgana",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_MagicSource_SoulShard_Dracula = new()
    {
        Name    = "SoulShardOfDracula",
        Guid    = new(666638454),
        Prefab  = "Item_MagicSource_SoulShard_Dracula",
        NameKey = null,
        DescKey = null,
    };
}