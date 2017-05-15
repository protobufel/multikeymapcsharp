using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap
{
    internal class BaseMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        private IDictionary<K, V> fullMap;
        private ILiteSetMultimap<T, K> partMap;

        internal BaseMultiKeyMap(IDictionary<K, V> fullMap, ILiteSetMultimap<T, K> partMap)
        {
            this.fullMap = fullMap;
            this.partMap = partMap;
        }

        #region IMultiKeyMap specific methods
        public IEnumerable<V> GetValuesByPartialKey(IEnumerable<T> partialKey)
        {
            IEnumerable<K> fullKeys = GetFullKeysByPartialKey(partialKey ?? throw new ArgumentNullException());

            if (Enumerable.Empty<K>() == fullKeys)
            {
                return Enumerable.Empty<V>();
            }

            IList<V> result = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    result.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return result;
        }

        public IEnumerable<KeyValuePair<K, V>> GetEntriesByPartialKey(IEnumerable<T> partialKey)
        {
            IEnumerable<K> fullKeys = GetFullKeysByPartialKey(partialKey ?? throw new ArgumentNullException());

            if (Enumerable.Empty<K>() == fullKeys)
            {
                return Enumerable.Empty<KeyValuePair<K, V>>();
            }

            IList<KeyValuePair<K, V>> result = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    result.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return result;
        }

        public IEnumerable<K> GetFullKeysByPartialKey(IEnumerable<T> partialKey)
        {
            partialKey = partialKey ?? throw new ArgumentNullException();
            if (partMap.Count == 0)
            {
                return new HashSet<K>(); // or Enumerable.Empty<K>() of Linq if we returned IEnumerable<K> instead
            }

            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (T subKey in partialKey)
            {
                if (!partMap.TryGetValue(subKey, out ISet<K> subResult))
                {
                    return Enumerable.Empty<K>();
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
                return Enumerable.Empty<K>();
            }

            ISet<K> result = new HashSet<K>(subResults[minPos], partMap.ValueComparer);

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
                        return Enumerable.Empty<K>();
                    }
                }
            }

            return result;
        }
        #endregion

        #region new TryGetsByPartialKey
        public bool TryGetValuesByPartialKey(IEnumerable<T> partialKey, out ICollection<V> values)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                values = default(ICollection<V>);
                return false;
            }

            values = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    values.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public bool TryGetEntriesByPartialKey(IEnumerable<T> partialKey, out ICollection<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                entries = default(ICollection<KeyValuePair<K, V>>);
                return false;
            }

            entries = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    entries.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out ISet<K> fullKeys)
        {
            if (partialKey == null) throw new ArgumentNullException();

            if (partMap.Count == 0)
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (T subKey in partialKey)
            {
                if (!partMap.TryGetValue(subKey, out ISet<K> subResult))
                {
                    fullKeys = default(ISet<K>);
                    return false;
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
                fullKeys = default(ISet<K>);
                return false;
            }

            fullKeys = new HashSet<K>(subResults[minPos], partMap.ValueComparer);

            if (subResults.Count == 1)
            {
                return true;
            }

            for (int i = 0; i < subResults.Count; i++)
            {
                if (i != minPos)
                {
                    fullKeys.IntersectWith(subResults[i]);

                    if (fullKeys.Count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region positioned filtered queries

        public bool TryGetValuesByPartialKey(IEnumerable<(int position, T subkey)> partialKey, out ICollection<V> values)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                values = default(ICollection<V>);
                return false;
            }

            values = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    values.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public bool TryGetEntriesByPartialKey(IEnumerable<(int position, T subkey)> partialKey, out ICollection<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                entries = default(ICollection<KeyValuePair<K, V>>);
                return false;
            }

            entries = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    entries.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public bool TryGetFullKeysByPartialKey(IEnumerable<(int position, T subkey)> partialKey, out ISet<K> fullKeys)
        {
            if (partialKey == null) throw new ArgumentNullException();

            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (var bag in partialKey)
            {

                if (!partMap.TryGetValue(bag.subkey, out ISet<K> subResult))
                {
                    fullKeys = default(ISet<K>);
                    return false;
                }

                if (bag.position > 0)
                {
                    ISet<K> filteredSubResult = new HashSet<K>();

                    foreach (K fullKey in subResult)
                    {
                        if (bag.subkey.Equals(fullKey.ElementAtOrDefault(bag.position)))
                        {
                            filteredSubResult.Add(fullKey);
                        }
                    }

                    if (filteredSubResult.Count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }

                    subResult = filteredSubResult;
                }

                if (subResult.Count < minSize)
                {
                    minSize = subResult.Count;
                    minPos = subResults.Count;
                }

                subResults.Add(subResult);
            }

            if (subResults.Count == 0)
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            fullKeys = new HashSet<K>(subResults[minPos], partMap.ValueComparer);

            if (subResults.Count == 1)
            {
                return true;
            }

            for (int i = 0; i < subResults.Count; i++)
            {
                if (i != minPos)
                {
                    fullKeys.IntersectWith(subResults[i]);

                    if (fullKeys.Count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }
                }
            }

            return true;
        }


        #endregion
        #region single sub-key TryGet queries

        internal bool TryGetValuesByPartialKey(T partialKey, out ICollection<V> values)
        {
            if (TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                values = fullKeys.Select(key => fullMap.TryGetValue(key, out V value) ? value : default(V)).ToList();
                return true;
            }

            values = default(ICollection<V>);
            return false;
        }

        internal bool TryGetEntriesByPartialKey(T partialKey, out ICollection<KeyValuePair<K, V>> entries)
        {
            if (TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                entries = fullKeys.Select(key => fullMap.TryGetValue(key, out V value)
                ? new KeyValuePair<K, V>(key, value) : default(KeyValuePair<K, V>)).ToList();
                return true;
            }

            entries = default(ICollection<KeyValuePair<K, V>>);
            return false;
        }

        internal bool TryGetFullKeysByPartialKey(T partialKey, out ISet<K> fullKeys)
        {
            if (partialKey == null) throw new ArgumentNullException();

            if ((partMap.Count == 0) || !partMap.TryGetValue(partialKey, out fullKeys))
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            return true;
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

        public V this[K key]
        {
            get => fullMap[key];
            set
            {
                int count = fullMap.Count;
                fullMap[key] = value;

                if (fullMap.Count > count)
                {
                    AddPartial(key);
                }
            }
        }

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
