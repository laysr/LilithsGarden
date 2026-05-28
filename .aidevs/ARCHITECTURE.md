# Architecture

## Layer Diagram

```
LilithsMind (pure C#, no game deps)
    ├── Prefabs/Definitions/*Index.cs    — static PrefabDef catalog
    ├── Network/*Payload.cs, *Data.cs    — shared DTOs
    └── Resources/English.json           — localized text refs
          │
          ▼
┌──────────────────────────────────────────────┐
│              LilithsHeart (server)             │
│  Foundation/  Events/  Config/  Services/     │
│  Network/     Patches/                        │
│  Plugin entry: HeartPlugin.cs                  │
│  NuGet: VampireReferenceAssemblies, VCF        │
└──────────────────────┬───────────────────────┘
                       │ BepInDependency
                       ▼
┌──────────────────────────────────────────────┐
│           LilithsCookbook (server)            │
│  Systems/     Data/     Config/               │
│  Plugin entry: CookbookPlugin.cs              │
│  Depends on Heart + Mind                      │
└──────────────────────────────────────────────┘

LilithsSoul (client, standalone)
    Foundation/    Services/    Config/
    Network/       Patches/
    Plugin entry: SoulPlugin.cs
    NuGet: VRising.Unholyed.Client
```

## Plugin GUIDs

| Plugin | GUID |
|--------|------|
| LilithsHeart | `audaciousbovine.lilithsheart` |
| LilithsCookbook | `audaciousbovine.lilithscookbook` |
| LilithsSoul | `audaciousbovine.lilithssoul` |

## Heart Initialization Sequence

```
HeartPlugin.Load()
  ├── HeartLogger.Initialize()
  ├── HeartConfig.Initialize()         — reads LilithsHeart.cfg
  ├── HeartEventBus.Initialize()
  ├── HeartModuleRegistry.Initialize()
  └── Harmony.PatchAll()
        │
        ▼  (world loads)
InitializationPatch.Postfix()          — hooks WarEventRegistrySystem
  └── Heart.OnInitialize()
        ├── PrefabNameResolver.Initialize()     — scans LilithsMind definitions via reflection
        ├── LocalizationService.Initialize()    — reads localization/*.json
        ├── Build baseline ServerSyncPayload (empty overrides)
        ├── _initialized = true
        ├── Fire OnInitialized event
        │     └── CookbookPlugin.OnHeartInitialized()
        │           ├── CookbookBuilder.GenerateAllRecipesIfRequested()
        │           ├── CookbookLoader.LoadRecipes()
        │           ├── CookbookLoader.LoadStations()
        │           ├── RecipeSystem.ApplyChanges()
        │           └── StationSystem.ApplyChanges()
        ├── Rebuild payload with accumulated overrides
        ├── HeartModuleRegistry.LogSummary()
        └── HeartEventBus.Publish(OnWorldReady)
```

## Client Connect Sequence

```
Client connects to server
  └── ServerBootstrapSystem.OnUserConnected
        └── ClientConnectPatch.Postfix()
              ├── Resolve userIndex from _NetEndPointToApprovedUserIndex
              ├── Read User + Character entities
              └── SyncSender.SendSyncToClient(userEntity, characterEntity, userIndex)
                    ├── Read CachedJson from SyncPayloadCache
                    ├── Chunkify() — split into 450-char pieces
                    ├── Send each chunk as ChatMessageServerEvent with prefix [[LG:N]]
                    └── Send [[LG:end]] sentinel
```

## Soul Initialization Sequence

```
SoulPlugin.Load()
  ├── SoulLogger.Initialize()
  ├── SoulConfig.Initialize()
  └── Harmony.PatchAll()
        │
        ▼  (client world loads)
ClientInitPatch.Postfix()             — hooks GameDataManager.OnUpdate
  └── SyncReceiver.NotifyWorldReady(connectionString)
        ├── LocalizationInjector.BuildLookupTable()
        ├── RecipePatcher.BuildNameMap()
        ├── TryPreApplyCachedSync(connectionString)
        │     ├── ServerRegistry.Load() — reads servers.json
        │     ├── Look up connectionString → folderName
        │     ├── Read sync.json from disk
        │     ├── Deserialize ServerSyncPayload
        │     └── ApplyPayload() — UI race condition fix
        └── If pendingPayload: ApplyPayload()

ClientChatSystemPatch.Prefix()        — per-frame
  └── For each ChatMessageServerEvent.System:
        ├── SyncReceiver.TryHandleMessage()
        │     ├── [[LG:N]] → accumulate chunk
        │     ├── [[LG:end]] → ProcessAccumulatedChunks()
        │     │     ├── Concat chunks, deserialize
        │     │     ├── WriteToDiskIfChanged()
        │     │     └── ApplyPayload()
        └── If consumed → DestroyEntity (hides from chat UI)
```

## Payload Application Order

```
ApplyPayload(ServerSyncPayload)
  1. LocalizationInjector.Inject(payload)
     └── ClearPrevious() — LoadDefaultLanguage()
     └── Write DisplayNameOverrides → Localization._LocalizedStrings
     └── Write TooltipOverrides → Localization._LocalizedStrings
  2. RecipePatcher.Apply(payload.RecipeOverrides)
     └── For each recipe: Patch RecipeData, RecipeHashLookupMap,
                           RecipeRequirementBuffer, RecipeOutputBuffer
  3. RecipePatcher.ApplyStationRecipes(payload.StationRecipeOverrides)
     └── For each station: Add/Remove WorkstationRecipesBuffer entries
  4. RecipePatcher.ApplyPlayerRecipes(payload.PlayerRecipesToAdd, ...)
     └── Patch local player User entity WorkstationRecipesBuffer
```

## Config File Layout (Server)

```
BepInEx/config/LilithsHeart/
  ├── LilithsHeart.cfg           — DebugLogging, ServerName
  ├── LilithsCookbook.cfg        — GenerateAllRecipes
  ├── Localization/              — *.json display name/tooltip overrides
  ├── Recipes/                   — *.json recipe config files
  └── Stations/                  — *.json station config files
```

## Config File Layout (Client)

```
BepInEx/config/LilithsSoul/
  ├── LilithsSoul.cfg            — DebugLogging
  ├── servers.json               — connection string → folder name mapping
  └── <ServerIdentity>/sync.json — cached ServerSyncPayload per server
```

## Module Registration Pattern

Child modules (like Cookbook) register with HeartModuleRegistry in their `Load()`:

```csharp
HeartModuleRegistry.Register(new HeartModuleData
{
    ModuleId   = "audaciousbovine.lilithscookbook",
    ModuleName = "LilithsCookbook",
    Version    = "0.1.0",
});
```

They subscribe to `Heart.OnInitialized` to apply changes after Heart is ready, then call Heart's `Register*()` methods to queue overrides for the sync payload.
