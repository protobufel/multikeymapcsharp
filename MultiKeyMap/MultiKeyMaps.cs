using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;
using GitHub.Protobufel.MultiKeyMap.Extensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    /// <summary>
    /// Provides factory methods of the generic GitHub.Protobufel.MultiKeyMap.IMultiKeyMap's implementations.
    /// The parameterless CreateMultiKeyDictionary() provides a good all-round default implementation based on System.Collection.Generic.Dictionary.
    /// </summary>
    public static class MultiKeyMaps
    {
        /// <summary>
        /// Provides a good all-round default implementation of the generic GitHub.Protobufel.MultiKeyMap.IMultiKeyMap based on 
        /// System.Collection.Generic.Dictionary, using the default equality comparers for its types. 
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyDictionary<T, K, V>() where K : IEnumerable<T>
        {
            IDictionary<K, V> fullMap = new Dictionary<K, V>(EqualityComparer<T>.Default.EnumerableEqualityComparerOf<T, K>());
            IDictionary<T, ISet<K>> basePartMap = new Dictionary<T, ISet<K>>(EqualityComparer<T>.Default);
            return new BaseMultiKeyMap<T, K, V>(fullMap, basePartMap.ToSetMultimap());
        }

        /// <summary>
        /// Gets a customized implementation of the generic GitHub.Protobufel.MultiKeyMap.IMultiKeyMap based on the provided IDictionary concrete instances.
        /// These instances used as-is, not cloned, and assumed to be empty. Therefore, it is important to set the desired IEqualityComparer(-s) accordingly,
        /// and not use these instances anywhere else!
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="fullMap">The desired empty instance of IDictionary implementation with the properly set IEqualityComparer. 
        /// Used as the basis for the underlying main Dictionary.</param>
        /// <param name="basePartMap">The desired empty instance of IDictionary implementation with the properly set IEqualityComparer. 
        /// Used as the basis for the underlying partial key based Dictionary</param>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyMap<T, K, V>(IDictionary<K,V> fullMap, IDictionary<T, ISet<K>> basePartMap) where K : IEnumerable<T>
        {
            return new BaseMultiKeyMap<T, K, V>(fullMap, basePartMap.ToSetMultimap());
        }

        /// <summary>
        /// Gets a Dictionary based implementation of the generic GitHub.Protobufel.MultiKeyMap.IMultiKeyMap, customized with the provided 
        /// subKey and full key IEqualityComparer(-s).
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="subKeyComparer">The custom IEqualityComparer for sub-keys.</param>
        /// <param name="fullKeyComparer">The custom IEqualityComparer for full keys.</param>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyDictionary<T, K, V>(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) where K : IEnumerable<T>
        {
            IDictionary<K, V> fullMap = new Dictionary<K, V>(fullKeyComparer);
            IDictionary<T, ISet<K>> basePartMap = new Dictionary<T, ISet<K>>(subKeyComparer);
            return new BaseMultiKeyMap<T, K, V>(fullMap, basePartMap.ToSetMultimap());
        }
    }
}
