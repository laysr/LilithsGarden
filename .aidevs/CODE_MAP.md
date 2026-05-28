# Code Map — File-by-File Index

## Root Files

| File | Purpose |
|------|---------|
| `LilithsGarden.sln` | Visual Studio solution referencing 4 projects |
| `Directory.Build.props` | Shared MSBuild properties (net6.0, C# 12, nullable, BepInEx packages) |
| `global.json` | Pins .NET SDK to 8.0.421 |
| `README.md` | Project description + naming conventions |

---

## LilithsMind (Shared Library — Pure C#)

### Root

| File | Purpose |
|------|---------|
| `LilithsMind.csproj` | Project file, no NuGet refs, version 0.1.0 |

### Prefabs/

| File | Class | Purpose |
|------|-------|---------|
| `PrefabDef.cs` | `PrefabDef` | `readonly record struct` — universal prefab definition (Name, GuidHash, Prefab, NameKey, DescKey). Stack-allocated, zero heap pressure. |

### Prefabs/Definitions/ — 22 static index classes

| File | Contains |
|------|----------|
| `WeaponsIndex.cs` | All weapon items (swords, axes, maces, spears, pistols, etc.) — ~1453 lines |
| `StationsIndex.cs` | All crafting/refinement station prefabs |
| `ArmorHeadIndex.cs` | Helmet/head armor prefabs |
| `ArmorChestIndex.cs` | Chest armor prefabs |
| `ArmorLegsIndex.cs` | Leg armor prefabs |
| `ArmorGlovesIndex.cs` | Glove armor prefabs |
| `ArmorBootsIndex.cs` | Boot armor prefabs |
| `ArmorCloakIndex.cs` | Cloak prefabs |
| `AccessoryIndex.cs` | Rings, sources, necklaces — fully filled out with NameKey/DescKey |
| `BagIndex.cs` | Bag/container prefabs |
| `SaddleIndex.cs` | Mount saddle prefabs |
| `ItemsResourcesIndex.cs` | Resource items (minerals, ingots, lumber, etc.) |
| `ItemsUsableIndex.cs` | Usable/consumable items |
| `ItemsMiscIndex.cs` | Miscellaneous items |
| `ItemsJewelIndex.cs` | Jewel/gem items |
| `ItemsBookIndex.cs` | Book/schematics items |
| `RecipesWeaponIndex.cs` | Weapon recipe prefabs |
| `RecipesUseableIndex.cs` | Usable item recipe prefabs |
| `RecipesResourceIndex.cs` | Resource recipe prefabs |
| `RecipesMiscIndex.cs` | Miscellaneous recipe prefabs |
| `RecipesJewelIndex.cs` | Jewel recipe prefabs |
| `RecipesEquipmentIndex.cs` | Equipment recipe prefabs |

### Network/

| File | Class | Purpose |
|------|-------|---------|
| `ServerSyncPayload.cs` | `ServerSyncPayload` | Full data contract sent on connect: identity, localization overrides, recipe overrides, station overrides, player recipe changes |
| `ServerEventPayload.cs` | `ServerEventPayload`, `EventKind` | Trigger-based in-session payload. EventKind uses reserved ranges (Core 0-99, Cookbook 100-199, Bounty 200-299, Treasury 300-399, Machinations 400-499) |
| `LilithRecipeData.cs` | `LilithRecipeData` | DTO: CraftDuration, Requirements (Dictionary<string,int>), Outputs (Dictionary<string,int>) |
| `LilithStationData.cs` | `LilithStationData` | DTO: RecipesToAdd (List<string>), RecipesToRemove (List<string>) |

### Resources/

| File | Purpose |
|------|---------|
| `English.json` | V Rising localized text strings (33K+ lines) — reference for NameKey/DescKey GUIDs |
| `unused/` | Placeholder |
| `Unsorted/` | Placeholder |

---

## LilithsHeart (Server Plugin)

### Root

| File | Class | Purpose |
|------|-------|---------|
| `HeartPlugin.cs` | `HeartPlugin : BasePlugin` | BepInEx entry point — Load() initializes config/eventbus/registry/Harmony, Unload() tears down |
| `LilithsHeart.csproj` | — | Net6.0, references Mind, VampireReferenceAssemblies, VCF |

### Foundation/

| File | Class | Purpose |
|------|-------|---------|
| `Heart.cs` | `Heart` | **Central server access point.** Static class: provides `EntityManager`, `PrefabCollectionSystem`, `GameDataSystem`. Manages initialization lifecycle, pending override dictionaries, module registration API (`RegisterRecipeOverrides`, `RegisterStationRecipeChanges`, `RegisterPlayerRecipeChanges`). |
| `HeartLogger.cs` | `HeartLogger` | Static logging wrapper around `ManualLogSource`. `Debug()` short-circuits when `HeartConfig.IsDebug` is false. |
| `EntityExtensions.cs` | `EntityExtensions` | Extension methods on `Entity`: `With<T>()`, `Read<T>()`, `Write<T>()`, `Has<T>()`, `Add<T>()`, `Remove<T>()`, `ReadBuffer<T>()`, `AddBuffer<T>()`, `TryGetBuffer<T>()`, `TryGetComponent<T>()`. |
| `HeartModuleData.cs` | `HeartModuleData` | Module metadata: ModuleId, ModuleName, Version, Capabilities (string[]). Passed to `HeartModuleRegistry.Register()`. |
| `HeartModuleRegistry.cs` | `HeartModuleRegistry` | In-memory dictionary `_modules` keyed by ModuleId. Methods: `Register()`, `IsLoaded()`, `GetModule()`, `GetAllModules()`, `LogSummary()`. Event: `OnModuleRegistered`. |

### Events/

| File | Class | Purpose |
|------|-------|---------|
| `HeartEventIndex.cs` | `OnWorldReady`, `OnWorldDestroyed` | Event struct definitions — fired when ECS world is ready/being torn down |
| `HeartEventBus.cs` | `HeartEventBus` | Generic pub/sub event bus. Thread-safe via lock. Supports `Subscribe<T>`, `SubscribeOnce<T>`, `Unsubscribe<T>`, `Publish<T>`. Snapshot dispatch for safe subscribe/unsubscribe during handlers. |

### Patches/

| File | Class | Purpose |
|------|-------|---------|
| `InitializationPatch.cs` | `InitializationPatch` | Harmony postfix on `WarEventRegistrySystem.RegisterWarEventEntities`. Single-fire — calls `Heart.OnInitialize()`. |
| `ClientConnectPatch.cs` | `ClientConnectPatch` | Harmony postfix on `ServerBootstrapSystem.OnUserConnected`. Resolves user from `_NetEndPointToApprovedUserIndex`, reads User + Character entities, calls `SyncSender.SendSyncToClient()`. |

### Network/

| File | Class | Purpose |
|------|-------|---------|
| `SyncSender.cs` | `SyncSender` | Chunks cached JSON into 450-char messages with `[[LG:N]]` prefix, sends as `ChatMessageServerEvent` with `ServerChatMessageType.System`. Ends with `[[LG:end]]`. Uses `SendEventToUser` for routing. |
| `SyncPayloadCache.cs` | `SyncPayloadCache` | Builds `ServerSyncPayload` DTO, populates from Heart's pending override dictionaries + `LocalizationConfig`, serializes to JSON with SHA256 hash prefix. `Rebuild()` called twice (baseline + after modules register). |

### Services/

| File | Class | Purpose |
|------|-------|---------|
| `PrefabNameResolver.cs` | `PrefabNameResolver` | Scans LilithsMind definition classes via reflection at startup. Builds 3 dictionaries: `_nameToGuid`, `_prefabToGuid`, `_guidToName`. Provides `TryResolve(name)` and `TryResolveName(guid)`. |
| `LocalizationService.cs` | `LocalizationService` | Owns the loading pipeline for localization overrides. Reads all `*.json` from `Localization/` directory, merges into `LocalizationConfig`. Supports `Reload()`. Example file generation is opt-in via `HeartConfig.GenerateLocalizationExample`. |

### Config/

| File | Class | Purpose |
|------|-------|---------|
| `HeartConfig.cs` | `HeartConfig` | BepInEx config bindings: `DebugLogging` (bool), `ServerName` (string), `GenerateLocalizationExample` (bool). No Lazy<T> wrappers — `ConfigEntry<T>.Value` already caches. |
| `HeartPathIndex.cs` | `HeartPathIndex` | Filesystem paths: `Root` (BepInEx/config/LilithsHeart/), `CoreConfig`, `LocalizationDir`, `ModuleConfig()`, `DataDir()`. |
| `LocalizationConfig.cs` | `LocalizationConfig` | **Pure data surface** — holds merged display name/tooltip overrides in two dictionaries. Loading logic lives in `LocalizationService`. Provides `GetDisplayName()`, `GetTooltip()`. Internal mutators: `Clear()`, `AddDisplayName()`, `AddTooltip()`, `MarkLoaded()`. |

---

## LilithsCookbook (Server Plugin)

### Root

| File | Class | Purpose |
|------|-------|---------|
| `CookbookPlugin.cs` | `CookbookPlugin : BasePlugin` | BepInEx entry point. Loads config, registers with HeartModuleRegistry, subscribes to `Heart.OnInitialized`. Stores static `RecipeData` and `StationData`. |
| `LilithsCookbook.csproj` | — | Net6.0, references Heart + Mind, VampireReferenceAssemblies, VCF |

### Systems/

| File | Class | Purpose |
|------|-------|---------|
| `RecipeSystem.cs` | `RecipeSystem` | Applies recipe changes from config to server ECS prefabs + `RecipeHashLookupMap`. Builds `LilithRecipeData` overrides from applied state for Soul sync. Handles: RecipeData scalars, requirements, outputs, repair costs, unit outputs, recipe links. |
| `StationSystem.cs` | `StationSystem` | Two-pass approach: (1) patch prefab entities, (2) after `RegisterGameData()` resets live entities, re-patch live + User entities. Handles both `RefinementstationRecipesBuffer` and `WorkstationRecipesBuffer`. |
| `CookbookLoader.cs` | `CookbookLoader` | Reads and merges all `*.json` files from Recipes/ and Stations/ directories. Later files win on key collision. |
| `CookbookBuilder.cs` | `CookbookBuilder` | Writes example config files on first run. If `GenerateAllRecipes` enabled, dumps all vanilla recipes from `RecipeHashLookupMap` to `all-recipes.json`, then resets the flag. Renamed from `CookbookGenerator` to match Builder naming convention. |

### Data/

| File | Class | Purpose |
|------|-------|---------|
| `CookbookItemData.cs` | `CookbookItemData` | **New** — single reusable item+amount DTO replacing `RecipeRequirement`, `RecipeOutput`, `RecipeRepairCost`, `RecipeUnitOutput`. Fields: `Item` (string), `Amount` (int). |
| `CookbookRecipeData.cs` | `CookbookRecipeData`, `RecipeEntryData` | JSON-deserializable config DTOs. `RecipeEntryData` replaces `RecipeEntry`. Uses `CookbookItemData` for Requirements, Outputs, RepairCosts, UnitOutputs lists. |
| `CookbookStationData.cs` | `CookbookStationData`, `StationEntryData` | JSON-deserializable config DTOs. `StationEntryData` replaces `StationEntry`. |

### Config/

| File | Class | Purpose |
|------|-------|---------|
| `CookbookConfig.cs` | `CookbookConfig` | `GenerateAllRecipes` (bool) flag. Includes `DisableGenerateAllRecipes()` auto-reset. |

---

## LilithsSoul (Client Plugin)

### Root

| File | Class | Purpose |
|------|-------|---------|
| `SoulPlugin.cs` | `SoulPlugin : BasePlugin` | BepInEx entry point for client. Loads config, Harmony patches. |
| `LilithsSoul.csproj` | — | Net6.0, references Mind, VRising.Unhollowed.Client (no VCF) |

### Foundation/

| File | Class | Purpose |
|------|-------|---------|
| `Soul.cs` | `Soul` | Client-side mirror of Heart. Finds client world by name, provides `EntityManager`. Supports `Reset()` for disconnect. |
| `SoulLogger.cs` | `SoulLogger` | Client logging wrapper — same pattern as HeartLogger |
| `EntityExtensions.cs` | `EntityExtensions` | Same fluent ECS extension methods as Heart (uses Soul.EntityManager) |

### Services/

| File | Class | Purpose |
|------|-------|---------|
| `LocalizationInjector.cs` | `LocalizationInjector` | Scans LilithsMind definitions for NameKey/DescKey. Injects `DisplayNameOverrides` and `TooltipOverrides` into `Localization._LocalizedStrings`. Uses `AssetGuid.FromString()` for parsing. Supports `ClearPrevious()` via `LoadDefaultLanguage()`. |
| `RecipePatcher.cs` | `RecipePatcher` | Builds name→GUID map from PrefabCollectionSystem + LilithsMind definitions. Patches: RecipeData, RecipeHashLookupMap, RecipeRequirementBuffer, RecipeOutputBuffer, WorkstationRecipesBuffer (stations + player). |
| `ServerRegistry.cs` | `ServerRegistry` | `servers.json` file management — maps connection string → folder name. Methods: `Load()`, `TryGetFolderName()`, `Register()`. Persists immediately on register. **Moved from Config/ to Services/.** |

### Patches/

| File | Class | Purpose |
|------|-------|---------|
| `ClientInitPatch.cs` | `ClientInitPatch` | Harmony postfix on `GameDataManager.OnUpdate`. Single-fire — reads `ClientBootstrapSystem.ConnectionString`, calls `SyncReceiver.NotifyWorldReady()`. Supports `Reset()`. |
| `ClientChatSystemPatch.cs` | `ClientChatSystemPatch` | Harmony **prefix** on `ClientChatSystem.OnUpdate`. Filters for `ServerChatMessageType.System`, passes to `SyncReceiver.TryHandleMessage()`. Destroys consumed entities before UI sees them. |

### Network/

| File | Class | Purpose |
|------|-------|---------|
| `SyncReceiver.cs` | `SyncReceiver` | Accumulates `[[LG:N]]` chunks from chat messages. On `[[LG:end]]`: concats, deserializes, writes to disk, applies. Supports pre-apply from cached sync.json. Handles out-of-order arrivals (payload before world ready → pending). |

### Config/

| File | Class | Purpose |
|------|-------|---------|
| `SoulConfig.cs` | `SoulConfig` | `DebugLogging` (bool) |
| `SoulPathIndex.cs` | `SoulPathIndex` | Root, CoreConfig, per-server ServerDir(), SyncFile(). Renamed from `SoulPaths` to match *Index convention. |
