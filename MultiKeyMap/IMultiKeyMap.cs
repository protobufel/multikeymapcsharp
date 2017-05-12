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
        /// <returns>A non-live non-null, possibly empty, sequence of values satisfying the partial key criteria.</returns>
        IEnumerable<V> GetValuesByPartialKey(IEnumerable<T> partialKey);

        /// <summary>
        /// Gets all KeyValuePairs for which their full keys contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <returns>A non-live non-null, possibly empty, sequence of KeyValuePairs satisfying the partial key criteria.</returns>
        IEnumerable<KeyValuePair<K, V>> GetEntriesByPartialKey(IEnumerable<T> partialKey);

        /// <summary>
        /// Gets all full keys that contain the partial key set in any order.
        /// </summary>
        /// <param name="partialKey">The combination of the sub-keys to search for.</param>
        /// <returns>A non-live non-null, possibly empty, sequence of full keys satisfying the partial key criteria.</returns>
        IEnumerable<K> GetFullKeysByPartialKey(IEnumerable<T> partialKey);
    }
}
