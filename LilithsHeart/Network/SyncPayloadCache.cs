using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LilithsHeart.Config;
using LilithsHeart.Foundation;
using LilithsMind.Network;

// ============================================================
//  SyncPayloadCache — LilithsHeart
//
//  Builds and caches the serialized ServerSyncPayload JSON that
//  is sent to connecting Soul clients via chunked chat messages.
//
//  Heart calls Rebuild() twice:
//    1. Before OnInitialized fires — baseline empty payload.
//    2. After all modules have registered overrides — final payload.
//
//  SyncSender reads CachedJson on demand when a client connects.
//
//  [CHANGED] ServerSyncPayload.Build() inlined into Rebuild().
//            ServerSyncPayload has moved to LilithsMind which has
//            no dependency on LilithsHeart.Config. Localization
//            seeding and identity sanitization now happen here
//            where LocalizationConfig is accessible.
//
//  [PERFORMANCE] Serialization runs at most twice at startup.
//                No per-frame or per-connect serialization cost.
// ============================================================

namespace LilithsHeart.Network;

public static class SyncPayloadCache
{
    private const string LOG_SOURCE = "LilithsHeart.SyncPayloadCache";

    static readonly JsonSerializerOptions _writeOptions = new() { WriteIndented = false };

    static volatile string? _cachedJson;
    public static string? CachedJson => _cachedJson;

    /// <summary>
    /// Builds and caches the sync payload JSON.
    /// Called by Heart.OnInitialize() before and after module registration.
    ///
    /// [PERFORMANCE] Serializes once per call — runs at most twice at startup.
    /// </summary>
    public static void Rebuild(
        string serverIdentity,
        Dictionary<string, RecipeOverrideData> recipeOverrides,
        Dictionary<string, StationRecipeOverrideData> stationRecipeOverrides,
        List<string> playerRecipesToAdd,
        List<string> playerRecipesToRemove)
    {
        try
        {
            // [CHANGED] Inlined from ServerSyncPayload.Build().
            //           ServerSyncPayload is now a plain DTO in LilithsMind
            //           with no access to Heart-side config. Localization
            //           seeding and folder name sanitization live here instead.
            var payload = new ServerSyncPayload
            {
                ServerIdentity       = SanitizeFolderName(serverIdentity),
                DisplayNameOverrides = new Dictionary<string, string>(LocalizationConfig.DisplayNames),
                TooltipOverrides     = new Dictionary<string, string>(LocalizationConfig.Tooltips),
            };

            foreach (var (key, value) in recipeOverrides)
                payload.RecipeOverrides[key] = value;

            foreach (var (key, value) in stationRecipeOverrides)
                payload.StationRecipeOverrides[key] = value;

            payload.PlayerRecipesToAdd    = new List<string>(playerRecipesToAdd);
            payload.PlayerRecipesToRemove = new List<string>(playerRecipesToRemove);

            // Hash is computed on the payload without the hash field itself
            // to avoid a circular dependency.
            var jsonForHash = JsonSerializer.Serialize(payload, _writeOptions);
            payload.PayloadHash = ComputeHash(jsonForHash);

            _cachedJson = JsonSerializer.Serialize(payload, _writeOptions);

            HeartLogger.Info(LOG_SOURCE,
                $"Payload cached — {_cachedJson.Length} chars, " +
                $"hash {payload.PayloadHash}, " +
                $"{payload.RecipeOverrides.Count} recipe override(s), " +
                $"{payload.StationRecipeOverrides.Count} station override(s), " +
                $"{payload.PlayerRecipesToAdd.Count} player add(s), " +
                $"{payload.PlayerRecipesToRemove.Count} player remove(s).");
        }
        catch (Exception ex)
        {
            HeartLogger.Error(LOG_SOURCE, $"Build failed: {ex.Message}");
            _cachedJson = null;
        }
    }

    // ── Internal ─────────────────────────────────────────────

    // [CHANGED] Moved from ServerSyncPayload.Build() — sanitizes the server
    //           name for safe use as a filesystem folder name.
    static string SanitizeFolderName(string name)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(name.Select(c => invalid.Contains(c) ? '_' : c)).Trim();
    }

    static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes)[..8];
    }
}