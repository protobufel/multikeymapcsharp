using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    internal interface ILiteSetMultimap<K, V>
    {
        void Clear();

        bool Remove(K key, V value);

        bool Remove(K key, V value, out bool removedEntireKey);

        int Count { get; }

        bool TryGetValue(K key, out ISet<V> value);

        bool Add(K key, V value);
    }
}
