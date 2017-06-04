/*
Copyright 2017 David Tesler

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace GitHub.Protobufel.MultiKeyMap
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
        public static IMultiKeyMap<T, K, V> CopyFrom<T, K, V>(this IMultiKeyMap<T, K, V> me, IMultiKeyMap<T, K, V> other) where K : class, IEnumerable<T>
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
        public static IMultiKeyMap<T, K, V> Of<T, K, V>(this IMultiKeyMap<T, K, V> me, IMultiKeyMap<T, K, V> other) where K : class, IEnumerable<T>
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
        public static IMultiKeyMap<T, K, V> CopyFrom<T, K, V>(this IMultiKeyMap<T, K, V> me, IDictionary<K, V> other) where K : class, IEnumerable<T>
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
        public static IMultiKeyMap<T, K, V> Of<T, K, V>(this IMultiKeyMap<T, K, V> me, IDictionary<K, V> other) where K : class, IEnumerable<T>
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
            out IEnumerable<V> values) where K : class, IEnumerable<T>
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
            out IEnumerable<KeyValuePair<K, V>> entries) where K : class, IEnumerable<T>
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
            out IEnumerable<K> fullKeys) where K : class, IEnumerable<T>
        {
            if (partialKey == null) throw new ArgumentNullException("partialKey");
            return me.TryGetFullKeysByPartialKey(partialKey.Select(t => t.subkey), partialKey.Select(t => t.position), out fullKeys);
        }
    }

    /// <summary>
    /// Provides IEqualityComparer extensions
    /// </summary>
    public static class EqualityComparerExtensions
    {
        /// <summary>
        /// Creates the sequence IEqualityComparer based on the supplied element comparer.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <typeparam name="K">The type of the sequence to generate the equality comparer for.</typeparam>
        /// <param name="elementComparer">The equality comparer of the target sequence's elements.</param>
        /// <returns>The equality comparer of the desired sequence of elements</returns>
        public static IEqualityComparer<K> EnumerableEqualityComparerOf<T, K>(this IEqualityComparer<T> elementComparer) where K : class, IEnumerable<T>
        {
            return new EnumerableEqualityComparer<T, K>(elementComparer);
        }

        /// <summary>
        /// Creates the sequence IEqualityComparer based on the default element comparer.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <typeparam name="K">The type of the sequence to generate the equality comparer for.</typeparam>
        /// <returns>The equality comparer of the desired sequence of elements</returns>
        public static IEqualityComparer<K> EnumerableEqualityComparerOf<T, K>() where K : class, IEnumerable<T>
        {
            return new EnumerableEqualityComparer<T, K>();
        }

        /// <summary>
        /// Returns the singleton reference IEqualityComparer of the type.
        /// </summary>
        /// <remarks>Only works for reference types. If the type is <see cref="string"/> then 
        /// uses <see cref="System.StringComparer.Ordinal"/>.</remarks>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <returns>The reference equality comparer of the desired type</returns>
        public static IEqualityComparer<T> ReferenceEqualityComparerOf<T>() where T : class
        {
            return (typeof(T) == typeof(string)) ? StringComparer.Ordinal as IEqualityComparer<T> : ReferenceEqualityComparer<T>.Default;
        }
    }

    [Serializable]
    internal class EnumerableEqualityComparer<T, K> : EqualityComparer<K> where K : class, IEnumerable<T>
    {
        private IEqualityComparer<T> elementComparer;

        public EnumerableEqualityComparer(IEqualityComparer<T> elementComparer)
        {
            this.elementComparer = elementComparer;
        }

        public EnumerableEqualityComparer() : this(EqualityComparer<T>.Default)
        {
        }

        public override bool Equals(K col1, K col2)
        {
            return Enumerable.SequenceEqual(col1, col2, elementComparer);
        }

        public override int GetHashCode(K col)
        {
            //return StructuralComparisons.StructuralEqualityComparer.GetHashCode(col);
            if (col == null)
            {
                return 0;
            }

            int hash = 5;

            unchecked
            {
                foreach (var item in col)
                {
                    hash = hash * 37 + elementComparer.GetHashCode(item);
                }
            }

            return hash;
        }
    }

    internal sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T> where T : class
    {
        internal static volatile ReferenceEqualityComparer<T> instance;

        internal static ReferenceEqualityComparer<T> Default
        {
            get
            {
                ReferenceEqualityComparer<T> comparer = instance;

                if (comparer == null)
                {
                    comparer = new ReferenceEqualityComparer<T>();
                    instance = comparer;
                }

                return comparer;
            }
        }

        private ReferenceEqualityComparer()
        {
        }

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}
