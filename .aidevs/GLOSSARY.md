# Glossary

## Project Terminology

| Term | Definition |
|------|------------|
| **LilithsGarden** | The overall mod suite name. Thematic naming: Heart (server core), Soul (client core), Mind (shared knowledge), Cookbook (recipe module). |
| **Heart** | `LilithsHeart` — server-side plugin that manages ECS access, module registration, and sync payload delivery. |
| **Soul** | `LilithsSoul` — client-side plugin that intercepts chat messages, patches local ECS entities, and injects localization. |
| **Mind** | `LilithsMind` — shared C# library with zero game dependencies. Holds prefab definitions and network DTOs. |
| **Cookbook** | `LilithsCookbook` — server-side child module of Heart that reads JSON config files and applies recipe/station changes. |
| **Child Module** | A BepInEx plugin that depends on Heart (`[BepInDependency("audaciousbovine.lilithsheart")]`) and registers via `HeartModuleRegistry`. |

## V Rising / ECS Terminology

| Term | Definition |
|------|------------|
| **ECS** | Entity Component System — Unity's DOTS architecture. V Rising uses this internally. Entities are IDs, Components are data structs, Systems process them. |
| **Entity** | An `Entity` struct — a lightweight ID referencing a collection of components in the ECS world. |
| **Prefab** | A template Entity stored in `PrefabCollectionSystem._PrefabGuidToEntityMap`. Recipes, items, stations, etc. all have prefab entities that define their base state. |
| **PrefabGUID** | `Stunlock.Core.PrefabGUID` — wraps a single `int` (`_Value`) identifying a prefab. |
| **AssetGuid** | `Stunlock.Core.AssetGuid` — a GUID type used as the key in `Localization._LocalizedStrings`. Maps to NameKey/DescKey strings in PrefabDef. |
| **Component** | A struct implementing `IComponentData` attached to entities. Examples: `RecipeData`, `NetworkId`, `User`. |
| **DynamicBuffer** | `Unity.Entities.DynamicBuffer<T>` — a resizable array buffer component. Examples: `RecipeRequirementBuffer`, `WorkstationRecipesBuffer`. |
| **World** | `Unity.Entities.World` — a container for entities and systems. V Rising has separate server and client worlds. |
| **EntityManager** | `Unity.Entities.EntityManager` — the API for creating, reading, writing, and destroying entities and components. |
| **System** | A class that processes entities. Accessible via `World.GetExistingSystemManaged<T>()`. |
| **GameDataSystem** | V Rising's system holding `RecipeHashLookupMap` — a dictionary mapping `PrefabGUID → RecipeData` that the crafting UI reads. |
| **PrefabCollectionSystem** | V Rising's system holding `_PrefabGuidToEntityMap` (PrefabGUID → Entity) and `_PrefabDataLookup` (PrefabGUID → PrefabData). |
| **RecipeHashLookupMap** | A `NativeHashMap<PrefabGUID, RecipeData>` in `GameDataSystem`. Crafting reads scalar fields from this map, not from entity components. |
| **WorkstationRecipesBuffer** | `DynamicBuffer<WorkstationRecipesBuffer>` — defines which recipes appear at a crafting station or for a player. |
| **RefinementstationRecipesBuffer** | `DynamicBuffer<RefinementstationRecipesBuffer>` — defines recipes for automatic refinement stations (Furnace, Grinder). |

## Sync Protocol Terminology

| Term | Definition |
|------|------------|
| **ServerSyncPayload** | The main data contract sent from Heart to Soul on client connect. Contains localization overrides, recipe overrides, station changes, and player recipe changes. |
| **ServerEventPayload** | A future payload type for in-session events (not yet implemented). Uses `EventKind` for routing. |
| **PayloadHash** | First 8 hex characters of SHA256 hash of the serialized payload. Used by Soul to skip redundant disk writes and re-injection on reconnect. |
| **Chunk** | A 450-character fragment of the JSON payload, sent as a `ChatMessageServerEvent` with `[[LG:N]]` prefix. |
| **[[LG:end]]** | Sentinel message telling Soul the payload is complete and ready to reassemble. |
| **ChatMessageServerEvent** | V Rising's network event type for system chat messages. Used as transport because Unity Netcode is unavailable in IL2CPP. |
| **ServerIdentity** | The sanitized server name from `HeartConfig.ServerName`. Used as a folder name on the client for per-server cached data. |

## Config / Data Terminology

| Term | Definition |
|------|------------|
| **PrefabDef** | A `readonly record struct` in LilithsMind defining a single prefab's metadata (Name, GuidHash, Prefab, NameKey, DescKey). |
| **NameKey** | A GUID string (e.g. `"37e872e1-4aa1-4f0a-8e2e-a67883b5a645"`) that maps to a display name in `Localization._LocalizedStrings`. |
| **DescKey** | A GUID string that maps to a tooltip/description in `Localization._LocalizedStrings`. |
| **CookbookRecipeData** | Deserialized from `Recipes/*.json`. Contains `Dictionary<string, RecipeEntryData>`. Uses `CookbookItemData` for requirements, outputs, repair costs, and unit outputs. |
| **RecipeEntryData** | A config DTO with scalar fields (`CraftDuration`, `AlwaysUnlocked`, etc.) and optional buffer lists (`Requirements`, `Outputs`, `RepairCosts`, `UnitOutputs`, `RecipeLinks`). Renamed from `RecipeEntry`. |
| **CookbookStationData** | Deserialized from `Stations/*.json`. Contains `Dictionary<string, StationEntryData>`. |
| **StationEntryData** | A config DTO with `ChangesEnabled`, `AddRecipes`, and `RemoveRecipes`. Renamed from `StationEntry`. |
| **CookbookItemData** | A single item+amount DTO (`Item` string, `Amount` int) used across all recipe slot contexts (requirements, outputs, repair costs, unit outputs). Consolidates the previous `RecipeRequirement`, `RecipeOutput`, `RecipeRepairCost`, and `RecipeUnitOutput` classes. |
| **LilithRecipeData** | Network DTO in `ServerSyncPayload.RecipeOverrides` — contains `CraftDuration`, `Requirements` (Dictionary<string,int>), and `Outputs` (Dictionary<string,int>). |
| **LilithStationData** | Network DTO in `ServerSyncPayload.StationRecipeOverrides` — contains `RecipesToAdd` and `RecipesToRemove`. |

## Development / Code Organization

| Term | Definition |
|------|------------|
| **`[CHANGED]`** | Inline comment marker documenting changes from previous code iterations. Essential for understanding evolution. |
| **`[PERFORMANCE]`** | Inline comment marker documenting performance characteristics and O-notation of operations. |
| **Harmony Patch** | A `[HarmonyPrefix]` or `[HarmonyPostfix]` method that injects code before or after a game method. Named `*Patch.cs`. |
| **Single-fire Guard** | A `static bool _initialized` field that prevents a Harmony patch from executing more than once (e.g., world init detection). |
| **`using static`** | Not used in this codebase. All usages are explicit. |

## EventKind Range Reservation

| Range | Module |
|-------|--------|
| 0–99 | Core (reserved) |
| 100–199 | LilithsCookbook |
| 200–299 | LilithsBounty |
| 300–399 | LilithsTreasury |
| 400–499 | LilithsMachinations |
