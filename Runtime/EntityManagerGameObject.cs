using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Entities
{
    public static partial class EntityManager
    {
        private static readonly Dictionary<GameObject, Entity> LookUp = new Dictionary<GameObject, Entity>();

        internal static event Action<GameObject, Entity> OnEntityGameObjectBind;
        internal static event Action<GameObject> OnEntityGameObjectUnbind;

        public static bool TryGetEntityByGameObject(GameObject gameObject, out Entity entity)
        {
            return LookUp.TryGetValue(gameObject, out entity);
        }

        public static Entity Create(GameObject gameObject, EntityCommandBuffer commandBuffer = null)
        {
            EntityQuery query = default;
            return Create(gameObject, query, commandBuffer);
        }

        public static Entity Create(GameObject gameObject, EntityQuery query, EntityCommandBuffer commandBuffer = null)
        {
            Assert.IsNotNull(gameObject);

            commandBuffer ??= CreateBeginCommandBuffer();
            if (!TryGetEntityByGameObject(gameObject, out var entity))
            {
                entity = Create(query, commandBuffer);
                BindGameObject(entity, gameObject, commandBuffer);
            }
            else
            {
                SetComponentData(entity, query, commandBuffer);
            }

            return entity;
        }

        public static void BindGameObject(Entity entity, GameObject gameObject, EntityCommandBuffer commandBuffer = null)
        {
            if (!CheckValid(entity))
                return;

            commandBuffer.BindGameObject(entity, gameObject);
            LookUp[gameObject] = entity;
        }

        public static void UnbindGameObject(Entity entity, out GameObject gameObject, EntityCommandBuffer commandBuffer = null)
        {
            gameObject = null;
            if (CheckValid(entity) && TryGetEntityData(entity, out var data) && data.gameObject != null)
            {
                gameObject = data.gameObject;
                commandBuffer ??= CreateEndCommandBuffer();
                commandBuffer.UnbindGameObject(entity);
            }
        }

        internal static void OnGameObjectBind(Entity entity, GameObject gameObject)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                Assert.IsNull(data.gameObject);
                data.gameObject = gameObject;
                OnEntityGameObjectBind?.Invoke(gameObject, entity);
            }
        }

        internal static void OnGameObjectUnbind(Entity entity)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                LookUp.Remove(data.gameObject);
                OnEntityGameObjectUnbind?.Invoke(data.gameObject);
                data.gameObject = null;
            }
        }

        public static T AddComponentObject<T>(Entity entity, EntityCommandBuffer commandBuffer = null) where T : Component
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                var comp = data.gameObject.AddComponent<T>();
                commandBuffer ??= CreateBeginCommandBuffer();
                commandBuffer.UpdateGameObject(entity);
                return comp;
            }

            return null;
        }

        public static Component AddComponentObject(Entity entity, Type type, EntityCommandBuffer commandBuffer = null)
        {
            if (CheckValid(entity) && TryGetEntityData(entity, out var data))
            {
                var comp = data.gameObject.AddComponent(type);
                commandBuffer ??= CreateBeginCommandBuffer();
                commandBuffer.UpdateGameObject(entity);
                return comp;
            }

            return null;
        }

        public static void RemoveComponentObject<T>(Entity entity, EntityCommandBuffer commandBuffer = null) where T : Component
        {
            if (TryGetComponentObject<T>(entity, out var component))
            {
                UnityEngine.Object.Destroy(component);
                commandBuffer ??= CreateEndCommandBuffer();
                commandBuffer.UpdateGameObject(entity);
            }
        }

        public static void RemoveComponentObject(Entity entity, Type type, EntityCommandBuffer commandBuffer = null)
        {
            if (TryGetComponentObject(entity, type, out var component))
            {
                UnityEngine.Object.Destroy(component);
                commandBuffer ??= CreateEndCommandBuffer();
                commandBuffer.UpdateGameObject(entity);
            }
        }

        public static bool TryGetComponentObject<T>(Entity entity, out T component) where T : Component
        {
            component = null;
            return CheckValid(entity) && TryGetEntityData(entity, out var data) && data.gameObject != null && data.gameObject.TryGetComponent<T>(out component);
        }

        public static bool TryGetComponentObject(Entity entity, Type type, out Component component)
        {
            component = null;
            return CheckValid(entity) && TryGetEntityData(entity, out var data) && data.gameObject != null && data.gameObject.TryGetComponent(type, out component);
        }
    }
}
