// ============================================================
//  SoulPaths — LilithsSoul
//
//  Single source of truth for every filesystem path used by
//  LilithsSoul and its child client modules.
//
//  All Soul config lives under:
//      BepInEx/config/LilithsSoul/
//
//  Structure:
//      LilithsSoul/
//          LilithsSoul.cfg                 ← Soul core settings
//          <ServerIdentity>/
//              sync.json                   ← cached ServerSyncPayload per server
//
//  ServerIdentity is the sanitized server name received in the
//  ServerSyncPayload. Each server the client connects to gets
//  its own subfolder so configs don't collide.
//
//  Unlike HeartPaths (which is flat for child modules), Soul uses
//  per-server subfolders because the data IS per-server by design.
// ============================================================

namespace LilithsSoul.Config;

public static class SoulPaths
{
    // ── Root ────────────────────────────────────────────────

    /// <summary>
    /// BepInEx/config/LilithsSoul/
    /// All Soul config lives under this directory.
    /// </summary>
    public static readonly string Root = Path.Combine(
        BepInEx.Paths.ConfigPath,
        "LilithsSoul"
    );

    // ── .cfg files ──────────────────────────────────────────

    /// <summary>
    /// BepInEx/config/LilithsSoul/LilithsSoul.cfg
    /// </summary>
    public static readonly string CoreConfig = Path.Combine(Root, "LilithsSoul.cfg");

    // ── Per-server data ─────────────────────────────────────

    /// <summary>
    /// Returns the directory for a specific server's cached data.
    /// e.g. SoulPaths.ServerDir("LilithsGarden")
    ///      → BepInEx/config/LilithsSoul/LilithsGarden/
    ///
    /// ServerIdentity comes from ServerSyncPayload.ServerIdentity
    /// which is already sanitized by Heart before sending.
    /// </summary>
    public static string ServerDir(string serverIdentity)
        => Path.Combine(Root, serverIdentity);

    /// <summary>
    /// Returns the path to the cached sync payload for a specific server.
    /// e.g. SoulPaths.SyncFile("LilithsGarden")
    ///      → BepInEx/config/LilithsSoul/LilithsGarden/sync.json
    /// </summary>
    public static string SyncFile(string serverIdentity)
        => Path.Combine(ServerDir(serverIdentity), "sync.json");
}