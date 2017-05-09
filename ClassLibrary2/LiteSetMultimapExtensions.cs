using System.Collections.Generic;

namespace MultiKeyMap.Extensions
{
    public static class LiteSetMultimapExtensions
    {
        public static ILiteSetMultimap<K, V> ToSetMultiMap<K, V>(this IDictionary<K, ISet<V>> map)
        {
            return new LiteSetMultimap<K, V>(map);
        }

        private class LiteSetMultimap<K, V> : ILiteSetMultimap<K, V>
        {
            private readonly IDictionary<K, ISet<V>> map;

            public LiteSetMultimap(IDictionary<K, ISet<V>> map)
            {
                this.map = map;
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

                col = new HashSet<V>() { value };
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
        }
    }

    public static class EnumerableEqualityExtensionsons
    {
       public static  IEqualityComparer<K> EnumerableEqualityComparerOf<T, K>(this IEqualityComparer<T> elementComparer) where K : IEnumerable<T>
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
            IEnumerator<T> it1 = col1.GetEnumerator();
            IEnumerator<T> it2 = col2.GetEnumerator();
            bool isNext1 = false;
            bool isNext2 = false;

            while ((isNext1 = it1.MoveNext()) & (isNext2 = it2.MoveNext()))
            {
                if (!elementComparer.Equals(it1.Current, it2.Current))
                {
                    return false;
                }
            }

            return (isNext1 == isNext2);
       }

        public override int GetHashCode(K col)
        {
            int hash = 5;

            unchecked
            {
                foreach (var item in col)
                {
                    hash = 37 * hash + elementComparer.GetHashCode(item);
                }
            }

            return hash;
        }
    }
}
