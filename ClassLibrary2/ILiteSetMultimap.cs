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
}
