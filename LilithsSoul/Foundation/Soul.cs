using Unity.Entities;

// ============================================================
//  Soul — LilithsSoul
//
//  Provides access to the V Rising client ECS world and its
//  managed systems. Mirrors Heart.cs on the server side.
//
//  The client world is named differently from the server world
//  and is found via WorldUtility.FindClientWorld() or by
//  searching World.All for a non-server world.
//
//  [PERFORMANCE] World reference is cached after first access.
//                Null-checked on each access in case the world
//                is torn down between calls.
// ============================================================

namespace LilithsSoul.Foundation;

public static class Soul
{
    static World? _client;

    /// <summary>
    /// The V Rising client ECS World. Null if not yet ready.
    /// </summary>
    public static World? ClientWorld
    {
        get
        {
            if (_client?.IsCreated == true)
                return _client;

            // Find the client world — it's the non-server world in World.All.
            foreach (var world in World.All)
            {
                if (world.Name.Contains("Client", StringComparison.OrdinalIgnoreCase))
                {
                    _client = world;
                    return _client;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Clears the cached world reference.
    /// Call when the client disconnects so the next access re-resolves.
    /// </summary>
    public static void Reset() => _client = null;
}