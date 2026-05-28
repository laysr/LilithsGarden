# LilithsGarden — AI Agent Index

> **Agent-agnostic reference.** These docs are designed to be consumed by any AI coding agent (OpenCode, Claude, Codex, Kiro, Cursor, etc.). They describe the codebase structure, conventions, and data flow without assuming any particular tool or workflow.

A modular **V Rising** mod suite that allows server administrators to customize recipes, crafting stations, item names, and tooltips without directly editing game files.

## Quick Reference

| Layer | Project | Role | Dependencies |
|-------|---------|------|-------------|
| **Mind** | `LilithsMind` | Shared library — pure C#, zero game dependencies | none |
| **Heart** | `LilithsHeart` | Server plugin — ECS access, module registration, sync sending | Mind |
| **Soul** | `LilithsSoul` | Client plugin — chat interception, localization injection | Mind |
| **Cookbook** | `LilithsCookbook` | Server plugin — recipe/station config | Heart + Mind |

## Key Files

| File | Purpose |
|------|---------|
| `.aidevs/ARCHITECTURE.md` | System architecture, layering, lifecycle |
| `.aidevs/CODE_MAP.md` | File-by-file index of all classes and responsibilities |
| `.aidevs/CONVENTIONS.md` | Design patterns, naming conventions, coding style |
| `.aidevs/DATA_FLOW.md` | Data flow diagrams and payload formats |
| `.aidevs/PREFAB_INDEX.md` | Prefab definition system reference |
| `.aidevs/GLOSSARY.md` | Domain-specific terminology |

## Tech Stack

- **Language:** C# 12.0, .NET 6.0
- **Mod Framework:** BepInEx 6 (IL2CPP)
- **Patcher:** HarmonyLib
- **ECS:** Unity Entities (DOTS) via V Rising assemblies
- **Server SDK:** VampireReferenceAssemblies v1.1.12
- **Client SDK:** VRising.Unhollowed.Client v1.1.9
- **Commands:** VampireCommandFramework v0.10.4
- **Serialization:** System.Text.Json

## Lifecycle Overview

```
BepInEx Load
  ├─ HeartPlugin.Load()       — config, Harmony patches
  ├─ CookbookPlugin.Load()    — config, register module, subscribe
  └─ SoulPlugin.Load()        — config, Harmony patches

World Ready (WarEventRegistrySystem)
  ├─ Heart.OnInitialize()
  │   ├─ PrefabNameResolver init
  │   ├─ LocalizationConfig init
  │   ├─ Build empty sync payload
  │   ├─ Fire OnInitialized → Cookbook applies changes
  │   ├─ Rebuild payload with overrides
  │   └─ Publish OnWorldReady
  └─ Soul.ClientInitPatch
      ├─ Build localization + recipe lookup tables
      ├─ TryPreApplyCachedSync (from disk)
      └─ Apply pending payload if arrived early

Client Connects (ServerBootstrapSystem)
  └─ ClientConnectPatch → SyncSender sends chunked payload

Client Chat Receive (ClientChatSystem)
  └─ ClientChatSystemPatch → SyncReceiver accumulates chunks
      └─ On [[LG:end]]: deserialize, cache to disk, ApplyPayload

## How to Use These Docs

When an AI agent is asked to work on this codebase, it should first read the relevant `.aidevs/*.md` files to understand the architecture before making changes. The files are designed to be read independently:

| If you need... | Read this first |
|----------------|-----------------|
| Project overview, tech stack, lifecycle | `README.md` |
| System architecture, layer diagram, initialization order | `ARCHITECTURE.md` |
| What every file does, class responsibilities | `CODE_MAP.md` |
| Design patterns, naming rules, coding style | `CONVENTIONS.md` |
| Data flow diagrams, payload formats, lookup chains | `DATA_FLOW.md` |
| Prefab definition system (item database) | `PREFAB_INDEX.md` |
| Domain terminology definitions | `GLOSSARY.md` |
```
