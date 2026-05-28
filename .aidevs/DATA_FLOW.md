# Data Flow

## ServerSyncPayload (Primary Data Contract)

The `ServerSyncPayload` class in `LilithsMind/Network/ServerSyncPayload.cs` is the core data contract sent from Heart (server) to Soul (client).

### Structure

```
ServerSyncPayload
├── ServerIdentity: string           — Sanitized server name (used as folder name)
├── PayloadHash: string              — First 8 hex chars of SHA256 (change detection)
├── DisplayNameOverrides: Dictionary<string, string>
│     Key: prefab Name or Prefab string
│     Value: custom display name text
├── TooltipOverrides: Dictionary<string, string>
│     Key: prefab Name or Prefab string
│     Value: custom tooltip text
├── RecipeOverrides: Dictionary<string, LilithRecipeData>
│     Key: recipe prefab name
│     Value: { CraftDuration, Requirements: {item→amount}, Outputs: {item→amount} }
├── StationRecipeOverrides: Dictionary<string, LilithStationData>
│     Key: station prefab name
│     Value: { RecipesToAdd: string[], RecipesToRemove: string[] }
├── PlayerRecipesToAdd: List<string>
└── PlayerRecipesToRemove: List<string>
```

### Build Pipeline (Server Side)

```
CookbookLoader reads JSON config files
  → RecipeSystem / StationSystem apply changes to ECS
  → Heart.RegisterRecipeOverrides() / RegisterStationRecipeChanges() / RegisterPlayerRecipeChanges()

Heart.OnInitialize():
  1. Build baseline payload (empty overrides)
  2. Fire OnInitialized → modules apply changes + register overrides
  3. Rebuild payload with accumulated overrides

SyncPayloadCache.Rebuild():
  ServerSyncPayload = {
    ServerIdentity       = HeartConfig.ServerName (sanitized),
    DisplayNameOverrides = LocalizationConfig.DisplayNames dict,
    TooltipOverrides     = LocalizationConfig.Tooltips dict,
    RecipeOverrides      = Heart._pendingRecipeOverrides,
    StationRecipeOverrides = Heart._pendingStationRecipeOverrides,
    PlayerRecipesToAdd   = Heart._pendingPlayerRecipesToAdd,
    PlayerRecipesToRemove = Heart._pendingPlayerRecipesToRemove
  }
  Serialize, Compute SHA256 hash → store as CachedJson
```

### Transport Protocol (Chat-Based)

```
No Unity Netcode available in IL2CPP → use ChatMessageServerEvent

SyncSender.Chunkify():
  ┌─────────────────────────────┐
  │ Max chunk: 450 chars        │
  │                             │
  │  [[LG:0]]<chunk content>    │
  │  [[LG:1]]<chunk content>    │
  │  ...                        │
  │  [[LG:end]]                 │
  └─────────────────────────────┘

Each chunk sent as:
  ChatMessageServerEvent {
    MessageType = ServerChatMessageType.System,
    FromCharacter = <NetworkId>,
    FromUser = <NetworkId>,
    MessageText = "[[LG:N]]<content>"
  }
  + SendEventToUser { UserIndex = <int> }

Typical payload: 3-5 KB → 10-20 chunks + end sentinel
```

### Receive Pipeline (Client Side)

```
ClientChatSystemPatch.Prefix (per-frame)
  → SyncReceiver.TryHandleMessage(string)
    ├── [[LG:N]] → _chunks.Add(content)  return true (consumed)
    ├── [[LG:end]] → ProcessAccumulatedChunks()
    └── other → return false (pass to chat UI)

ProcessAccumulatedChunks():
  json = string.Concat(_chunks)
  payload = JsonSerializer.Deserialize<ServerSyncPayload>(json)
  ServerRegistry.Register(connectionString, payload.ServerIdentity)
  WriteToDiskIfChanged(payload)  — compares SHA256 hash
  if _clientWorldReady → ApplyPayload(payload)
  else → _pendingPayload = payload

WriteToDiskIfChanged(payload):
  path = BepInEx/config/LilithsSoul/<ServerIdentity>/sync.json
  if exists && existing.PayloadHash == payload.PayloadHash → skip write
  else → write JSON to disk
```

### Pre-Apply (Cached Sync — UI Race Fix)

```
ClientInitPatch detects world ready
  → SyncReceiver.NotifyWorldReady(connectionString)
    → ServerRegistry.Load()  (reads servers.json)
    → ServerRegistry.TryGetFolderName(connectionString)
    → Read sync.json from disk
    → Deserialize
    → ApplyPayload()  — BEFORE CharacterHUD builds
    → Later: server payload arrives → ApplyPayload() again (idempotent if unchanged)
```

---

## ServerEventPayload (In-Session Events)

Not yet implemented, but reserved for future use:

```
ServerEventPayload {
    Kind: EventKind  (int, see range reservation)
    Data: string     (JSON-serialized event-specific data)
}

EventKind Range Reservation:
  0-99     Core
  100-199  LilithsCookbook
  200-299  LilithsBounty
  300-399  LilithsTreasury
  400-499  LilithsMachinations
```

---

## Cookbook Config File Format

### recipes.json

```json
{
  "Recipes": {
    "Recipe_Weapon_Sword_T01_Bone": {
      "ChangesEnabled": true,
      "CraftDuration": 10.0,
      "AlwaysUnlocked": true,
      "HideInStation": false,
      "IgnoreServerSettings": false,
      "HudSortingOrder": 0,
      "Requirements": [
        { "Item": "Item_BloodEssence_T01", "Amount": 5 }
      ],
      "Outputs": [
        { "Item": "Item_Weapon_Sword_T01_Bone", "Amount": 1 }
      ],
      "UseRepairCosts": false,
      "RepairCosts": null,
      "UseUnitOutputs": false,
      "UnitOutputs": null,
      "UseRecipeLinks": false,
      "RecipeLinks": null
    }
  }
}
```

### stations.json

```json
{
  "Stations": {
    "TM_Blacksmith_Stations_Standard": {
      "ChangesEnabled": true,
      "AddRecipes": ["Recipe_Weapon_Sword_T04_Copper_Reinforced"],
      "RemoveRecipes": []
    }
  }
}
```

---

## Localization Override File Format

```json
{
  "_readme": "Keys are prefab Name or Prefab string from LilithsMind PrefabDef entries",
  "Item_BloodEssence_T01": {
    "DisplayName": "Red Marble",
    "Tooltip": "A gorgeous Red Marble dropped from the living..."
  }
}
```

Files load alphabetically — later files win on key conflicts. Requires NameKey (for DisplayName) and DescKey (for Tooltip) to be populated in the corresponding LilithsMind PrefabDef entry.

---

## Key Lookup Chains

### Name → GUID Resolution (Server)

```
Config file name string
  → PrefabNameResolver.TryResolve(name)
    → Check _nameToGuid (Name alias from LilithsMind)
    → If not found, check _prefabToGuid (Prefab string from LilithsMind)
    → Returns PrefabGUID or PrefabGUID(0) on failure
  → PrefabCollectionSystem._PrefabGuidToEntityMap[guid] → Entity
```

### Name → GUID Resolution (Client)

```
Payload name string
  → RecipePatcher._nameToGuid (built from PrefabCollectionSystem + LilithsMind definitions)
    → Built in two passes:
      1. _PrefabDataLookup.AssetName → GUID
      2. LilithsMind PrefabDef.Name fields → GUID
    → Returns PrefabGUID or warning/skip
```

### Name → Localization Key Resolution (Client)

```
Payload prefab name
  → LocalizationInjector._nameToNameGuid / _nameToDescGuid
    → Keyed by both Prefab string and Name alias
    → Value: AssetGuid parsed from LilithsMind NameKey/DescKey
  → Localization._LocalizedStrings[assetGuid] = override text
```
