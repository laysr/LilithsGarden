// ============================================================
//  ServerEventPayload — LilithsHeart
//
//  Trigger-based payload sent from Heart to Soul when a game
//  event occurs during a session (as opposed to ServerSyncPayload
//  which is sent once on connect).
//
//  Design:
//  ───────
//  • EventKind identifies what happened. Soul switches on this
//    to know how to deserialize the Data field.
//
//  • Data is a JSON-serialized string of the event-specific data.
//    Each EventKind has a corresponding data shape documented below.
//    Using a single string field keeps the payload class stable as
//    new event types are added — only EventKind and the Soul-side
//    handler need to change.
//
//  • NOT cached — built fresh when the trigger fires since event
//    data is specific to the moment (which player, which item, etc.)
//
//  ⚠️  SYNC REQUIREMENT:
//      EventKind enum values MUST be kept in sync with the
//      parallel enum in LilithsSoul/Network/ServerEventPayload.cs.
//      Both sides are maintained manually — add new values to
//      both files at the same time and document the data shape here.
//
//  EventKind data shapes:
//  ──────────────────────
//  (none defined yet — added per module as needed)
//
//  [PERFORMANCE] Serialized per-event-occurrence on the server.
//                Keep Data payloads small — event data should be
//                deltas, not full state snapshots.
// ============================================================

namespace LilithsHeart.Network;

/// <summary>
/// Identifies the type of event being sent to Soul.
/// Must be kept in sync with LilithsSoul.Network.EventKind manually.
/// </summary>
public enum EventKind
{
    // ── Core ────────────────────────────────────────────────
    // Reserved range 0-99 for Heart core events.

    // None is a sentinel — never sent over the wire.
    None = 0,

    // ── Module events ───────────────────────────────────────
    // Each module claims a range when it adds events.
    // Document the range here to avoid collisions.
    //
    // LilithsCookbook:   100-199  (none yet)
    // LilithsBounty:     200-299  (none yet)
    // LilithsTreasury:   300-399  (none yet)
    // LilithsMachinations: 400-499 (none yet)
}

/// <summary>
/// Trigger-based payload sent from Heart to Soul during a session.
/// Not cached — built fresh per event occurrence.
/// </summary>
public sealed class ServerEventPayload
{
    /// <summary>
    /// What kind of event this is. Soul switches on this to route the payload.
    /// </summary>
    public EventKind Kind { get; set; } = EventKind.None;

    /// <summary>
    /// JSON-serialized event-specific data.
    /// Shape depends on Kind — see EventKind documentation above.
    /// </summary>
    public string Data { get; set; } = string.Empty;

    /// <summary>
    /// Builds a ServerEventPayload for the given event kind and data object.
    /// Serializes data to JSON automatically.
    /// </summary>
    public static ServerEventPayload Build<T>(EventKind kind, T data)
        => new()
        {
            Kind = kind,
            Data = System.Text.Json.JsonSerializer.Serialize(data)
        };
}