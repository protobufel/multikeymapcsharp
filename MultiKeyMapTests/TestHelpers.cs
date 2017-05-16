using System;
using System.Collections.Generic;
using System.Linq;
using GitHub.Protobufel.MultiKeyMap;

namespace MultiKeyMapTests
{
    public static class TestHelpers
    {
        #region creation helpers
        public static IMultiKeyMap<T, K, V> CreateMultiKeyMap<T, K, V>(K[] keys, V[] values) where K : IEnumerable<T>
        {
            return CreateMultiKeyMap<T, K, V>(keys, values, () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>());
        }

        public static IMultiKeyMap<T, K, V> CreateMultiKeyMap<T, K, V>(K[] keys, V[] values, Func<IMultiKeyMap<T, K, V>> supplier) where K : IEnumerable<T>
        {
            var map = supplier.Invoke();

            foreach (var entry in keys.Zip(values, (k, v) => new KeyValuePair<K, V>(k, v)))
            {
                map.Add(entry);
            }

            return map;
        }

        public static IDictionary<K, V> CreateDictionary<T, K, V>(K[] keys, V[] values) where K : IEnumerable<T>
        {
            var map = keys.Zip(values, (k, v) => new KeyValuePair<K, V>(k, v)).ToDictionary(entry => entry.Key, entry => entry.Value);
            return map;
        }
        #endregion
    }
}
