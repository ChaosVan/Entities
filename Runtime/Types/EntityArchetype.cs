using System;
using System.Linq;

namespace Entities
{
    public struct EntityQuery
    {
        public ComponentType[] types;

        public readonly int TypesCount => types == null ? 0 : types.Length;

        public readonly bool Valid => TypesCount > 0;

        public EntityQuery(params ComponentType[] types)
        {
            this.types = types;
        }

        public readonly ComponentType this[int index] => types[index];
    }

    internal readonly struct EntityArchetype : IEquatable<EntityArchetype>
    {
        internal readonly ComponentType[] types;

        internal EntityArchetype(int typeCount)
        {
            this.types = new ComponentType[typeCount];
            for (int i = 0; i < typeCount; i++)
                this.types[i] = ComponentType.Exclude(i);
        }

        internal readonly ComponentType this[int index] => types[index];

        internal readonly bool IsNull => !types.Any(type => type.AccessModeType != ComponentType.AccessMode.Exclude);

        internal readonly void SetComponentType(ComponentType type)
        {
            types[type.TypeIndex] = type;
        }

        internal bool Match(EntityQuery query)
        {
            for (int i = 0; i < query.TypesCount; i++)
            {
                if (types[query[i].TypeIndex] != query[i])
                    return false;
            }

            return true;
        }

        public bool Equals(EntityArchetype other)
        {
            return Match(other);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityArchetype other && Equals(other);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator EntityQuery(EntityArchetype archetype)
        {
            return new EntityQuery(archetype.types);
        }

        public static bool operator ==(EntityArchetype lhs, EntityArchetype rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(EntityArchetype lhs, EntityArchetype rhs)
        {
            return !lhs.Equals(rhs);
        }
    }
}
