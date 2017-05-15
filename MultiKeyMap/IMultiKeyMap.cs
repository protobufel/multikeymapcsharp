using System;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    /// <summary>
    /// Represents a generic IDictionary of composite keys and the methods to query it by any combination of sub-keys in any order.  
    /// </summary>
    /// <typeparam name="T">The type of sub-keys to query by and compose the full keys of</typeparam>
    /// <typeparam name="K">The type of the composite keys comprising some enumerable of sub-keys of type T</typeparam>
    /// <typeparam name="V">The type of values in the dictionary</typeparam>
    public interface IMultiKeyMap<T, K, V> : IDictionary<K, V> where K : IEnumerable<T>
    {
        /// <summary>
        /// Gets all values  for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <returns>A non-live non-null, possibly empty, sequence of the values satisfying the partial key criteria.</returns>
        [Obsolete("This method is obsolete. Call TryGetValuesByPartialKey instead.", false)]
        IEnumerable<V> GetValuesByPartialKey(IEnumerable<T> partialKey);

        /// <summary>
        /// Gets all KeyValuePairs for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <returns>A non-live non-null, possibly empty, sequence of the KeyValuePair(-s) satisfying the partial key criteria.</returns>
        [Obsolete("This method is obsolete. Call TryGetEntriesByPartialKey instead.", false)]
        IEnumerable<KeyValuePair<K, V>> GetEntriesByPartialKey(IEnumerable<T> partialKey);

        /// <summary>
        /// Gets all full keys that contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <returns>A non-live non-null, possibly empty, sequence of the full keys satisfying the partial key criteria.</returns>
        [Obsolete("This method is obsolete. Call TryGetFullKeysByPartialKey instead.", false)]
        IEnumerable<K> GetFullKeysByPartialKey(IEnumerable<T> partialKey);

        /// <summary>
        /// Gets all values  for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <param name="values">A non-live non-empty sequence of the values satisfying the partial key criteria, or the default value of the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        bool TryGetValuesByPartialKey(IEnumerable<T> partialKey, out ICollection<V> values);

        /// <summary>
        /// Gets all KeyValuePairs for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <param name="entries">A non-live non-empty sequence of the KeyValuePair(-s) satisfying the partial key criteria, or the default value of the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        bool TryGetEntriesByPartialKey(IEnumerable<T> partialKey, out ICollection<KeyValuePair<K, V>> entries);

        /// <summary>
        /// Gets all full keys that contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <param name="fullKeys">A non-live non-empty set of the full keys satisfying the partial key criteria, or the default value of the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out ISet<K> fullKeys);

        /// <summary>
        /// Gets all values  for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for, wherein each sub-key is a Tuple of the position and the sub-key itself.
        /// The negative position signifies non-positional sub-key to search anywhere within the full key, otherwise, its exact position within the full key. 
        /// </param>
        /// <param name="values">A non-live non-empty sequence of the values satisfying the partial key criteria, or the default value of the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        bool TryGetValuesByPartialKey(IEnumerable<(int position, T subkey)> partialKey, out ICollection<V> values);

        /// <summary>
        /// Gets all KeyValuePairs for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for, wherein each sub-key is a Tuple of the position and the sub-key itself.
        /// The negative position signifies non-positional sub-key to search anywhere within the full key, otherwise, its exact position within the full key. 
        /// </param>
        /// <param name="entries">A non-live non-empty sequence of the KeyValuePair(-s) satisfying the partial key criteria, or the default value of the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        bool TryGetEntriesByPartialKey(IEnumerable<(int position, T subkey)> partialKey, out ICollection<KeyValuePair<K, V>> entries);

        /// <summary>
        /// Gets all full keys that contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for, wherein each sub-key is a Tuple of the position and the sub-key itself.
        /// The negative position signifies non-positional sub-key to search anywhere within the full key, otherwise, its exact position within the full key. 
        /// </param>
        /// <param name="fullKeys">A non-live non-empty set of the full keys satisfying the partial key criteria, or the default value of the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        bool TryGetFullKeysByPartialKey(IEnumerable<(int position, T subkey)> partialKey, out ISet<K> fullKeys);
    }
}
