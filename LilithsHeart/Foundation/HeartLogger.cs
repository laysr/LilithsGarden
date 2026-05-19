using BepInEx.Logging;
using LilithsHeart.Config;

// [CHANGED] Moved from LilithsHeart/ root → Foundation/.
//           Namespace updated: LilithsHeart → LilithsHeart.Foundation.

namespace LilithsHeart.Foundation;

public static class HeartLogger
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

    // [PERFORMANCE] Short-circuits immediately if IsDebug is false —
    //               no string allocation or log write when debug is off.
    public static void Debug(string source, string message)
    {
        if (HeartConfig.IsDebug)
            _logger.LogDebug($"[{source}] {message}");
    }
}