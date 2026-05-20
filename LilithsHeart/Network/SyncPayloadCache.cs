using System.IO.Compression;
using System.Text;
using System.Text.Json;
using LilithsHeart.Config;
using LilithsHeart.Foundation;

// ============================================================
//  SyncPayloadCache — LilithsHeart
//
//  Owns the compressed, ready-to-send bytes of ServerSyncPayload.
//
//  Lifecycle:
//  ──────────
//  1. Heart.OnInitialize() calls SyncPayloadCache.Build() once
//     after LocalizationConfig is loaded.
//  2. On each client connect, SyncSender reads CachedBytes
//     directly — no serialization or compression at send time.
//  3. If LocalizationConfig.Reload() is called (admin hot-reload),
//     it calls SyncPayloadCache.Rebuild() which eagerly re-builds
//     so the next connecting client gets fresh data immediately.
//
//  Why eager rebuild on reload?
//  ────────────────────────────
//  Lazy rebuild would make the first connecting client after a
//  reload pay the build cost. Eager rebuild pays it immediately
//  at reload time — more predictable, better for server operators.
//
//  [PERFORMANCE] Serialization + GZip compression runs once per
//                server boot (and once per Reload() call).
//                Send time is a memcpy of CachedBytes — zero
//                allocation beyond the network buffer.
//                CachedBytes is a byte[] — no lock needed for reads
//                since it is replaced atomically via reference swap.
// ============================================================

namespace LilithsHeart.Network;

public static class SyncPayloadCache
{
    private const string LOG_SOURCE = "LilithsHeart.SyncPayloadCache";

    // Volatile so reads on other threads see the latest reference
    // after a Rebuild() without needing a lock.
    // [PERFORMANCE] Reference swap is atomic on all .NET platforms.
    static volatile byte[]? _cachedBytes;

    /// <summary>
    /// The compressed, serialized ServerSyncPayload ready to send.
    /// Null only before Build() has been called.
    /// </summary>
    public static byte[]? CachedBytes => _cachedBytes;

    /// <summary>
    /// Builds the cache from the current LocalizationConfig state.
    /// Call this once from Heart.OnInitialize() after LocalizationConfig
    /// has loaded. Also called by Rebuild() on hot-reload.
    /// </summary>
    public static void Build(string serverIdentity)
    {
        HeartLogger.Info(LOG_SOURCE, "Building sync payload cache...");

        try
        {
            var payload = ServerSyncPayload.Build(serverIdentity);
            _cachedBytes = Compress(JsonSerializer.Serialize(payload));

            HeartLogger.Info(LOG_SOURCE,
                $"Sync payload cached. " +
                $"Entries: {payload.DisplayNameOverrides.Count} names, {payload.TooltipOverrides.Count} tooltips. " +
                $"Compressed size: {_cachedBytes.Length} bytes.");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Failed to build sync payload cache: {ex.Message}");
        }
    }

    /// <summary>
    /// Eagerly rebuilds the cache after a LocalizationConfig.Reload().
    /// Call this from LocalizationConfig.Reload() after the dictionaries
    /// have been repopulated, passing the same server identity used at boot.
    /// </summary>
    public static void Rebuild(string serverIdentity)
    {
        HeartLogger.Info(LOG_SOURCE, "Rebuilding sync payload cache after reload...");
        Build(serverIdentity);
    }

    // ── Compression ─────────────────────────────────────────

    /// <summary>
    /// GZip-compresses a JSON string to bytes.
    /// [PERFORMANCE] Runs once per boot/reload. Cost is negligible
    /// compared to the I/O and ECS work already done at startup.
    /// </summary>
    static byte[] Compress(string json)
    {
        var inputBytes = Encoding.UTF8.GetBytes(json);

        using var output     = new MemoryStream();
        using var gzip       = new GZipStream(output, CompressionLevel.Optimal);
        gzip.Write(inputBytes, 0, inputBytes.Length);
        gzip.Close(); // flush before ToArray()

        return output.ToArray();
    }

    /// <summary>
    /// Decompresses GZip bytes back to a JSON string.
    /// Provided here for symmetry and testing — Soul has its own copy.
    /// </summary>
    public static string Decompress(byte[] compressed)
    {
        using var input  = new MemoryStream(compressed);
        using var gzip   = new GZipStream(input, CompressionMode.Decompress);
        using var reader = new StreamReader(gzip, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}