using System;

namespace Entities
{
    public static class ArrayHelper
    {
        public static void Sort<T>(this T[] array, Func<T, T, int> func)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (func.Invoke(array[j], array[i]) < 0)
                    {
                        var temp = array[j];
                        array[j] = array[i];
                        array[i] = temp;
                    }
                }
            }
        }

        public static T[] Insert<T>(this T[] array, int index, T item)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            for (int i = array.Length; i > index; i--)
                newArray[i] = newArray[i - 1];
            newArray[index] = item;
            return newArray;
        }

        public static T[] Delete<T>(this T[] array, int index)
        {
            if (array.Length > 1)
            {
                for (int i = index; i < array.Length - 1; i++)
                    array[i] = array[i + 1];
                T[] newArray = new T[array.Length - 1];
                Array.Copy(array, newArray, array.Length - 1);
                return newArray;
            }

            return null;
        }

        public static T[] Expand<T>(this T[] array, T item)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = item;
            return newArray;
        }

        public static bool Find<T>(this T[] array, out T ret, Func<T, bool> func)
        {
            ret = default;
            for (int i = 0; i < array.Length; i++)
            {
                if (func(array[i]))
                {
                    ret = array[i];
                    return true;
                }
            }
            return false;
        }

        public static bool Contains<T>(this T[] array, T item)
        {
            return array.IndexOf(item) >= 0;
        }

        public static int IndexOf<T>(this T[] array, T item)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(item))
                    return i;
            }

            return -1;
        }
    }

    public struct EntityQuery
    {
        public ComponentType[] types;

        public int TypesCount => types == null ? 0 : types.Length;

        public EntityQuery(params ComponentType[] types)
        {
            this.types = types;
        }

        public ComponentType this[int index] => types[index];
    }

    [Serializable]
    public struct EntityArchetype : IEquatable<EntityArchetype>
    {
        internal ComponentType[] types;

        public readonly int TypesCount => types == null ? 0 : types.Length;

        public readonly bool Valid => TypesCount > 0;

        public ComponentType this[int index] => types[index];

        public EntityArchetype(params ComponentType[] types)
        {
            if (types != null)
            {
                this.types = new ComponentType[types.Length];
                for (int i = 0; i < types.Length; i++)
                    this.types[i] = types[i];
                this.types.Sort((a, b) => a < b ? -1 : a == b ? 0 : 1);
            }
            else
            {
                this.types = null;
            }
        }

        public void AddComponentType(ComponentType type)
        {
            if (TypesCount == 0)
            {
                types = new ComponentType[] { type };
            }
            else
            {
                for (int i = 0; i < TypesCount; i++)
                {
                    if (types[i] == type)
                        throw new Exception($"ComponentType {type} already exists in archetype");

                    if (types[i] < type)
                        continue;

                    types = types.Insert(i, type);
                    return;
                }
                
                types = types.Expand(type);
            }
        }

        public void RemoveComponentType(ComponentType type)
        {
            for (int i = 0; i < TypesCount; i++)
            {
                if (types[i] == type)
                {
                    types = types.Delete(i);
                    break;
                }
            }
        }

        public bool TryGetComponentType(int typeIndex, out ComponentType type)
        {
            type = default;
            if (types != null && types.Find(out type, t => t.TypeIndex == typeIndex))
                return true;

            return false;
        }

        public bool Contains(ComponentType other)
        {
            return types != null && types.Contains(other);
        }

        public bool Contains(EntityArchetype other)
        {
            if (other.types == null)
                return false;

            foreach (var t in other.types)
            {
                if (!Contains(t))
                    return false;
            }

            return true;
        }

        public bool Equals(EntityArchetype other)
        {
            if (TypesCount == other.TypesCount)
            {
                for (int i = 0; i < TypesCount; i++)
                {
                    if (types[i] != other.types[i])
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
