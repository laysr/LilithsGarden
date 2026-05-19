// ============================================================
//  ServerSyncPayload — LilithsHeart
//
//  The single data bundle Heart sends to a connecting Soul client.
//  Sent once per connection. Soul writes it to disk under:
//      BepInEx/config/LilithsSoul/<ServerIdentity>/sync.json
//
//  Design decisions:
//  ─────────────────
//  • One packet, one file — all server-defined client data travels
//    together. This keeps the network handshake simple and gives
//    Soul a single source of truth per server on disk.
//
//  • ServerIdentity is used as the folder key on the Soul side.
//    It should be stable across restarts (server name from
//    GameSettings, not a runtime GUID) so Soul can match a cached
//    payload to the right server on reconnect.
//
//  • Gameplay settings are a snapshot of the values Heart has
//    applied server-side. Soul uses these to patch client prefabs
//    so the UI reflects actual server values (recipe costs, etc.).
//
//  • Localization entries are the flattened contents of
//    LocalizationConfig — keyed by prefab name, separate display
//    name and tooltip fields. null means "use vanilla string".
//
//  • PayloadHash is an xxHash/SHA256 short hash of the serialized
//    content. Soul compares this against the cached file's hash
//    and skips re-writing and re-patching if nothing changed.
//    This avoids unnecessary disk I/O and prefab re-patching on
//    every connect to the same server.
//
//  [PERFORMANCE] This class is serialized once on connect and
//                deserialized once on the client. It is not
//                updated or re-sent during a session.
//                Keep fields flat — avoid nested collections
//                that would bloat serialization cost.
// ============================================================

namespace LilithsHeart.Network;

/// <summary>
/// All server-defined configuration that Heart sends to Soul on connect.
/// Serialize to JSON for both network transport and on-disk caching.
/// </summary>
public sealed class ServerSyncPayload
{
    // ── Identity ────────────────────────────────────────────

    /// <summary>
    /// Human-readable server name (from GameSettings).
    /// Used by Soul as the folder name under LilithsSoul/config/.
    /// Must be filesystem-safe — sanitize before use as a path segment.
    /// </summary>
    public string ServerIdentity { get; set; } = string.Empty;

    /// <summary>
    /// Short content hash of this payload (e.g. first 8 chars of SHA256).
    /// Soul compares this to its cached value to skip redundant writes/patches.
    /// Populated by <see cref="BuildHash"/> before sending.
    /// </summary>
    public string PayloadHash { get; set; } = string.Empty;

    // ── Gameplay settings ───────────────────────────────────
    // These mirror the server-side values Heart has applied.
    // Soul uses them to patch client-side prefab data so the
    // UI displays correct quantities, costs, and multipliers.

    /// <summary>Server-configured starting inventory size.</summary>
    public int StartingInventorySize { get; set; }

    /// <summary>Server-configured global movement speed multiplier.</summary>
    public float GlobalMovementSpeedMultiplier { get; set; }

    // ── Localization ────────────────────────────────────────

    /// <summary>
    /// Display name overrides, keyed by prefab name.
    /// Value is the server-defined display string.
    /// Soul injects these into the client localization system.
    /// </summary>
    public Dictionary<string, string> DisplayNameOverrides { get; set; } = new();

    /// <summary>
    /// Tooltip overrides, keyed by prefab name.
    /// Value is the server-defined tooltip string.
    /// </summary>
    public Dictionary<string, string> TooltipOverrides { get; set; } = new();

    // ── Factory ─────────────────────────────────────────────

    /// <summary>
    /// Builds a ready-to-send payload from the current Heart config state.
    /// Call this once per connecting client, not once per session —
    /// different servers may run different Heart versions with different values.
    /// </summary>
    public static ServerSyncPayload Build(string serverIdentity)
    {
        var payload = new ServerSyncPayload
        {
            ServerIdentity              = SanitizeFolderName(serverIdentity),
            StartingInventorySize       = Config.HeartConfig.StartingInventorySize,
            GlobalMovementSpeedMultiplier = Config.HeartConfig.GlobalPlayerMovementSpeedMultiplier,

            // [ADDED] Flatten LocalizationConfig dictionaries into the payload.
            //         ToList() + new Dictionary avoids sharing the live reference.
            DisplayNameOverrides = new Dictionary<string, string>(Config.LocalizationConfig.DisplayNames),
            TooltipOverrides     = new Dictionary<string, string>(Config.LocalizationConfig.Tooltips),
        };

        payload.PayloadHash = payload.BuildHash();
        return payload;
    }

    /// <summary>
    /// Computes a short stable hash of the payload content (excluding PayloadHash itself).
    /// Used by Soul to detect unchanged payloads and skip redundant work.
    /// </summary>
    public string BuildHash()
    {
        // [PERFORMANCE] We serialize to JSON to get a canonical string, then hash it.
        //               This runs once per connect — cost is acceptable.
        //               If payload size grows significantly, switch to a field-by-field
        //               hash to avoid the intermediate JSON allocation.
        var content = System.Text.Json.JsonSerializer.Serialize(new
        {
            ServerIdentity,
            StartingInventorySize,
            GlobalMovementSpeedMultiplier,
            DisplayNameOverrides,
            TooltipOverrides,
        });

        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes(content));

        // First 8 hex chars — short enough for a filename/log, collision risk is negligible.
        return Convert.ToHexString(bytes)[..8];
    }

    // ── Helpers ─────────────────────────────────────────────

    /// <summary>
    /// Strips characters that are invalid in folder names on Windows and Linux.
    /// Soul uses ServerIdentity directly as a path segment.
    /// </summary>
    static string SanitizeFolderName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(name.Select(c => invalid.Contains(c) ? '_' : c)).Trim();
    }
}