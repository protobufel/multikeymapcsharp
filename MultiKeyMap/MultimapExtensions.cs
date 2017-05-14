using System;
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
}
