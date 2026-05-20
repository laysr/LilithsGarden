// ============================================================
//  ServerSyncPayload — LilithsSoul
//
//  Client-side duplicate of LilithsHeart.Network.ServerSyncPayload.
//  Deserialize-only — Soul never constructs or hashes a payload,
//  it only reads what Heart sends.
//
//  ⚠️  SYNC REQUIREMENT:
//      This class must be kept structurally in sync with
//      LilithsHeart.Network.ServerSyncPayload manually.
//      If Heart adds a field, add it here too. The JSON
//      deserializer will silently ignore unknown fields,
//      but missing fields will deserialize as their default.
//
//  Factory methods (Build, BuildHash, SanitizeFolderName) are
//  intentionally absent — Soul has no use for them.
// ============================================================

namespace LilithsSoul.Network;

public sealed class ServerSyncPayload
{
    /// <summary>
    /// Human-readable server name. Used as the folder key under
    /// BepInEx/config/LilithsSoul/<ServerIdentity>/
    /// Already sanitized by Heart before sending.
    /// </summary>
    public string ServerIdentity { get; set; } = string.Empty;

    /// <summary>
    /// Short SHA256 hash of the payload content.
    /// Soul compares this against the cached sync.json hash to
    /// skip redundant disk writes and localization re-injection.
    /// </summary>
    public string PayloadHash { get; set; } = string.Empty;

    /// <summary>
    /// Display name overrides keyed by prefab name.
    /// Injected into the client localization system by LocalizationInjector.
    /// </summary>
    public Dictionary<string, string> DisplayNameOverrides { get; set; } = new();

    /// <summary>
    /// Tooltip overrides keyed by prefab name.
    /// Injected into the client localization system by LocalizationInjector.
    /// </summary>
    public Dictionary<string, string> TooltipOverrides { get; set; } = new();
}