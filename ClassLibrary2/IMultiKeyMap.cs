using System;
using System.Collections.Generic;

namespace MultiKeyMap
{
    interface IMultiKeyMap<T, K, V> : IDictionary<K, V> where T : IEquatable<T> where K : IEnumerable<T>
    {

        IEnumerable<V> GetValuesByPartialKey(IEnumerable<T> partialKey);

        IEnumerable<KeyValuePair<K, V>> GetEntriesByPartialKey(IEnumerable<T> partialKey);

        ISet<K> GetFullKeysByPartialKey(IEnumerable<T> partialKey);
    }
}
