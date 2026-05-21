// ============================================================
//  SaddleList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/SaddleList.cs
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

public static class SaddleList
{
    public static readonly PrefabDef Item_Saddle_Basic = new()
    {
        Name    = "VampireHorseSaddle",
        Guid    = new(-1209228232),
        Prefab  = "Item_Saddle_Basic",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Eternal Dominance
    public static readonly PrefabDef Item_Saddle_Blackfang_DLC = new()
    {
        Name    = "DarkvenomWarSaddle",
        Guid    = new(-1793846754),
        Prefab  = "Item_Saddle_Blackfang_DLC",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Sinister Evolution
    public static readonly PrefabDef Item_Saddle_Gloomrot_DLC = new()
    {
        Name    = "PlagueChemistsSaddle",
        Guid    = new(-554782766),
        Prefab  = "Item_Saddle_Gloomrot_DLC",
        NameKey = null,
        DescKey = null,
    };

    // DLC: Legacy of Castlevania
    public static readonly PrefabDef Item_Saddle_PMKSkeleton_DLC = new()
    {
        Name    = "RowdainsSteedSaddle",
        Guid    = new(-1270904319),
        Prefab  = "Item_Saddle_PMKSkeleton_DLC",
        NameKey = null,
        DescKey = null,
    };
}