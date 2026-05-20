// ============================================================
//  ServerEventPayload — LilithsSoul
//
//  Client-side duplicate of LilithsHeart.Network.ServerEventPayload.
//
//  ⚠️  SYNC REQUIREMENT:
//      EventKind values MUST be kept in sync with
//      LilithsHeart.Network.EventKind manually.
//      Add new values to BOTH files at the same time.
//      The int values must match exactly — they are serialized
//      as integers over the wire.
//
//  Soul receives this payload via SyncReceiver on the
//  "LilithsGarden_Event" named message channel, switches on
//  Kind, and routes it to the appropriate handler.
// ============================================================

namespace LilithsSoul.Network;

/// <summary>
/// Mirrors LilithsHeart.Network.EventKind exactly.
/// Must be kept in sync manually.
/// </summary>
public enum EventKind
{
    // ── Core ────────────────────────────────────────────────
    None = 0,

    // ── Module events ───────────────────────────────────────
    // LilithsCookbook:     100-199  (none yet)
    // LilithsBounty:       200-299  (none yet)
    // LilithsTreasury:     300-399  (none yet)
    // LilithsMachinations: 400-499  (none yet)
}

/// <summary>
/// Trigger-based payload received from Heart during a session.
/// </summary>
public sealed class ServerEventPayload
{
    /// <summary>
    /// What kind of event this is.
    /// Soul switches on this to know how to deserialize Data.
    /// </summary>
    public EventKind Kind { get; set; } = EventKind.None;

    /// <summary>
    /// JSON-serialized event-specific data.
    /// Shape depends on Kind — deserialize to the appropriate type
    /// in each event handler.
    /// </summary>
    public string Data { get; set; } = string.Empty;
}