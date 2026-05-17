using BepInEx.Logging;

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

    // [CHANGED] Debug guard removed temporarily until HeartConfig is written.
    //           Will be re-added when HeartConfig is in place.
    public static void Debug(string source, string message)
        => _logger.LogDebug($"[{source}] {message}");
}