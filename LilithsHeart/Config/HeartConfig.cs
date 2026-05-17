using BepInEx.Configuration;

namespace LilithsHeart.Config;

public static class HeartConfig
{
    private const string LOG_SOURCE = "LilithsHeart.HeartConfig";

    // Raw BepInEx config entries — these are bound to the config file.
    // Kept private so consumers use the clean Lazy<T> properties below.
static ConfigEntry<int>   _startingInventorySize                 = null!;
static ConfigEntry<float> _globalPlayerMovementSpeedMultiplier   = null!;
static ConfigEntry<bool>  _debugLogging                          = null!;

    // Lazy<T> accessors — values are not read until first access.
    // [PERFORMANCE] Deferred reads mean no config file I/O at startup
    //               beyond the initial Bind() calls in Initialize().
    static readonly Lazy<int>   _startingInventorySizeLazy                  = new(() => _startingInventorySize.Value);
    static readonly Lazy<float> _globalPlayerMovementSpeedMultiplierLazy    = new(() => _globalPlayerMovementSpeedMultiplier.Value);
    static readonly Lazy<bool>  _debugLoggingLazy                           = new(() => _debugLogging.Value);

    // Public accessors — these are what the rest of the codebase uses.
    public static int   StartingInventorySize                => _startingInventorySizeLazy.Value;
    public static float GlobalPlayerMovementSpeedMultiplier  => _globalPlayerMovementSpeedMultiplierLazy.Value;
    public static bool  IsDebug                              => _debugLoggingLazy.Value;

    public static void Initialize(ConfigFile config)
    {
        _startingInventorySize = config.Bind(
            section:      "Player",
            key:          "StartingInventorySize",
            defaultValue: 20,
            description:  "Number of inventory slots a new character starts with. Vanilla default is 20."
        );

        _globalPlayerMovementSpeedMultiplier = config.Bind(
            section:      "Player",
            key:          "GlobalMovementSpeedMultiplier",
            defaultValue: 1.0f,
            description:  "Multiplier applied to all player movement speeds. " +
                          "1.0 = vanilla speed. 1.5 = 50% faster. Applied on spawn and relog."
        );

        _debugLogging = config.Bind(
            section:      "Core",
            key:          "DebugLogging",
            defaultValue: false,
            description:  "Enable verbose debug logging for LilithsHeart. " +
                          "Useful during development, disable on live servers."
        );

        LilithsLogger.Info(LOG_SOURCE, $"HeartConfig loaded. " +
            $"StartingInventorySize={StartingInventorySize}, " +
            $"MovementMultiplier={GlobalPlayerMovementSpeedMultiplier}, " +
            $"Debug={IsDebug}");
    }
}