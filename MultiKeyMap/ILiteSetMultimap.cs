using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    public interface ILiteSetMultimap<K, V>
    {
        void Clear();

        bool Remove(K key, V value);

        int Count { get; }

        bool TryGetValue(K key, out ISet<V> value);

        bool Add(K key, V value);

        IEqualityComparer<V> ValueComparer { get; }
    }
}
