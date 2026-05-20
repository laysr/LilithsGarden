using BepInEx.Logging;
using LilithsSoul.Config;

// ============================================================
//  SoulLogger — LilithsSoul
//
//  Thin wrapper around BepInEx ManualLogSource.
//  Mirrors HeartLogger exactly — same pattern, Soul namespace.
//
//  All Soul and child client-module logging flows through here.
//  Child modules call SoulLogger directly (no dependency on Heart).
//
//  [PERFORMANCE] Debug() short-circuits immediately when debug
//                is disabled — no string allocation or write.
// ============================================================

namespace LilithsSoul.Foundation;

public static class SoulLogger
{
    private static ManualLogSource _logger = null!;

    internal static void Initialize(ManualLogSource logger)
    {
        _logger = logger;
    }

    public static void Info(string source, string message)
        => _logger.LogInfo($"[{source}] {message}");

    public static void Warning(string source, string message)
        => _logger.LogWarning($"[{source}] {message}");

    public static void Error(string source, string message)
        => _logger.LogError($"[{source}] {message}");

    public static void Debug(string source, string message)
    {
        if (SoulConfig.IsDebug)
            _logger.LogDebug($"[{source}] {message}");
    }
}