using System.Text.Json;
using Stunlock.Core;

namespace LilithsHeart.Systems;

public static class PrefabNameResolver
{
    static readonly Dictionary<string, PrefabGUID> _configNameToGuid = new();
    static readonly Dictionary<string, PrefabGUID> _nameToGuid = new();

    static readonly string ConfigDir = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsHeart"
    );

    public static void Initialize()
    {
        LoadPrefabNames("prefab-names-items.json");
        LoadPrefabNames("prefab-names-recipes.json");
        LoadPrefabNames("prefab-names-stations.json");

        LilithsLogger.Info($"PrefabNameResolver initialized with {_configNameToGuid.Count} entries.");
    }

    static void LoadPrefabNames(string fileName)
    {
        var path = Path.Combine(ConfigDir, fileName);

        if (!File.Exists(path))
        {
            LilithsLogger.Warning($"{fileName} not found, skipping.");
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            var entries = JsonSerializer.Deserialize<Dictionary<string, PrefabNameEntry>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (entries == null) return;

            foreach (var (guidStr, entry) in entries)
            {
                if (!int.TryParse(guidStr, out int guidValue)) continue;

                var guid = new PrefabGUID(guidValue);

                if (!string.IsNullOrEmpty(entry.Name))
                    _nameToGuid[entry.Name] = guid;

                if (!string.IsNullOrEmpty(entry.ConfigName))
                    _configNameToGuid[entry.ConfigName] = guid;
            }
        }
        catch (Exception e)
        {
            LilithsLogger.Error($"Failed to load {fileName}: {e.Message}");
        }
    }

    public static bool TryResolve(string name, out PrefabGUID guid)
    {
        if (_configNameToGuid.TryGetValue(name, out guid))
            return true;

        if (_nameToGuid.TryGetValue(name, out guid))
            return true;

        guid = new PrefabGUID(0);
        LilithsLogger.Warning($"Could not resolve prefab name: {name}");
        return false;
    }
}

public class PrefabNameEntry
{
    public string Name { get; set; } = string.Empty;
    public string ConfigName { get; set; } = string.Empty;
}