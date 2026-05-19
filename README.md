# LilithsGarden
A modular V Rising mod suite












[LilithsHeart] >
    [Config] >
        HeartConfig.cs
        HeartPaths.cs
        LocalizationConfig.cs
    [Events] >
        HeartEventBus.cs
        HeartEvents.cs
    [Foundation] >
        EntityExtensions.cs
        Heart.cs
        HeartLogger.cs
    [Modules] >
        HeartRegistry.cs
        ModuleInfo.cs
    [Network] >
        ServerSyncPayload.cs
    [Patches] >
        InitializationPatch.cs
    [Prefabs] >
        [Definitions] >
            Equipment.cs
            Items.cs
            PrefabNameAttribute.cs
            Recipes.cs
            Stations.cs
            Unsorted.cs
            Weapons.cs
        PrefabNameEntry.cs
        PrefabNameResolver.cs
        PrefabNameExporter.cs
    HeartPlugin.cs

[LilithsCookbook] >
    [Config] >
        Cookbook.Config.cs
    [Data] >
        CookbookRecipeData.cs
        CookbookStationData.cs
    [Systems] >
        CookbookGenerator.cs
        CookbookLoader.cs
        RecipeSystem.cs
        StationSystem.cs
    CookbookPlugin.cs