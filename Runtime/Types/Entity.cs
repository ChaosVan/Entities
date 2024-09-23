using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Entities
{
    public interface IComponentData { }

    public struct Entity
    {
        public ulong index;

        public static implicit operator Entity(ulong index)
        {
            return new Entity { index = index };
        }

        public static implicit operator Entity(uint index)
        {
            return new Entity { index = index };
        }

        public static implicit operator ulong(Entity entity)
        {
            return entity.index;
        }

        public static bool operator ==(Entity lhs, Entity rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Entity lhs, Entity rhs)
        {
            return lhs.Equals(rhs);
        }

        public override int GetHashCode()
        {
            return index.GetHashCode();
        }

        public bool Equals(Entity entity)
        {
            return entity.index == index;
        }

        public override bool Equals(object obj)
        {
            return obj is Entity entity && Equals(entity);
        }

        public override string ToString()
        {
            return string.Format("Entity{0}", index);
        }
    }

    internal sealed class EntityData : IDisposable
    {
        public Entity GUID { get; internal set; }    // Generic Unique Identifier  本地ID
        internal GameObject gameObject;
        internal readonly IComponentData[] m_AllComponentData;
        internal EntityArchetype archetype;

        public EntityData()
        {
            m_AllComponentData = new IComponentData[TypeManager.TypeCount];
        }

        public void Dispose()
        {
            Assert.IsNull(gameObject);
            Assert.IsFalse(archetype.Valid, $"{GUID}");
            for (int i = 0; i < m_AllComponentData.Length; i++)
                Assert.IsNull(m_AllComponentData[i], $"{GUID}: {m_AllComponentData[i]}");
            GUID = 0UL;
        }

        internal void InternalAddComponentData(ComponentType type, IComponentData componentData)
        {
            Assert.IsNull(m_AllComponentData[type.TypeIndex], $"{GUID} already has type: {TypeManager.GetType(type.TypeIndex).FullName}, {m_AllComponentData[type.TypeIndex]}");
            archetype.AddComponentType(type);
            m_AllComponentData[type.TypeIndex] = componentData;
        }

        internal bool InternalRemoveComponentData(ComponentType type, out IComponentData ret)
        {
            if (InternalGetComponentData(type, out ret))
            {
                archetype.RemoveComponentType(type);
                m_AllComponentData[type.TypeIndex] = null;
                return true;
            }

            return false;
        }

        internal bool InternalSetComponentData(IComponentData componentData, out IComponentData outData)
        {
            var type = new ComponentType(componentData.GetType());
            InternalRemoveComponentData(type, out outData);
            InternalAddComponentData(type, componentData);
            return true;
        }

        internal bool InternalGetComponentData(ComponentType type, out IComponentData ret)
        {
            ret = m_AllComponentData[type.TypeIndex];
            if (type.AccessModeType == ComponentType.AccessMode.Exclude)
                return ret == null;

            // if (type.AccessModeType == ComponentType.AccessMode.ReadOnly)
            //     return ret != null; // do nothing for now

            return ret != null;
        }

        internal bool Match(EntityQuery query)
        {
            Assert.IsTrue(query.TypesCount > 0);
            for (int i = 0; i < query.TypesCount; i++)
            {
                if (!InternalGetComponentData(query.types[i], out _))
                    return false;
            }

            return true;
        }
    }

}
