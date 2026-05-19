using BepInEx.Configuration;
using LilithsHeart.Foundation;

namespace LilithsHeart.Config;

// [CHANGED] Removed Lazy<T> wrappers from all config accessors.
//           BepInEx's ConfigEntry<T>.Value already caches the parsed value
//           after the first read. Lazy<T> on top of that added no benefit and
//           introduced a hot-reload bug: once evaluated, Lazy<T> never
//           re-reads even if the ConfigFile is reloaded at runtime.
//           Reading .Value directly matches the pattern used in CookbookConfig
//           and is safe — no file I/O occurs after Initialize().
//
// [CHANGED] Removed StartingInventorySize and GlobalPlayerMovementSpeedMultiplier.
//           HeartConfig is infrastructure-only. Gameplay settings belong in the
//           module that owns the feature, not in the shared core. Those two
//           settings will move to the appropriate gameplay module when built.
public static class HeartConfig
{
    private const string LOG_SOURCE = "LilithsHeart.HeartConfig";

    static ConfigEntry<bool> _debugLogging = null!;

    public static bool IsDebug => _debugLogging.Value;

    public static void Initialize(ConfigFile config)
    {
        _debugLogging = config.Bind(
            section:      "Core",
            key:          "DebugLogging",
            defaultValue: false,
            description:  "Enable verbose debug logging for LilithsHeart. " +
                          "Useful during development, disable on live servers."
        );

        HeartLogger.Info(LOG_SOURCE, $"HeartConfig loaded. Debug={IsDebug}");
    }
}