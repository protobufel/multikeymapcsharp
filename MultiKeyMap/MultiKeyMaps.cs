using System;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap.Dictionaries;

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
        /// <param name="creationStrategy">The type of MultiKeyMap to create; OptimizedForNonPositionalSearch by default
        /// <see cref="MultiKeyCreationStrategy.OptimizedForNonPositionalSearch"/></param>
        /// <returns>The serializable instance of the Dictionary-based IMultiKeyMap implementation.</returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyDictionary<T, K, V>(
            MultiKeyCreationStrategy creationStrategy = MultiKeyCreationStrategy.OptimizedForNonPositionalSearch) where K : class, IEnumerable<T>
        {
            return CreateMultiKeyDictionary<T, K, V>(EqualityComparer<T>.Default,
                        EqualityComparer<T>.Default.EnumerableEqualityComparerOf<T, K>(), creationStrategy);
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
        /// <param name="creationStrategy"></param>
        /// <returns>The serializable instance of the Dictionary-based IMultiKeyMap implementation.</returns>
        public static IMultiKeyMap<T, K, V> CreateMultiKeyDictionary<T, K, V>(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer,
            MultiKeyCreationStrategy creationStrategy = MultiKeyCreationStrategy.OptimizedForNonPositionalSearch) where K : class, IEnumerable<T>
        {
            if (subKeyComparer == null) throw new ArgumentNullException("subKeyComparer");
            if (fullKeyComparer == null) throw new ArgumentNullException("fullKeyComparer");

            switch (creationStrategy)
            {
                case MultiKeyCreationStrategy.OptimizedForNonPositionalSearch:
                    return new DictionaryNonPositionalMultiKeyMap<T, K, V>(subKeyComparer, fullKeyComparer);
                case MultiKeyCreationStrategy.OptimizedForPositionalSearch:
                    return new DictionaryMaskMultiKeyMap<T, K, V>(subKeyComparer, fullKeyComparer);
                // return new DictionaryBasePosMaskMultiKeyMap<T, K, V>(subKeyComparer, fullKeyComparer);
                default:
                    throw new ArgumentOutOfRangeException("creationStrategy");
            }
        }

        /// <summary>
        /// The MultiKeyMap creation strategy. In most cases, OptimizedForNonPositionalSearch would be the most general case.  
        /// </summary>
        public enum MultiKeyCreationStrategy
        {
            /// <summary>
            /// The created MultiKeyMap would work best with non-positional sub-keys, and will further constrain positional 
            /// sub-keys during search by partial key.
            /// </summary>
            OptimizedForNonPositionalSearch,
            /// <summary>
            /// The created MultiKeyMap would work best with positional sub-keys, but would be slower for non-positional 
            /// sub-keys during search by partial key. .
            /// </summary>
            OptimizedForPositionalSearch
        }
    }
}
