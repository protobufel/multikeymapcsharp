using System.Collections.Generic;

namespace MultiKeyMap
{
    public interface ILiteSetMultimap<K, V>
    {
        void Clear();

        bool Remove(K key, V value);

        int Count { get; }

        bool TryGetValue(K key, out ISet<V> value);

        bool Add(K key, V value);
    }

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
}
