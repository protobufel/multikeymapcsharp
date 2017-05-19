using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions
{
    internal static class LiteSetMultimapExtensions
    {
        public static ILiteSetMultimap<K, V> ToSetMultimap<K, V>(this IDictionary<K, ISet<V>> map, IEqualityComparer<V> valueComparer)
        {
            return new LiteSetMultimap<K, V>(map, valueComparer);
        }

        public static ILiteSetMultimap<K, V> ToSetMultimap<K, V>(this IDictionary<K, ISet<V>> map)
        {
            return ToSetMultimap(map, EqualityComparer<V>.Default);
        }

        internal class LiteSetMultimap<K, V> : ILiteSetMultimap<K, V>
        {
            protected readonly IDictionary<K, ISet<V>> map;
            protected readonly IEqualityComparer<V> valueComparer;

            public LiteSetMultimap(IDictionary<K, ISet<V>> map, IEqualityComparer<V> valueComparer)
            {
                this.map = map;
                this.valueComparer = valueComparer;
            }

            public virtual void Clear()
            {
                map.Clear();
            }

            public virtual bool TryGetValue(K key, out ISet<V> value)
            {
                return map.TryGetValue(key, out value);
            }

            public virtual bool Add(K key, V value)
            {
                if (map.TryGetValue(key, out ISet<V> col))
                {
                    col.Add(value);
                    return true;
                }

                col = new HashSet<V>(valueComparer) { value };
                map.Add(key, col);
                return false;
            }

            public virtual bool Remove(K key, V value)
            {
                return Remove(key, value, out bool removedEntireKey);
            }

            public virtual bool Remove(K key, V value, out bool removedEntireKey)
            {
                if (map.TryGetValue(key, out ISet<V> col) && col.Remove(value))
                {
                    if (col.Count == 0)
                    {
                        map.Remove(key);
                        removedEntireKey = true;
                    }

                    removedEntireKey = false;
                    return true;
                }

                removedEntireKey = false;
                return false;
            }

            public virtual int Count
            {
                get
                {
                    return map.Count;
                }
            }

            public virtual IEqualityComparer<V> ValueComparer => valueComparer;
        }
    }

    internal static class EnumerableEqualityExtensionsons
    {
        public static IEqualityComparer<K> EnumerableEqualityComparerOf<T, K>(this IEqualityComparer<T> elementComparer) where K : IEnumerable<T>
        {
            return new EnumerableEqualityComparer<T, K>(elementComparer);
        }

        public static IEqualityComparer<K> EnumerableEqualityComparerOf<T, K>() where K : IEnumerable<T>
        {
            return new EnumerableEqualityComparer<T, K>();
        }
    }

    [Serializable]
    internal class EnumerableEqualityComparer<T, K> : EqualityComparer<K> where K : IEnumerable<T>
    {
        private IEqualityComparer<T> elementComparer;

        public EnumerableEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        public EnumerableEqualityComparer() : this(EqualityComparer<T>.Default)
        {
        }

        public override bool Equals(K col1, K col2)
        {
            return Enumerable.SequenceEqual(col1, col2, elementComparer);
        }

        public override int GetHashCode(K col)
        {
            //return StructuralComparisons.StructuralEqualityComparer.GetHashCode(col);
            if (col == null)
            {
                return 0;
            }

            int hash = 5;

            unchecked
            {
                foreach (var item in col)
                {
                    hash = hash * 37 + elementComparer.GetHashCode(item);
                }
            }

            return hash;
        }
    }

    internal static class BitArrayExtensions
    {

        public static void SetAndResize(this BitArray fields, int position, bool value)
        {
            if (position >= fields.Length)
            {
                fields.Length = position + 1;
            }

            fields.Set(position, value);
        }

        public static BitArray ToBitArray(this IList<int> list)
        {
            BitArray fields = new BitArray(32);

            foreach (int field in list)
            {
                if (field >= 0) SetAndResize(fields, field, true);
            }

            return fields;
        }
    }
}
