using System.Collections;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    //implementation: IDictionary<K, IDictionary<int, V>> and BitArray as a mask of included keys
    internal interface IPosSetMultimap<K, V>
    {
        void Clear();

        bool Remove(K key, int position, V value);

        int Count { get; }

        bool TryGetValue(K key, int position, out ISet<V> value, BitArray excludePositions = default(BitArray));

        bool Add(K key, int position, V value);
    }
}
