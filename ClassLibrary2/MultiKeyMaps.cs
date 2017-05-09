using System.Collections.Generic;
using MultiKeyMap.Extensions;

namespace MultiKeyMap
{
    public static class MultiKeyMaps
    {
        public static IMultiKeyMap<T, K, V> NewMultiKeyDictionary<T, K, V>() where K : IEnumerable<T>
        {
            IDictionary<K, V> fullMap = new Dictionary<K, V>(EqualityComparer<T>.Default.EnumerableEqualityComparerOf<T, K>());
            IDictionary<T, ISet<K>> basePartMap = new Dictionary<T, ISet<K>>(EqualityComparer<T>.Default);
            return new BaseMultiKeyMap<T, K, V>(fullMap, basePartMap.ToSetMultiMap());
        }

        public static IMultiKeyMap<T, K, V> NewMultiKeyMap<T, K, V>(IDictionary<K,V> fullMap, IDictionary<T, ISet<K>> basePartMap, IEqualityComparer<T> comparer) where K : IEnumerable<T>
        {
            return new BaseMultiKeyMap<T, K, V>(fullMap, basePartMap.ToSetMultiMap());
        }

        public static IMultiKeyMap<T, K, V> NewMultiKeyDictionary<T, K, V>(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) where K : IEnumerable<T>
        {
            IDictionary<K, V> fullMap = new Dictionary<K, V>(fullKeyComparer);
            IDictionary<T, ISet<K>> basePartMap = new Dictionary<T, ISet<K>>(subKeyComparer);
            return new BaseMultiKeyMap<T, K, V>(fullMap, basePartMap.ToSetMultiMap());
        }
    }
}
