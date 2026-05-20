using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LilithsHeart.Foundation;

// ============================================================
//  SyncPayloadCache — LilithsHeart
//
//  Holds the pre-serialized ServerSyncPayload JSON string ready
//  to be chunked and sent to any connecting Soul client.
//
//  [CHANGED] Now caches JSON string instead of GZip-compressed bytes.
//  Compression is no longer needed — the chat message transport
//  splits by character count, not by binary size. Plain JSON is
//  simpler to debug and avoids base64 encoding overhead.
//
//  Build() is called once at Heart initialization and again
//  whenever LocalizationConfig is reloaded (server admin action).
//  The cached string is then read by SyncSender per connect event.
//
//  [PERFORMANCE] Build() runs O(1) serialization at init time.
//                SyncSender reads CachedJson with no allocation.
//                Thread safety: volatile string reference — safe
//                for single-writer (main thread) / multi-reader.
// ============================================================

namespace LilithsHeart.Network;

public static class SyncPayloadCache
{
    private const string LOG_SOURCE = "LilithsHeart.SyncPayloadCache";

    // volatile ensures the updated reference is visible across threads
    // without a full lock. Safe because we only write from the main thread.
    static volatile string? _cachedJson;

    /// <summary>
    /// The pre-serialized JSON payload ready to be chunked and sent.
    /// Null until Build() has been called successfully.
    /// </summary>
    public static string? CachedJson => _cachedJson;

    /// <summary>
    /// Builds and caches the JSON payload from the current server state.
    /// Call at Heart init and on localization reload.
    /// </summary>
    public static void Build(string serverIdentity)
    {
        try
        {
            var payload = ServerSyncPayload.Build(serverIdentity);
            var json    = JsonSerializer.Serialize(payload,
                new JsonSerializerOptions { WriteIndented = false });

            // Compute short hash for Soul's cache-invalidation check.
            payload.PayloadHash = ComputeHash(json);

            // Re-serialize with hash included.
            _cachedJson = JsonSerializer.Serialize(payload,
                new JsonSerializerOptions { WriteIndented = false });

            HeartLogger.Info(LOG_SOURCE,
                $"Payload cached — {_cachedJson.Length} chars, hash {payload.PayloadHash}.");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Build failed: {ex.Message}");
            _cachedJson = null;
        }
    }

    /// <summary>
    /// Invalidates the cache. Next Build() call will regenerate.
    /// </summary>
    public static void Rebuild(string serverIdentity) => Build(serverIdentity);

    // ── Internal ─────────────────────────────────────────────

    static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes)[..8];
    }
}