﻿using System.Collections.Generic;
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

        private class LiteSetMultimap<K, V> : ILiteSetMultimap<K, V>
        {
            private readonly IDictionary<K, ISet<V>> map;
            private readonly IEqualityComparer<V> valueComparer;

            public LiteSetMultimap(IDictionary<K, ISet<V>> map, IEqualityComparer<V> valueComparer)
            {
                this.map = map;
                this.valueComparer = valueComparer;
            }

            public void Clear()
            {
                map.Clear();
            }

            public bool TryGetValue(K key, out ISet<V> value)
            {
                return map.TryGetValue(key, out value);
            }

            public bool Add(K key, V value)
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

            public bool Remove(K key, V value)
            {
                if (map.TryGetValue(key, out ISet<V> col) && col.Remove(value))
                {
                    if (col.Count == 0)
                    {
                        map.Remove(key);
                    }

                    return true;
                }

                return false;
            }

            public int Count
            {
                get
                {
                    return map.Count;
                }
            }

            public IEqualityComparer<V> ValueComparer => valueComparer;
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
}
