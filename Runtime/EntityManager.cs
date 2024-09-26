using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Entities
{
    public static partial class EntityManager
    {
        [DomainReload(1000000UL)]
        internal static ulong GUID_COUNT = 1000000UL;

        internal static event Action<ulong> OnEntityCreated;
        internal static event Action<ulong> OnEntityDestroyed;
        internal static event Action<ulong, IComponentData> OnEntityAddComponentData;
        internal static event Action<ulong, IComponentData> OnEntityRemoveComponentData;

        internal static readonly Dictionary<Entity, EntityData> Entities = new Dictionary<Entity, EntityData>();
        internal static GCHandle m_World;

        private static EntityCommandBufferSystem ecbs_begin, ecbs_end;

        public static World World => (World)m_World.Target;

        internal static void Initialize(World world)
        {
            LookUp.Clear();
            Entities.Clear();

            if (m_World.IsAllocated)
                m_World.Free();

            m_World = GCHandle.Alloc(world);

            ecbs_begin = world.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
            ecbs_end = world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        internal static bool CheckValid(Entity entity)
        {
            return entity.index > 0UL;
        }

        internal static bool TryGetEntityData(Entity entity, out EntityData data)
        {
            return Entities.TryGetValue(entity, out data);
        }

        internal static Entity[] BeginStructual(int count, params Entity[] entities)
        {
            var array = new Entity[count];
            if (entities != null && entities.Length >= count)
                Array.Copy(entities, array, count);

            return array;
        }

        internal static void EndStructual(IEnumerable<Entity> entities)
        {
            var e = World.Systems.GetEnumerator();
            while (e.MoveNext())
            {
                if (e.Current is SystemBase system)
                {
                    ForEach(entities, entity => system.InjectEntity(entity));
                }
            }
        }

        public static void ForEach<T>(IEnumerable<T> ie, Action<T> action)
        {
            foreach (var obj in ie) action?.Invoke(obj);
        }

        public static EntityCommandBuffer CreateBeginCommandBuffer()
        {
            return ecbs_begin.CreateCommandBuffer();
        }

        public static EntityCommandBuffer CreateEndCommandBuffer()
        {
            return ecbs_end.CreateCommandBuffer();
        }

        public static bool TryGetEntities(EntityQuery query, out List<Entity> entities)
        {
            entities = new List<Entity>();
            foreach (var entity in Entities.Keys)
            {
                if (Match(entity, query, out _))
                {
                    entities.Add(entity);
                }
            }

            return entities.Count > 0;
        }

        public static void DestroyAll(out GameObject[] gameObjects, EntityCommandBuffer commandBuffer = null)
        {
            DestroyEntities(Entities.Keys, out gameObjects, commandBuffer);
        }

        public static void DestroyEntities(EntityQuery query, out GameObject[] gameObjects, EntityCommandBuffer commandBuffer = null)
        {
            gameObjects = null;
            if (TryGetEntities(query, out var entities))
                DestroyEntities(entities, out gameObjects, commandBuffer);
        }

        public static void DestroyEntities(IEnumerable<Entity> entities, out GameObject[] gameObjects, EntityCommandBuffer commandBuffer = null)
        {
            gameObjects = new GameObject[entities.Count()];
            commandBuffer ??= CreateEndCommandBuffer();
            int index = 0;
            foreach (var entity in entities)
            {
                DestroyEntity(entity, out gameObjects[index++], commandBuffer);
            }
        }

        public static void DestroyEntity(Entity entity, out GameObject gameObject, EntityCommandBuffer commandBuffer = null)
        {
            gameObject = null;
            if (!CheckValid(entity))
                return;

            commandBuffer ??= CreateEndCommandBuffer();
            UnbindGameObject(entity, out gameObject, commandBuffer);
            commandBuffer.DestroyEntity(entity);
        }

        public static Entity Create(EntityCommandBuffer commandBuffer = null)
        {
            EntityQuery query = default;
            return Create(query, commandBuffer);
        }

        public static Entity Create(EntityCommandBuffer commandBuffer = null, params ComponentType[] componentTypes)
        {
            EntityQuery query = new EntityQuery(componentTypes);
            return Create(query, commandBuffer);
        }

        public static Entity Create(EntityQuery query, EntityCommandBuffer commandBuffer = null)
        {
            Create(query, 1, out var entities, commandBuffer);
            return entities[0];
        }

        public static void Create(EntityQuery query, int count, out Entity[] entities, EntityCommandBuffer commandBuffer = null)
        {
            commandBuffer ??= CreateBeginCommandBuffer();
            entities = new Entity[count];
            for (int i = 0; i < count; i++)
            {
                entities[i] = new Entity { index = GUID_COUNT++ };
                commandBuffer.CreateEntity(entities[i], query);
            }
        }

        public static void AddComponentData(Entity entity, Type type, EntityCommandBuffer commandBuffer = null)
        {
            commandBuffer ??= CreateBeginCommandBuffer();
            commandBuffer.AddComponentData(entity, type);
        }

        public static void AddComponentData<T>(Entity entity, EntityCommandBuffer commandBuffer = null) where T : IComponentData
        {
            AddComponentData(entity, typeof(T), commandBuffer);
        }

        public static void RemoveComponentData(Entity entity, Type type, EntityCommandBuffer commandBuffer = null)
        {
            commandBuffer ??= CreateEndCommandBuffer();
            commandBuffer.RemoveComponentData(entity, type);
        }

        public static void RemoveComponentData<T>(Entity entity, EntityCommandBuffer commandBuffer = null) where T : IComponentData
        {
            RemoveComponentData(entity, typeof(T), commandBuffer);
        }

        public static void RemoveComponentData(Entity entity, EntityQuery query, EntityCommandBuffer commandBuffer = null)
        {
            commandBuffer ??= CreateEndCommandBuffer();
            commandBuffer.RemoveComponentData(entity, query);
        }

        public static void SetComponentData(Entity entity, IComponentData componentData, EntityCommandBuffer commandBuffer = null)
        {
            commandBuffer ??= CreateBeginCommandBuffer();
            commandBuffer.SetComponentData(entity, componentData);
        }

        public static IComponentData SetComponentData(Entity entity, Type type, EntityCommandBuffer commandBuffer = null)
        {
            var componentData = (IComponentData)ReferencePool.SpawnInstance(type);
            SetComponentData(entity, componentData, commandBuffer);
            return componentData;
        }

        public static T SetComponentData<T>(Entity entity, EntityCommandBuffer commandBuffer = null) where T : IComponentData
        {
            var componentData = ReferencePool.SpawnInstance<T>();
            SetComponentData(entity, componentData, commandBuffer);
            return componentData;
        }

        public static void SetComponentData(Entity entity, EntityQuery query, EntityCommandBuffer commandBuffer = null)
        {
            commandBuffer ??= CreateBeginCommandBuffer();
            commandBuffer.SetComponentData(entity, query);
        }

        public static bool TryGetComponentData(Entity entity, ComponentType type, out IComponentData componentData)
        {
            componentData = null;
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
                return data.InternalGetComponentData(type, out componentData);

            return false;
        }

        public static bool TryGetComponentData<T>(Entity entity, out T componentData) where T : IComponentData
        {
            componentData = default;
            if (TryGetComponentData(entity, typeof(T), out var component))
                return (componentData = (T)component) != null;

            return false;
        }

        internal static EntityData InternalCreate(Entity entity, EntityQuery query)
        {
            var data = ReferencePool.SpawnInstance<EntityData>();
            data.GUID = entity;
            Entities.Add(entity, data);
            OnEntityCreated?.Invoke(entity);
            InternalSetComponentData(entity, query);

            return data;
        }

        internal static void InternalDestroyEntity(Entity entity)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                InternalRemoveComponentData(entity, data.archetype);
                OnEntityDestroyed?.Invoke(entity);
                Entities.Remove(entity);
                ReferencePool.RecycleInstance(data);
            }
        }

        internal static void InternalAddComponentData(Entity entity, ComponentType componentType)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                var componentData = (IComponentData)ReferencePool.SpawnInstance(componentType);
                data.InternalAddComponentData(componentType, componentData);
                OnEntityAddComponentData?.Invoke(entity, componentData);
            }
        }

        internal static void InternalSetComponentData(Entity entity, IComponentData componentData)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                if (data.InternalSetComponentData(componentData, out var outData))
                {
                    if (outData != null)
                    {
                        OnEntityRemoveComponentData?.Invoke(entity, outData);
                        ReferencePool.RecycleInstance(outData);
                    }

                    OnEntityAddComponentData?.Invoke(entity, componentData);
                }
            }
        }

        internal static void InternalSetComponentData(Entity entity, EntityQuery query)
        {
            if (query.Valid && CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                ForEach(query.types, componentType =>
                {
                    if (componentType.AccessModeType == ComponentType.AccessMode.ReadWrite)
                    {
                        var componentData = (IComponentData)ReferencePool.SpawnInstance(componentType);
                        if (data.InternalSetComponentData(componentData, out var outData))
                        {
                            if (outData != null)
                            {
                                OnEntityRemoveComponentData?.Invoke(entity, outData);
                                ReferencePool.RecycleInstance(outData);
                            }

                            OnEntityAddComponentData?.Invoke(entity, componentData);
                        }
                    }
                    else if (componentType.AccessModeType == ComponentType.AccessMode.Exclude)
                    {
                        if (data.InternalRemoveComponentData(componentType.GetManagedType(), out var componentData))
                        {
                            OnEntityRemoveComponentData?.Invoke(entity, componentData);
                            ReferencePool.RecycleInstance(componentData);
                        }
                    }
                });
            }
        }

        internal static void InternalRemoveComponentData(Entity entity, ComponentType type)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                if (data.InternalRemoveComponentData(type, out var componentData))
                {
                    OnEntityRemoveComponentData?.Invoke(entity, componentData);
                    ReferencePool.RecycleInstance(componentData);
                }
            }
        }

        internal static void InternalRemoveComponentData(Entity entity, EntityQuery query)
        {
            if (query.Valid && CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                ForEach(query.types, componentType =>
                {
                    if (data.InternalRemoveComponentData(componentType, out var componentData))
                    {
                        OnEntityRemoveComponentData?.Invoke(entity, componentData);
                        ReferencePool.RecycleInstance(componentData);
                    }
                });
            }
        }

        internal static bool Match(Entity entity, EntityQuery query, out EntityData data)
        {
            data = null;
            if (CheckValid(entity) && TryGetEntityData(entity, out data))
                return data.Match(query);

            return false;
        }
    }

    public static class EntityExtensions
    {
        public static T GetOrAddComponentData<T>(this Entity entity, EntityCommandBuffer commandBuffer = null) where T : IComponentData
        {
            if (EntityManager.TryGetComponentData(entity, typeof(T), out var componentData))
                return (T)componentData;

            return EntityManager.SetComponentData<T>(entity, commandBuffer); ;
        }

        public static T GetOrAddComponentObject<T>(this Entity entity, EntityCommandBuffer commandBuffer = null) where T : Component
        {
            if (!EntityManager.TryGetComponentObject<T>(entity, out var component))
                component = EntityManager.AddComponentObject<T>(entity, commandBuffer);

            return component;
        }

        public static Entity[] Expand(this Entity[] array, Entity item)
        {
            Entity[] newArray = new Entity[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = item;
            return newArray;
        }

        public static bool Contains(this Entity[] array, Entity item)
        {
            return array.IndexOf(item) >= 0;
        }

        public static int IndexOf(this Entity[] array, Entity item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                    return i;
            }

            return -1;
        }
    }
}
