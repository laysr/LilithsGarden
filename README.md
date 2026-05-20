# LilithsGarden
A modular V Rising mod suite











```
[LilithsHeart] >
    [Config] >
        HeartConfig.cs - Config file for LilithsHeart
        HeartPaths.cs - Paths for where files should go, all modules get their paths from here
        LocalizationConfig.cs - Loads Localization files from Config>Localization> *.json files on the server
    [Events] >
        HeartEventBus.cs - Inter module communicator, announces Events, Modules subscribe to listen
        HeartEvents.cs - defines Event Types, when something happens, this listens
    [Foundation] >
        EntityExtensions.cs - Extensions to more easily interact with entities in code
        Heart.cs - Accesses World Data, allows other scripts to run after initialization
        HeartLogger.cs - Logging tool for console messages, all modules communicate with this
    [Modules] >
        HeartRegistry.cs - Tracks what modules are loaded
        ModuleInfo.cs - Data that holds info on Modules installed, versions, compatibility
    [Network] >
        ServerSyncPayload.cs - Data bundle that will be sent to the client to communicate with LilithsSoul
    [Patches] >
        InitializationPatch.cs - Adds in our changes on startup through Harmony Patching
    [Prefabs] >
        [Definitions] >
            Equipment.cs - GUIDs
            Items.cs - GUIDs
            PrefabNameAttribute.cs - adds a value to prefabGUIDs that holds their set alias
            Recipes.cs - GUIDs
            Stations.cs - GUIDs
            Unsorted.cs - unsorted GUIDs
            Weapons.cs - GUIDs
        PrefabNameEntry.cs - Data holding the OriginalName and NewName for prefabGUIDs allowing for renaming
        PrefabNameResolver.cs - Reads Names Exported and looks up the GUIDs when theyre used in configs
        PrefabNameExporter.cs - Reads Names > *.json configs on startup to see what GUID has what configured name
    HeartPlugin.cs - ENTRY POINT, Does the initial loads of patches, logger, config, eventbus, registry

[LilithsCookbook] >
    [Config] >
        CookbookConfig.cs - Config File
    [Data] >
        CookbookRecipeData.cs - model for how recipe.jsons are built
        CookbookStationData.cs - model for how station.jsons are built
    [Systems] >
        CookbookGenerator.cs - Creates configs, examples or an all recipe dump
        CookbookLoader.cs - reads all Recipes/Stations > *.json server configs
        RecipeSystem.cs - applies recipe changes
        StationSystem.cs - applies station changes
    CookbookPlugin.cs - ENTRY POINT
```