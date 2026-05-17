using ProjectM;
using ProjectM.Scripting;
using Unity.Entities;

namespace LilithsHeart.Extensions;

public static class EntityExtensions
{
    public delegate void WithRefHandler<T>(ref T item) where T : struct;

    public static void With<T>(this Entity entity, WithRefHandler<T> action) where T : struct
    {
        if (!entity.Has<T>()) return;

        T item = Core.EntityManager.GetComponentData<T>(entity);
        action(ref item);
        Core.EntityManager.SetComponentData(entity, item);
    }

    public static T Read<T>(this Entity entity) where T : struct
    {
        return Core.EntityManager.GetComponentData<T>(entity);
    }

    public static void Write<T>(this Entity entity, T componentData) where T : struct
    {
        if (!entity.Has<T>()) return;
        Core.EntityManager.SetComponentData(entity, componentData);
    }

    public static bool Has<T>(this Entity entity) where T : struct
    {
        return Core.EntityManager.HasComponent<T>(entity);
    }

    public static void Add<T>(this Entity entity) where T : struct
    {
        if (!entity.Has<T>())
            Core.EntityManager.AddComponent<T>(entity);
    }

    public static void Remove<T>(this Entity entity) where T : struct
    {
        if (entity.Has<T>())
            Core.EntityManager.RemoveComponent<T>(entity);
    }

    public static DynamicBuffer<T> ReadBuffer<T>(this Entity entity) where T : struct
    {
        return Core.EntityManager.GetBuffer<T>(entity);
    }

    public static DynamicBuffer<T> AddBuffer<T>(this Entity entity) where T : struct
    {
        return Core.EntityManager.AddBuffer<T>(entity);
    }

    public static bool TryGetBuffer<T>(this Entity entity, out DynamicBuffer<T> buffer) where T : struct
    {
        if (Core.EntityManager.HasComponent<T>(entity))
        {
            buffer = Core.EntityManager.GetBuffer<T>(entity);
            return true;
        }

        buffer = default;
        return false;
    }

    public static bool TryGetComponent<T>(this Entity entity, out T componentData) where T : struct
    {
        componentData = default;

        if (entity.Has<T>())
        {
            componentData = entity.Read<T>();
            return true;
        }

        return false;
    }
}