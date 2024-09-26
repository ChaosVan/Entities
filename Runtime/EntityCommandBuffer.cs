using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public class EntityCommandBuffer : System.IDisposable
    {
        internal int SystemID;
        internal SystemHandleUntyped OriginSystemHandle;

        private readonly List<EntityCommandBufferData> m_Data = new List<EntityCommandBufferData>();

        public void Dispose()
        {
            m_Data.Clear();
        }

        internal void CreateEntity(Entity entity, EntityQuery query)
        {
            AddEntityArchetypeCommand(ECBCommand.CreateEntity, entity, query);
        }

        internal void DestroyEntity(Entity entity)
        {
            AddEntityDestroyCommand(ECBCommand.DestroyEntity, entity);
        }

        internal void AddComponentData(Entity entity, ComponentType componentType)
        {
            AddEntityComponentTypeCommand(ECBCommand.AddComponent, entity, componentType);
        }

        internal void RemoveComponentData(Entity entity, ComponentType componentType)
        {
            AddEntityComponentTypeCommand(ECBCommand.RemoveComponent, entity, componentType);
        }

        internal void RemoveComponentData(Entity entity, EntityQuery query)
        {
            AddEntityQueryCommand(ECBCommand.RemoveComponentWithQuery, entity, query);
        }

        internal void SetComponentData(Entity entity, IComponentData componentData)
        {
            AddEntityComponentDataCommand(ECBCommand.SetComponent, entity, componentData);
        }

        internal void SetComponentData(Entity entity, EntityQuery query)
        {
            AddEntityArchetypeCommand(ECBCommand.SetComponentWithQuery, entity, query);
        }

        internal void BindGameObject(Entity entity, GameObject gameObject)
        {
            AddGameObjectCommand(ECBCommand.BindGameObject, entity, gameObject);
        }

        internal void UnbindGameObject(Entity entity)
        {
            AddGameObjectCommand(ECBCommand.UnbindGameObject, entity);
        }

        internal void UpdateGameObject(Entity entity)
        {
            AddGameObjectCommand(ECBCommand.UpdateGameObject, entity);
        }

        private void AddEntityDestroyCommand(ECBCommand op, Entity entity)
        {
            var ecbd = new EntityCommandBufferData();
            ecbd.commandType = op;
            ecbd.entity = entity;
            m_Data.Add(ecbd);
        }

        private void AddEntityComponentDataCommand(ECBCommand op, Entity entity, IComponentData componentData)
        {
            var ecbd = new EntityCommandBufferData();
            ecbd.commandType = op;
            ecbd.entity = entity;
            ecbd.componentData = componentData;
            m_Data.Add(ecbd);
        }

        private void AddEntityComponentTypeCommand(ECBCommand op, Entity entity, ComponentType componentType)
        {
            var ecbd = new EntityCommandBufferData();
            ecbd.commandType = op;
            ecbd.entity = entity;
            ecbd.componentType = componentType;
            m_Data.Add(ecbd);
        }

        private void AddEntityArchetypeCommand(ECBCommand op, Entity entity, EntityQuery query)
        {
            var ecbd = new EntityCommandBufferData();
            ecbd.commandType = op;
            ecbd.entity = entity;
            ecbd.query = query;
            m_Data.Add(ecbd);
        }

        private void AddEntityQueryCommand(ECBCommand op, Entity entity, EntityQuery query)
        {
            var ecbd = new EntityCommandBufferData();
            ecbd.commandType = op;
            ecbd.entity = entity;
            ecbd.query = query;
            m_Data.Add(ecbd);
        }

        private void AddGameObjectCommand(ECBCommand op, Entity entity, GameObject gameObject = null)
        {
            var ecbd = new EntityCommandBufferData();
            ecbd.commandType = op;
            ecbd.entity = entity;
            ecbd.gameObject = gameObject;
            m_Data.Add(ecbd);
        }

        internal void Playback(ref Entity[] entities)
        {
            PlaybackInternal(ref entities);
        }

        private void PlaybackInternal(ref Entity[] entities)
        {
            foreach (var data in m_Data)
            {
                try
                {
                    Entity entity = data.entity;
                    switch (data.commandType)
                    {
                        case ECBCommand.CreateEntity:
                            EntityManager.InternalCreate(entity, data.query);
                            break;
                        case ECBCommand.DestroyEntity:
                            EntityManager.InternalDestroyEntity(entity);
                            break;
                        case ECBCommand.AddComponent:
                            EntityManager.InternalAddComponentData(entity, data.componentType);
                            break;
                        case ECBCommand.SetComponent:
                            EntityManager.InternalSetComponentData(entity, data.componentData);
                            break;
                        case ECBCommand.SetComponentWithQuery:
                            EntityManager.InternalSetComponentData(entity, data.query);
                            break;
                        case ECBCommand.RemoveComponent:
                            EntityManager.InternalRemoveComponentData(entity, data.componentType);
                            break;
                        case ECBCommand.RemoveComponentWithQuery:
                            EntityManager.InternalRemoveComponentData(entity, data.query);
                            break;
                        case ECBCommand.BindGameObject:
                            EntityManager.OnGameObjectBind(entity, data.gameObject);
                            break;
                        case ECBCommand.UnbindGameObject:
                            EntityManager.OnGameObjectUnbind(entity);
                            break;
                        default:
                            break;
                    }

                    if (!entities.Contains(entity))
                        entities = entities.Expand(entity);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }

            m_Data.Clear();
        }

        private struct EntityCommandBufferData
        {
            internal ECBCommand commandType;
            internal Entity entity;
            internal ComponentType componentType;
            internal IComponentData componentData;
            internal EntityQuery query;
            internal GameObject gameObject;
        }

        private enum ECBCommand
        {
            CreateEntity,
            DestroyEntity,

            AddComponent,
            SetComponent,
            SetComponentWithQuery,
            RemoveComponent,
            RemoveComponentWithQuery,

            BindGameObject,
            UnbindGameObject,
            UpdateGameObject,
        }
    }
}
