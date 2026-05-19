// ============================================================
//  HeartPaths — LilithsHeart
//
//  Single source of truth for every filesystem path used by
//  LilithsHeart and its child modules.
//
//  All config for the entire suite lives under one root:
//      BepInEx/config/LilithsHeart/
//
//  Structure:
//      LilithsHeart/
//          LilithsHeart.cfg            ← Heart core settings
//          LilithsCookbook.cfg         ← child module cfg (example)
//          LilithsBounty.cfg           ← child module cfg (example)
//          Names/
//              *.json                  ← prefab name export files
//          Localization/
//              overrides.json          ← server localization overrides
//          Recipes/                    ← future: recipe config data
//          Stations/                   ← future: station config data
//          Buffs/                      ← future: buff config data
//
//  Child modules should use HeartPaths to resolve all paths
//  rather than building their own path strings. This keeps
//  the layout consistent and refactorable from one place.
//
//  Usage in a child module's Plugin.Load():
//
//      var cfg = new ConfigFile(HeartPaths.ModuleConfig("LilithsCookbook"), saveOnInit: true);
//
//  Usage for a data subfolder:
//
//      var recipesDir = HeartPaths.DataDir("Recipes");
//
// ============================================================

namespace LilithsHeart;

public static class HeartPaths
{
    // ── Root ────────────────────────────────────────────────

    /// <summary>
    /// BepInEx/config/LilithsHeart/
    /// All suite config lives under this directory.
    /// </summary>
    public static readonly string Root = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsHeart"
    );

    // ── .cfg files ──────────────────────────────────────────

    /// <summary>
    /// BepInEx/config/LilithsHeart/LilithsHeart.cfg
    /// The core Heart config file. Pass to HeartConfig.Initialize().
    /// </summary>
    public static readonly string CoreConfig = Path.Combine(Root, "LilithsHeart.cfg");

    /// <summary>
    /// Returns the path for a child module's .cfg file.
    /// e.g. HeartPaths.ModuleConfig("LilithsCookbook")
    ///      → BepInEx/config/LilithsHeart/LilithsCookbook.cfg
    ///
    /// Pass the result directly to: new ConfigFile(path, saveOnInit: true)
    /// </summary>
    public static string ModuleConfig(string moduleName)
        => Path.Combine(Root, $"{moduleName}.cfg");

    // ── Data subdirectories ─────────────────────────────────

    /// <summary>
    /// BepInEx/config/LilithsHeart/Names/
    /// Prefab name export JSON files written by PrefabNamesExporter.
    /// </summary>
    public static readonly string NamesDir = Path.Combine(Root, "Names");

    /// <summary>
    /// BepInEx/config/LilithsHeart/Localization/
    /// Server localization override files.
    /// </summary>
    public static readonly string LocalizationDir = Path.Combine(Root, "Localization");

    /// <summary>
    /// Returns the path for a named data subdirectory.
    /// e.g. HeartPaths.DataDir("Recipes")
    ///      → BepInEx/config/LilithsHeart/Recipes/
    ///
    /// The directory is NOT created here — call Directory.CreateDirectory()
    /// at the point of first use so empty folders are not left on disk
    /// for features that are never exercised.
    /// </summary>
    public static string DataDir(string category)
        => Path.Combine(Root, category);

    // ── Specific file paths ─────────────────────────────────

    // [CHANGED] Removed LocalizationOverrides single-file path.
    //           LocalizationConfig now scans the whole LocalizationDir
    //           for any *.json file, matching the Names/ directory pattern.
}