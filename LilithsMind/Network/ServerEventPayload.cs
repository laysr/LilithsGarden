// ============================================================
//  ServerEventPayload — LilithsMind
//  LilithsMind/Network/ServerEventPayload.cs
//
//  Trigger-based payload sent from Heart to Soul when a game
//  event occurs during a session (as opposed to ServerSyncPayload
//  which is sent once on connect).
//
//  [CHANGED] Migrated from duplicate definitions in:
//              LilithsHeart/Network/ServerEventPayload.cs
//              LilithsSoul/Network/ServerEventPayload.cs
//            Both files are now deleted. This is the single
//            definition shared between Heart and Soul.
//
//  Design:
//  ───────
//  EventKind identifies what happened. Soul switches on this
//  to know how to deserialize the Data field.
//
//  Data is a JSON-serialized string of the event-specific data.
//  Each EventKind has a corresponding data shape documented below.
//  Using a single string field keeps the payload class stable as
//  new event types are added — only EventKind and the Soul-side
//  handler need to change.
//
//  NOT cached — built fresh when the trigger fires since event
//  data is specific to the moment (which player, which item, etc.)
//
//  ⚠️  MODULE RANGE RESERVATION:
//      When adding new EventKind values, claim a range here to
//      avoid collisions across modules. Document the data shape
//      for each value inline.
//
//  EventKind ranges:
//  ──────────────────
//  Core:                0–99    (reserved)
//  LilithsCookbook:   100–199   (none yet)
//  LilithsBounty:     200–299   (none yet)
//  LilithsTreasury:   300–399   (none yet)
//  LilithsMachinations: 400–499 (none yet)
//
//  EventKind data shapes:
//  ──────────────────────
//  (none defined yet — added per module as needed)
//
//  [PERFORMANCE] Serialized per-event-occurrence on the server.
//                Keep Data payloads small — event data should be
//                deltas, not full state snapshots.
// ============================================================

namespace LilithsMind.Network;

/// <summary>
/// Identifies the type of session event being sent from Heart to Soul.
/// Values must be stable — they are serialized as integers over the wire.
/// Do not renumber or remove existing values.
/// </summary>
public enum EventKind
{
    // ── Core ────────────────────────────────────────────────

    /// <summary>Sentinel — never sent over the wire.</summary>
    None = 0,

    // ── Module events ───────────────────────────────────────
    // Claim a range above when your module adds events.
}

/// <summary>
/// Trigger-based payload sent from Heart to Soul during a session.
/// Not cached — built fresh per event occurrence.
/// </summary>
public sealed class ServerEventPayload
{
    /// <summary>
    /// What kind of event this is.
    /// Soul switches on this to route the payload to the correct handler.
    /// </summary>
    public EventKind Kind { get; set; } = EventKind.None;

    /// <summary>
    /// JSON-serialized event-specific data.
    /// Shape depends on Kind — see EventKind documentation above.
    /// Deserialize to the appropriate type in each Soul-side handler.
    /// </summary>
    public string Data { get; set; } = string.Empty;
}