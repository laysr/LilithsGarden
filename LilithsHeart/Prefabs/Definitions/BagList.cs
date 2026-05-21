// ============================================================
//  BagList — LilithsHeart
//  LilithsHeart/Prefabs/Definitions/BagList.cs
//
//  [CHANGED] Field names now match the prefab string exactly, consistent
//            with the bare PrefabGUID convention used before migration.
//            All nullable fields (NameKey, DescKey) are shown explicitly
//            as null until looked up from game data.
//
//  [PERFORMANCE] Static readonly PrefabDef fields — initialised once at
//                class load, zero per-frame cost. Stack-allocated structs,
//                no heap pressure.
// ============================================================

using Stunlock.Core;

namespace LilithsHeart.Prefabs.Definitions;

public static class BagList
{
    public static readonly PrefabDef Item_NewBag_T01 = new()
    {
        Name    = "BanditBag",
        Guid    = new(-219760992),
        Prefab  = "Item_NewBag_T01",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_NewBag_T02 = new()
    {
        Name    = "LeatherBag",
        Guid    = new(-1991977825),
        Prefab  = "Item_NewBag_T02",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_NewBag_T03 = new()
    {
        Name    = "SilverThreadBag",
        Guid    = new(-261654929),
        Prefab  = "Item_NewBag_T03",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_NewBag_T04 = new()
    {
        Name    = "MountainPeakBag",
        Guid    = new(-1922998918),
        Prefab  = "Item_NewBag_T04",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_NewBag_T05 = new()
    {
        Name    = "PristineLeatherBag",
        Guid    = new(1117281334),
        Prefab  = "Item_NewBag_T05",
        NameKey = null,
        DescKey = null,
    };

    public static readonly PrefabDef Item_NewBag_T06 = new()
    {
        Name    = "BatLeatherBag",
        Guid    = new(-181179773),
        Prefab  = "Item_NewBag_T06",
        NameKey = null,
        DescKey = null,
    };
}