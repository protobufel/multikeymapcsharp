using System.Collections;
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
        /// <returns>The serializable instance of the Dictionary-based IMultiKeyMap implementation.</returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyDictionary<T, K, V>() where K : IEnumerable<T>
        {
            return new DictionaryBasePosMultiKeyMap<T, K, V>(EqualityComparer<T>.Default, EqualityComparer<T>.Default.EnumerableEqualityComparerOf<T, K>());
        }

        /// <summary>
        /// Gets a Dictionary based serializable implementation of the generic GitHub.Protobufel.MultiKeyMap.IMultiKeyMap, customized with the provided 
        /// subKey and full key IEqualityComparer(-s).
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="subKeyComparer">The custom IEqualityComparer for sub-keys.</param>
        /// <param name="fullKeyComparer">The custom IEqualityComparer for full keys.</param>
        /// <returns>The serializable instance of the Dictionary-based IMultiKeyMap implementation.</returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyDictionary<T, K, V>(IEqualityComparer<T> subKeyComparer,
            IEqualityComparer<K> fullKeyComparer) where K : IEnumerable<T>
        {
            return new DictionaryBasePosMultiKeyMap<T, K, V>(subKeyComparer, fullKeyComparer);
        }
    }
}
