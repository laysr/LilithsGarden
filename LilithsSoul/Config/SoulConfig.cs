using BepInEx.Configuration;
using LilithsSoul.Foundation;

// ============================================================
//  SoulConfig — LilithsSoul
//
//  BepInEx config bindings for the Soul core.
//  Follows the same pattern as HeartConfig — minimal,
//  infrastructure-only. Client-facing feature settings belong
//  in the child client module that owns the feature.
// ============================================================

namespace LilithsSoul.Config;

public static class SoulConfig
{
    private const string LOG_SOURCE = "LilithsSoul.SoulConfig";

    static ConfigEntry<bool> _debugLogging = null!;

    public static bool IsDebug => _debugLogging.Value;

    public static void Initialize(ConfigFile config)
    {
        _debugLogging = config.Bind(
            section:      "Core",
            key:          "DebugLogging",
            defaultValue: false,
            description:  "Enable verbose debug logging for LilithsSoul. " +
                          "Useful during development, disable in production."
        );

        SoulLogger.Info(LOG_SOURCE, $"SoulConfig loaded. Debug={IsDebug}");
    }
}