using System;
using System.Linq;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap.Extensions
{
    /// <summary>
    /// Provides MultiKeyMap copy extensions
    /// </summary>
    public static class MultiKeyMapCopyExtensions
    {
        /// <summary>
        /// Copies (adds) all KeyValuePair(-s) from the other IMultiKeyMap into this object.
        /// NOTE: The keys and values are not cloned, but just simply added as-is.
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="me">this object</param>
        /// <param name="other">the source of the data to copy from</param>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> CopyFrom<T, K, V>(this IMultiKeyMap<T, K, V> me, IMultiKeyMap<T, K, V> other) where K : IEnumerable<T>
        {
            other = other ?? throw new ArgumentNullException();

            foreach (var entry in other)
            {
                me.Add(entry.Key, entry.Value);
            }

            return me;
        }

        /// <summary>
        /// Clears itself and copies all KeyValuePair(-s) from the other IMultiKeyMap over.
        /// NOTE: The keys and values are not cloned, but just simply added as-is.
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="me">this object</param>
        /// <param name="other">the source of the data to copy from</param>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> Of<T, K, V>(this IMultiKeyMap<T, K, V> me, IMultiKeyMap<T, K, V> other) where K : IEnumerable<T>
        {
            other = other ?? throw new ArgumentNullException();
            me.Clear();

            foreach (var entry in other)
            {
                me.Add(entry.Key, entry.Value);
            }

            return me;
        }

        /// <summary>
        /// Copies (adds) all KeyValuePair(-s) from the other IMultiKeyMap into this object.
        /// NOTE: The keys and values are not cloned, but just simply added as-is.
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="me">this object</param>
        /// <param name="other">the source of the data to copy from</param>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> CopyFrom<T, K, V>(this IMultiKeyMap<T, K, V> me, IDictionary<K, V> other) where K : IEnumerable<T>
        {
            other = other ?? throw new ArgumentNullException();

            foreach (var entry in other)
            {
                me.Add(entry.Key, entry.Value);
            }

            return me;
        }

        /// <summary>
        /// Clears itself and copies all KeyValuePair(-s) from the other IMultiKeyMap over.
        /// NOTE: The keys and values are not cloned, but just simply added as-is.
        /// </summary>
        /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
        /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
        /// <typeparam name="V">The type of values in the dictionary</typeparam>
        /// <param name="me">this object</param>
        /// <param name="other">the source of the data to copy from</param>
        /// <returns></returns>
        public static IMultiKeyMap<T, K, V> Of<T, K, V>(this IMultiKeyMap<T, K, V> me, IDictionary<K, V> other) where K : IEnumerable<T>
        {
            other = other ?? throw new ArgumentNullException();
            me.Clear();

            foreach (var entry in other)
            {
                me.Add(entry.Key, entry.Value);
            }

            return me;
        }
    }

    /// <summary>
    /// Provides IMultiKeyMap interface extensions
    /// </summary>
    public static class MultiKeyMapInterfaceExtensions
    {
        /// <summary>
        /// Gets all values  for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for, wherein each sub-key is a Tuple of the position and the sub-key itself.
        /// The negative position signifies non-positional sub-key to search anywhere within the full key, otherwise, its exact position within the full key. 
        /// </param>
        /// <param name="values">A non-live non-empty sequence of the values satisfying the partial key criteria, or the default value of the result 
        /// type if not found.</param>
        /// <param name="me">this object</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        public static bool TryGetValuesByPartialKey<T, K, V>(this IMultiKeyMap<T, K, V> me, IEnumerable<(int position, T subkey)> partialKey,
            out IEnumerable<V> values) where K : IEnumerable<T>
        {
            if (partialKey == null) throw new ArgumentNullException("partialKey");
            return me.TryGetValuesByPartialKey(partialKey.Select(t => t.subkey), partialKey.Select(t => t.position), out values);
        }

        /// <summary>
        /// Gets all KeyValuePairs for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for, wherein each sub-key is a Tuple of the position and the sub-key itself.
        /// The negative position signifies non-positional sub-key to search anywhere within the full key, otherwise, its exact position within the full key. 
        /// </param>
        /// <param name="entries">A non-live non-empty sequence of the KeyValuePair(-s) satisfying the partial key criteria, or the default value of the 
        /// result type if not found.</param>
        /// <param name="me">this object</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        public static bool TryGetEntriesByPartialKey<T, K, V>(this IMultiKeyMap<T, K, V> me, IEnumerable<(int position, T subkey)> partialKey,
            out IEnumerable<KeyValuePair<K, V>> entries) where K : IEnumerable<T>
        {
            if (partialKey == null) throw new ArgumentNullException("partialKey");
            return me.TryGetEntriesByPartialKey(partialKey.Select(t => t.subkey), partialKey.Select(t => t.position), out entries);
        }

        /// <summary>
        /// Gets all full keys that contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for, wherein each sub-key is a Tuple of the position and the sub-key itself.
        /// The negative position signifies non-positional sub-key to search anywhere within the full key, otherwise, its exact position within the full key. 
        /// </param>
        /// <param name="fullKeys">A non-live non-empty set of the full keys satisfying the partial key criteria, or the default value of the result 
        /// type if not found.</param>
        /// <param name="me">this object</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        public static bool TryGetFullKeysByPartialKey<T, K, V>(this IMultiKeyMap<T, K, V> me, IEnumerable<(int position, T subkey)> partialKey,
            out IEnumerable<K> fullKeys) where K : IEnumerable<T>
        {
            if (partialKey == null) throw new ArgumentNullException("partialKey");
            return me.TryGetFullKeysByPartialKey(partialKey.Select(t => t.subkey), partialKey.Select(t => t.position), out fullKeys);
        }
    }
}
