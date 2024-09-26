using System;
using System.Text;

namespace Entities
{
    [Serializable]
    public partial struct ComponentType : IEquatable<ComponentType>
    {
        public enum AccessMode
        {
            ReadWrite,
            // ReadOnly,
            Exclude,
        }

        public int TypeIndex;
        public AccessMode AccessModeType;

        public static ComponentType ReadWrite<T>()
        {
            return FromTypeIndex(TypeManager.GetTypeIndex<T>());
        }

        public static ComponentType ReadWrite(Type type)
        {
            return FromTypeIndex(TypeManager.GetTypeIndex(type));
        }

        public static ComponentType ReadWrite(int typeIndex)
        {
            return FromTypeIndex(typeIndex);
        }

        public static ComponentType FromTypeIndex(int typeIndex)
        {
            ComponentType type;
            type.TypeIndex = typeIndex;
            type.AccessModeType = AccessMode.ReadWrite;
            return type;
        }

        // public static ComponentType ReadOnly(Type type)
        // {
        //    ComponentType t = FromTypeIndex(TypeManager.GetTypeIndex(type));
        //    t.AccessModeType = AccessMode.ReadOnly;
        //    return t;
        // }

        // public static ComponentType ReadOnly(int typeIndex)
        // {
        //    ComponentType t = FromTypeIndex(typeIndex);
        //    t.AccessModeType = AccessMode.ReadOnly;
        //    return t;
        // }

        // public static ComponentType ReadOnly<T>()
        // {
        //    ComponentType t = ReadWrite<T>();
        //    t.AccessModeType = AccessMode.ReadOnly;
        //    return t;
        // }

        public static ComponentType Exclude(Type type)
        {
            return Exclude(TypeManager.GetTypeIndex(type));
        }

        public static ComponentType Exclude(int typeIndex)
        {
            ComponentType t = FromTypeIndex(typeIndex);
            t.AccessModeType = AccessMode.Exclude;
            return t;
        }

        public static ComponentType Exclude<T>()
        {
            return Exclude(TypeManager.GetTypeIndex<T>());
        }

        public ComponentType(Type type, AccessMode accessModeType = AccessMode.ReadWrite)
        {
            TypeIndex = TypeManager.GetTypeIndex(type);
            AccessModeType = accessModeType;
        }

        public readonly Type GetManagedType()
        {
            return TypeManager.GetType(TypeIndex);
        }

        public static implicit operator ComponentType(Type type)
        {
            return new ComponentType(type, AccessMode.ReadWrite);
        }

        public static implicit operator Type(ComponentType type)
        {
            return type.GetManagedType();
        }

        public static bool operator <(ComponentType lhs, ComponentType rhs)
        {
            if (lhs.TypeIndex == rhs.TypeIndex)
                return lhs.AccessModeType < rhs.AccessModeType;

            return lhs.TypeIndex < rhs.TypeIndex;
        }

        public static bool operator >(ComponentType lhs, ComponentType rhs)
        {
            return rhs < lhs;
        }

        public static bool operator ==(ComponentType lhs, ComponentType rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ComponentType lhs, ComponentType rhs)
        {
            return !lhs.Equals(rhs);
        }

        public bool Equals(ComponentType other)
        {
            return this.TypeIndex == other.TypeIndex && this.AccessModeType == other.AccessModeType;
        }

        public override bool Equals(object obj)
        {
            return obj is ComponentType type && Equals(type);
        }

        public override int GetHashCode()
        {
            return TypeIndex * 5813;
        }

        public override string ToString()
        {
            var info = TypeManager.GetTypeInfo(TypeIndex);
            StringBuilder ns = new StringBuilder();
            ns.Append(info.DebugTypeName);

            if (AccessModeType == AccessMode.Exclude)
                ns.Append(" [Exclude]");

            return ns.ToString();

        }
    }
}
