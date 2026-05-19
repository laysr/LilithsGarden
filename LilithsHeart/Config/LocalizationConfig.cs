using System.Text.Json;
using LilithsHeart.Foundation;

// ============================================================
//  LocalizationConfig — LilithsHeart
//
//  Reads server-defined display name and tooltip overrides from
//  ALL *.json files found in:
//      BepInEx/config/LilithsHeart/Localization/
//
//  Files are loaded and merged in alphabetical order.
//  Later files win on key conflicts, so admins can layer
//  overrides (e.g. base.json + seasonal_event.json).
//
//  Format of each file:
//  {
//    "_readme": "...",                        ← skipped (non-object value)
//    "Item_BloodEssence_T01":        { "DisplayName": "Vitae",     "Tooltip": "..." },
//    "Item_Ingredient_Plant_Cotton": { "DisplayName": "Moonweave", "Tooltip": null }
//  }
//
//  [PERFORMANCE] All files are read once at world ready and merged
//                into two flat dictionaries for O(1) lookup.
//                No file I/O occurs after initialization.
// ============================================================

namespace LilithsHeart.Config;

// [CHANGED] LilithsLogger → HeartLogger throughout.
public static class LocalizationConfig
{
    private const string LOG_SOURCE = "LilithsHeart.LocalizationConfig";

    static readonly Dictionary<string, string> _displayNames = new();
    static readonly Dictionary<string, string> _tooltips     = new();

    public static IReadOnlyDictionary<string, string> DisplayNames => _displayNames;
    public static IReadOnlyDictionary<string, string> Tooltips     => _tooltips;

    public static bool IsLoaded { get; private set; }

    public static void Initialize()
    {
        Directory.CreateDirectory(HeartPaths.LocalizationDir);
        EnsureExampleFile();
        Load();
    }

    public static void Reload()
    {
        _displayNames.Clear();
        _tooltips.Clear();
        IsLoaded = false;

        HeartLogger.Info(LOG_SOURCE, "Reloading localization overrides...");
        Load();
    }

    public static string? GetDisplayName(string prefabName)
        => _displayNames.TryGetValue(prefabName, out var v) ? v : null;

    public static string? GetTooltip(string prefabName)
        => _tooltips.TryGetValue(prefabName, out var v) ? v : null;

    // ── Internal ────────────────────────────────────────────

    static void Load()
    {
        var files = Directory.GetFiles(HeartPaths.LocalizationDir, "*.json")
                             .OrderBy(f => f)
                             .ToArray();

        if (files.Length == 0)
        {
            HeartLogger.Info(LOG_SOURCE, "No localization JSON files found. Overrides disabled.");
            IsLoaded = true;
            return;
        }

        foreach (var file in files)
            LoadFile(file);

        IsLoaded = true;
        HeartLogger.Info(LOG_SOURCE,
            $"Loaded {_displayNames.Count} display name(s) and {_tooltips.Count} tooltip(s) " +
            $"from {files.Length} file(s).");
    }

    static void LoadFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);

            // Deserialize into Dictionary<string, JsonElement> first rather than
            // directly into Dictionary<string, LocalizationEntry>.
            // The example file may contain a "_readme" key whose value is a plain
            // string, not an object. By landing in JsonElement first we can inspect
            // each value's Kind before attempting conversion — non-Object entries
            // are skipped cleanly without throwing.
            var raw = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (raw == null)
            {
                HeartLogger.Warning(LOG_SOURCE,
                    $"'{Path.GetFileName(filePath)}' parsed as null — check for malformed JSON.");
                return;
            }

            int nameCount    = 0;
            int tooltipCount = 0;

            foreach (var (key, element) in raw)
            {
                if (element.ValueKind != JsonValueKind.Object) continue;

                string? displayName = null;
                string? tooltip     = null;

                if (element.TryGetProperty("DisplayName", out var dn) &&
                    dn.ValueKind == JsonValueKind.String)
                    displayName = dn.GetString();

                if (element.TryGetProperty("Tooltip", out var tt) &&
                    tt.ValueKind == JsonValueKind.String)
                    tooltip = tt.GetString();

                if (!string.IsNullOrWhiteSpace(displayName))
                {
                    _displayNames[key] = displayName!;
                    nameCount++;
                }

                if (!string.IsNullOrWhiteSpace(tooltip))
                {
                    _tooltips[key] = tooltip!;
                    tooltipCount++;
                }
            }

            HeartLogger.Info(LOG_SOURCE,
                $"Loaded '{Path.GetFileName(filePath)}' — " +
                $"{nameCount} name(s), {tooltipCount} tooltip(s).");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to load '{Path.GetFileName(filePath)}': {ex.Message}");
        }
    }

    static void EnsureExampleFile()
    {
        var examplePath = Path.Combine(HeartPaths.LocalizationDir, "example-overrides.json");
        if (File.Exists(examplePath)) return;

        var example = new Dictionary<string, object>
        {
            ["_readme"] = "Keys are prefab names. Set DisplayName and/or Tooltip to override vanilla strings. " +
                          "Set ChangesEnabled to true to apply. Files are merged alphabetically — later files win.",
            ["Item_BloodEssence_T01"] = new { DisplayName = "Vitae", Tooltip = (string?)null }
        };

        try
        {
            var json = JsonSerializer.Serialize(example, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(examplePath, json);
            HeartLogger.Info(LOG_SOURCE, "Generated example-overrides.json.");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to write example-overrides.json: {ex.Message}");
        }
    }
}