using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MultiKeyMap
{
    internal class BaseMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where T : IEquatable<T> where K : IEnumerable<T>
    {
        private IDictionary<K, V> fullMap;
        private ILiteSetMultimap<T, K> partMap;

        internal BaseMultiKeyMap(IDictionary<K, V> fullMap, ILiteSetMultimap<T, K> partMap, IEqualityComparer<T> comparer)
        {
            this.fullMap = fullMap;
            this.partMap = partMap;
        }

        #region IMultiKeyMap specific methods
        public IEnumerable<V> GetValuesByPartialKey(IEnumerable<T> partialKey)
        {
            ISet<K> fullKeys = GetFullKeysByPartialKey(partialKey);

            if (fullKeys.Count == 0)
            {
                return Enumerable.Empty<V>();
            }

            IList<V> result = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                fullMap.TryGetValue(fullKey, out V value);
                result.Add(value);
            }

            return result;
        }

        public IEnumerable<KeyValuePair<K, V>> GetEntriesByPartialKey(IEnumerable<T> partialKey)
        {
            ISet<K> fullKeys = GetFullKeysByPartialKey(partialKey);

            if (fullKeys.Count == 0)
            {
                return Enumerable.Empty<KeyValuePair<K, V>>();
            }

            IList<KeyValuePair<K, V>> result = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                fullMap.TryGetValue(fullKey, out V value);
                KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                result.Add(entry);
            }

            return result;
        }

        public ISet<K> GetFullKeysByPartialKey(IEnumerable<T> partialKey)
        {
            if (partMap.Count == 0)
            {
                return new HashSet<K>();
            }

            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (T subKey in partialKey)
            {
                if (!partMap.TryGetValue(subKey, out ISet<K> subResult))
                {
                    return new HashSet<K>();
                }
                else if (subResult.Count < minSize)
                {
                    minSize = subResult.Count;
                    minPos = subResults.Count;
                }

                subResults.Add(subResult);
            }

            if (subResults.Count == 0)
            {
                return new HashSet<K>();
            }

            ISet<K> result = new HashSet<K>(subResults[minPos]);

            if (subResults.Count == 1)
            {
                return result;
            }

            for (int i = 0; i < subResults.Count; i++)
            {
                if (i != minPos)
                {
                    result.IntersectWith(subResults[i]);

                    if (result.Count == 0)
                    {
                        return new HashSet<K>();
                    }
                }
            }

            return result;
        }
        #endregion

        #region Implementation of the partial map helpers
        private void AddPartial(K key)
        {
            foreach (T subKey in key)
            {
                partMap.Add(subKey, key);
            }
        }

        private void DeletePartial(K key)
        {
            foreach (T subKey in key)
            {
                partMap.Remove(subKey, key);
            }
        }
        #endregion

        public V this[K key] { get => fullMap[key]; set => fullMap[key] = value; }

        public ICollection<K> Keys => fullMap.Keys;

        public ICollection<V> Values => fullMap.Values;

        public int Count => fullMap.Count;

        public bool IsReadOnly => fullMap.IsReadOnly;

        public void Add(K key, V value)
        {
            fullMap.Add(key, value);
            AddPartial(key);
        }

        public void Add(KeyValuePair<K, V> item)
        {
            fullMap.Add(item);
            AddPartial(item.Key);
        }

        public void Clear()
        {
            fullMap.Clear();
        }

        public bool Contains(KeyValuePair<K, V> item)
        {
            return fullMap.Contains(item);
        }

        public bool ContainsKey(K key)
        {
            return fullMap.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            fullMap.CopyTo(array, arrayIndex);
        }

        public bool Remove(K key)
        {
            if (fullMap.Remove(key))
            {
                DeletePartial(key);
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<K, V> item)
        {
            if (fullMap.Remove(item))
            {
                DeletePartial(item.Key);
                return true;
            }

            return false;
        }

        public bool TryGetValue(K key, out V value)
        {
            return fullMap.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return fullMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fullMap.GetEnumerator();
        }
    }
}
