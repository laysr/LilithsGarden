// ============================================================
//  HeartEvents — Core event definitions for LilithsGarden.
//  New events are added here as each module requires them.
// ============================================================

namespace LilithsHeart.Systems;

/// <summary>
/// Fired once when the game world ECS is ready and safe to query.
/// Modules should defer any EntityManager work until after this fires.
/// Subscribe with SubscribeOnce unless you need to react every world load.
/// </summary>
public struct OnWorldReady { }

/// <summary>
/// Fired when the world is being torn down.
/// Modules should release cached entity references here.
/// </summary>
public struct OnWorldDestroyed { }