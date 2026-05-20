using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using LilithsSoul.Config;
using LilithsSoul.Foundation;
using LilithsSoul.Network;

// ============================================================
//  SoulPlugin — LilithsSoul
//
//  BepInEx entry point for the LilithsSoul client mod.
//  Mirrors the HeartPlugin pattern — minimal, delegates
//  everything to focused subsystems.
//
//  Load order:
//  ───────────
//  1. SoulLogger   — logging available immediately
//  2. SoulConfig   — reads LilithsSoul.cfg
//  3. Harmony      — patches ClientInitPatch
//  4. SyncReceiver — registers named message handlers
//
//  Note on SyncReceiver.RegisterHandlers():
//  NetworkManager.Singleton may not be available at Load() time
//  on the client. SyncReceiver guards against this and logs a
//  warning. A future NetworkManager-ready patch can call
//  RegisterHandlers() at the correct moment if needed.
// ============================================================

namespace LilithsSoul;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SoulPlugin : BasePlugin
{
    static Harmony? _harmony;

    public override void Load()
    {
        SoulLogger.Initialize(base.Log);

        try
        {
            SoulLogger.Info("LilithsSoul",
                $"{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} loading.");

            // Config file explicitly named — avoids "audaciousbovine.lilithssoul.cfg".
            var configFile = new ConfigFile(SoulPaths.CoreConfig, saveOnInit: true);
            SoulConfig.Initialize(configFile);

            _harmony?.UnpatchSelf();
            _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();

            // Register network message handlers.
            // May no-op if NetworkManager isn't ready yet — see SyncReceiver.
            SyncReceiver.RegisterHandlers();

            SoulLogger.Info("LilithsSoul", "LilithsSoul loaded.");
        }
        catch (Exception ex)
        {
            SoulLogger.Error("LilithsSoul", $"Failed to load: {ex.Message}");
        }
    }

    public override bool Unload()
    {
        SyncReceiver.UnregisterHandlers();
        _harmony?.UnpatchSelf();

        SoulLogger.Info("LilithsSoul", "LilithsSoul unloaded.");
        return true;
    }
}