using System;
using System.Collections.Generic;

namespace MultiKeyMap
{
    public interface IMultiKeyMap<T, K, V> : IDictionary<K, V> where K : IEnumerable<T>
    {

        IEnumerable<V> GetValuesByPartialKey(IEnumerable<T> partialKey);

        IEnumerable<KeyValuePair<K, V>> GetEntriesByPartialKey(IEnumerable<T> partialKey);

        ISet<K> GetFullKeysByPartialKey(IEnumerable<T> partialKey);
    }
}
