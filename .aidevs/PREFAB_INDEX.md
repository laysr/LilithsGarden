# Prefab Definition System Reference

## Overview

The prefab catalog is a **compile-time, type-safe approach** to defining V Rising game entities. Instead of runtime JSON/XML parsing, all prefabs are defined as `static readonly PrefabDef` fields in static classes under `LilithsMind/Prefabs/Definitions/`. These are discovered at runtime via **reflection** by both Heart and Soul.

## PrefabDef Record

**File:** `LilithsMind/Prefabs/PrefabDef.cs`

```csharp
public readonly record struct PrefabDef
{
    string?  Name;     // Human-readable admin name (e.g. "BoneSword")
    int      GuidHash; // Raw int from PrefabGUID._Value
    string   Prefab;   // Game asset name (e.g. "Item_Weapon_Sword_T01_Bone")
    string?  NameKey;  // Localization AssetGuid for display name
    string?  DescKey;  // Localization AssetGuid for tooltip
}
```

## Population States

| State | Fields Filled | Use Case |
|-------|---------------|----------|
| **Stub** | GuidHash + Prefab | Minimum viable — can be looked up by asset name |
| **Partial** | + Name | Admin-friendly config keys, logging, command output |
| **Complete** | + NameKey + DescKey | Localization injection, name/tooltip overrides |

## Discovery Pattern

Both Heart and Soul reflectively scan `typeof(PrefabDef).Assembly` for static classes in namespace `LilithsMind.Prefabs.Definitions`:

```csharp
var types = typeof(PrefabDef).Assembly.GetTypes()
    .Where(t => t.IsClass && t.IsAbstract && t.IsSealed &&
                t.Namespace == "LilithsMind.Prefabs.Definitions");

foreach (var type in types) {
    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static)
        .Where(f => f.FieldType == typeof(PrefabDef));
    foreach (var field in fields) {
        var def = (PrefabDef)field.GetValue(null)!;
        // ... build lookup dictionaries
    }
}
```

## Server-Side Lookups (PrefabNameResolver)

| Dictionary | Key | Value | Source |
|------------|-----|-------|--------|
| `_nameToGuid` | `def.Name` | `PrefabGUID` | LilithsMind Name field |
| `_prefabToGuid` | `def.Prefab` | `PrefabGUID` | LilithsMind Prefab field |
| `_guidToName` | `def.GuidHash` | `string` | Prefers Name, falls back to Prefab |

## Client-Side Lookups

### RecipePatcher._nameToGuid (Two Sources)
1. `PrefabCollectionSystem._PrefabDataLookup.AssetName` → GUID (ECS source of truth)
2. LilithsMind definition `Name` fields → same GUID (admin alias support)

### LocalizationInjector Lookups
| Dictionary | Key | Value |
|------------|-----|-------|
| `_nameToNameGuid` | Prefab string or Name | `AssetGuid` from NameKey |
| `_nameToDescGuid` | Prefab string or Name | `AssetGuid` from DescKey |
| `_injectedGuids` | `AssetGuid` | Tracked for clean removal on server switch |

## Definition File Naming Convention

Each file uses a `public static class` (or `partial class`) named with the category + `Index` suffix:

| File | Class | Lines | Exhaustiveness |
|------|-------|-------|----------------|
| `WeaponsIndex.cs` | `WeaponsIndex` | ~1453 | All weapons |
| `AccessoryIndex.cs` | `AccessoryIndex` | ~274 | All filled out with NameKey/DescKey |
| `StationsIndex.cs` | `StationsIndex` | — | All stations |
| `BagIndex.cs` | `BagIndex` | — | All bags |
| `SaddleIndex.cs` | `SaddleIndex` | — | All saddles |
| `ArmorHeadIndex.cs` | `ArmorHeadIndex` | — | All head armor |
| `ArmorChestIndex.cs` | `ArmorChestIndex` | — | All chest armor |
| `ArmorLegsIndex.cs` | `ArmorLegsIndex` | — | All leg armor |
| `ArmorGlovesIndex.cs` | `ArmorGlovesIndex` | — | All gloves |
| `ArmorBootsIndex.cs` | `ArmorBootsIndex` | — | All boots |
| `ArmorCloakIndex.cs` | `ArmorCloakIndex` | — | All cloaks |
| `ItemsResourcesIndex.cs` | `ItemsResourcesIndex` | — | All resources |
| `ItemsUsableIndex.cs` | `ItemsUsableIndex` | — | All usable items |
| `ItemsMiscIndex.cs` | `ItemsMiscIndex` | — | All misc items |
| `ItemsJewelIndex.cs` | `ItemsJewelIndex` | — | All jewels |
| `ItemsBookIndex.cs` | `ItemsBookIndex` | — | All books |
| `RecipesWeaponIndex.cs` | `RecipesWeaponIndex` | — | All weapon recipes |
| `RecipesUseableIndex.cs` | `RecipesUseableIndex` | — | All usable recipes |
| `RecipesResourceIndex.cs` | `RecipesResourceIndex` | — | All resource recipes |
| `RecipesMiscIndex.cs` | `RecipesMiscIndex` | — | All misc recipes |
| `RecipesJewelIndex.cs` | `RecipesJewelIndex` | — | All jewel recipes |
| `RecipesEquipmentIndex.cs` | `RecipesEquipmentIndex` | — | All equipment recipes |

## Example Entry (Complete — AccessoryIndex)

```csharp
public static readonly PrefabDef Item_MagicSource_BloodKey_T01 = new()
{
    Name    = "BloodKey",
    GuidHash = 1655869633,
    Prefab  = "Item_MagicSource_BloodKey_T01",
    NameKey = "4e77a4af-348e-41d0-88ae-ecbe993d3fa6",
    DescKey = "6d953637-aa08-41db-8c8e-4404483d66d7",
};
```

## Example Entry (Stub — WeaponsIndex)

```csharp
public static readonly PrefabDef Item_Weapon_Sword_T02_Bone_Reinforced = new()
{
    Name    = "ReinforcedBoneSword",
    GuidHash = -796306296,
    Prefab  = "Item_Weapon_Sword_T02_Bone_Reinforced",
    NameKey = null,
    DescKey = null,
};
```

## Adding a New Prefab Entry

1. Determine the category (weapon, armor, item, recipe, etc.)
2. Open the appropriate `*Index.cs` file in `LilithsMind/Prefabs/Definitions/`
3. Add a `public static readonly PrefabDef` field with the prefab's constant name matching the game's Prefab string naming convention
4. Fill GuidHash and Prefab (required)
5. Add Name if you want admin-friendly config keys
6. Add NameKey and DescKey if you want localization injection support (find GUIDs in `LilithsMind/Resources/English.json`)

## Important Constraints

- `GuidHash` is a **signed int** — can be negative. It maps to `PrefabGUID._Value`.
- `NameKey` and `DescKey` are **string GUIDs** (not integers) — they map to `AssetGuid` in `Localization._LocalizedStrings`.
- Entries without `NameKey`/`DescKey` are **silently skipped** by `LocalizationInjector` (logged as warnings).
- An entry's `Name` field takes priority over `Prefab` in all forward lookups — config files can use either form.
- `PrefabDef` is a `readonly record struct` — value semantics, no heap allocation.
