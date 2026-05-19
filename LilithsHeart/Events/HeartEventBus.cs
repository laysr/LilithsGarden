using System.Collections.Generic;
using LilithsHeart.Foundation;

// ============================================================
//  HeartEventBus — Lightweight pub/sub event system
//
//  [CHANGED] Moved from Systems/ → Events/ to better describe
//            what this folder contains.
//
//  Features:
//    - Type-safe generic subscriptions
//    - Thread-safe via lock on all operations
//    - Subscriber exceptions are caught and logged — one bad
//      module cannot break others
//    - One-shot subscriptions via SubscribeOnce()
//    - Snapshot dispatch — safe to subscribe/unsubscribe
//      from inside a handler
//
//  Usage:
//    Subscribe:   HeartEventBus.Subscribe<OnPlayerDeath>(handler);
//    Unsubscribe: HeartEventBus.Unsubscribe<OnPlayerDeath>(handler);
//    Publish:     HeartEventBus.Publish(new OnPlayerDeath { ... });
//    Once:        HeartEventBus.SubscribeOnce<OnWorldReady>(handler);
//
//  [PERFORMANCE] Delegate invocation is cheap. Lock contention
//  is negligible since events fire on the main thread.
//  Keep handlers fast — no I/O or heavy ECS queries inside them.
// ============================================================

namespace LilithsHeart.Events;

public static class HeartEventBus
{
    private const string LOG_SOURCE = "LilithsHeart.EventBus";

    // Maps event type → list of delegates.
    // object used to avoid making HeartEventBus generic itself.
    private static readonly Dictionary<Type, List<Delegate>> _handlers = new();

    // Single lock object for all thread-safe operations.
    private static readonly object _lock = new();

    public static void Initialize()
    {
        lock (_lock)
        {
            _handlers.Clear();
        }
        HeartLogger.Info(LOG_SOURCE, "EventBus initialized.");
    }

    public static void Shutdown()
    {
        lock (_lock)
        {
            _handlers.Clear();
        }
        HeartLogger.Info(LOG_SOURCE, "EventBus shut down.");
    }

    /// <summary>
    /// Subscribe to an event type.
    /// Call this in your module's Load().
    /// Remember to Unsubscribe in Unload() for hot-reload compatibility.
    /// </summary>
    public static void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : struct
    {
        var type = typeof(TEvent);

        lock (_lock)
        {
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            _handlers[type].Add(handler);
        }

        HeartLogger.Debug(LOG_SOURCE, $"Subscribed handler for {type.Name}.");
    }

    /// <summary>
    /// Subscribe to an event type for a single firing only.
    /// The handler automatically unsubscribes after the first time it is called.
    /// Useful for one-time setup on OnWorldReady, etc.
    /// </summary>
    public static void SubscribeOnce<TEvent>(Action<TEvent> handler) where TEvent : struct
    {
        Action<TEvent>? wrapper = null;
        wrapper = evt =>
        {
            handler(evt);
            Unsubscribe(wrapper!);
        };
        Subscribe(wrapper);

        HeartLogger.Debug(LOG_SOURCE, $"SubscribeOnce registered for {typeof(TEvent).Name}.");
    }

    /// <summary>
    /// Unsubscribe a previously registered handler.
    /// Important for modules that support hot-reload — call in Unload().
    /// </summary>
    public static void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : struct
    {
        var type = typeof(TEvent);

        lock (_lock)
        {
            if (_handlers.TryGetValue(type, out var list))
                list.Remove(handler);
        }

        HeartLogger.Debug(LOG_SOURCE, $"Unsubscribed handler for {type.Name}.");
    }

    /// <summary>
    /// Publish an event to all subscribers.
    /// Dispatches synchronously on the calling thread.
    /// Subscriber exceptions are caught and logged — other subscribers
    /// will still receive the event even if one handler throws.
    ///
    /// [PERFORMANCE] Dispatches to a snapshot copy of the handler list
    ///               so subscribe/unsubscribe during dispatch is safe.
    ///               Keep handlers fast — queue expensive work if needed.
    /// </summary>
    public static void Publish<TEvent>(TEvent evt) where TEvent : struct
    {
        var type = typeof(TEvent);
        Delegate[]? snapshot = null;

        // Take a snapshot under lock, then dispatch outside the lock
        // so handlers can safely call Subscribe/Unsubscribe.
        lock (_lock)
        {
            if (_handlers.TryGetValue(type, out var list) && list.Count > 0)
                snapshot = list.ToArray();
        }

        if (snapshot == null)
            return;

        foreach (var handler in snapshot)
        {
            try
            {
                ((Action<TEvent>)handler).Invoke(evt);
            }
            catch (Exception ex)
            {
                // Exceptions are caught per-handler so one bad subscriber
                // cannot prevent others from receiving the event.
                HeartLogger.Error(LOG_SOURCE, $"Handler for {type.Name} threw: {ex.Message}");
            }
        }
    }
}