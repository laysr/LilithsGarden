using BepInEx.Logging;
using LilithsHeart.Config;

namespace LilithsHeart;

public static class LilithsLogger
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

    // [CHANGED] Debug guard restored now that HeartConfig is written.
    //           Zero cost when debug is off — no string allocation or write operations.
    // [PERFORMANCE] Short-circuits immediately if IsDebug is false.
    public static void Debug(string source, string message)
    {
        if (HeartConfig.IsDebug)
            _logger.LogDebug($"[{source}] {message}");
    }
}