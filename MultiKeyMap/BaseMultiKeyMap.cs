using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class BaseMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        internal protected IDictionary<K, V> fullMap;
        [NonSerialized]
        internal protected ILiteSetMultimap<T, K> partMap;

        protected internal IEqualityComparer<K> fullKeyComparer;
        protected internal IEqualityComparer<T> subKeyComparer;

        internal BaseMultiKeyMap(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer,
            IDictionary<K, V> fullMap, ILiteSetMultimap<T, K> partMap)
        {
            this.fullMap = fullMap ?? throw new ArgumentNullException("fullMap");
            this.partMap = partMap ?? throw new ArgumentNullException("partMap");
            this.subKeyComparer = subKeyComparer ?? throw new ArgumentNullException("subKeyComparer");
            this.fullKeyComparer = fullKeyComparer ?? throw new ArgumentNullException("fullKeyComparer");
        }

        protected internal virtual IEqualityComparer<K> FullKeyComparer => fullKeyComparer;
        protected internal virtual IEqualityComparer<T> SubKeyComparer => subKeyComparer;

        #region non-positional TryGetsByPartialKey

        public virtual bool TryGetValuesByPartialKey(IEnumerable<T> partialKey, out ICollection<V> values)
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

        public virtual bool TryGetEntriesByPartialKey(IEnumerable<T> partialKey, out ICollection<KeyValuePair<K, V>> entries)
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

        public virtual bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out ISet<K> fullKeys)
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

            fullKeys = new HashSet<K>(subResults[minPos], FullKeyComparer);

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

        public virtual bool TryGetValuesByPartialKey(IList<T> subKeys, IList<int> positions, out ICollection<V> values)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out ISet<K> fullKeys))
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

        public virtual bool TryGetEntriesByPartialKey(IList<T> subKeys, IList<int> positions, out ICollection<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out ISet<K> fullKeys))
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

        public virtual bool TryGetFullKeysByPartialKey(IList<T> subKeys, IList<int> positions, out ISet<K> fullKeys)
        {
            if (subKeys == null) throw new ArgumentNullException("subKeys");
            if (positions == null) throw new ArgumentNullException("positions");

            if (subKeys.Count == 0)
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (var subKey in subKeys)
            {
                if (!partMap.TryGetValue(subKey, out ISet<K> subResult))
                {
                    fullKeys = default(ISet<K>);
                    return false;
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

            if (GetAtOrNegative(positions, minPos) < 0)
            {
                fullKeys = new HashSet<K>(subResults[minPos], FullKeyComparer);

            }
            else if (!TryGetFilteredFullKeys(GetAtOrNegative(positions, minPos), subKeys[minPos], subResults[minPos], FullKeyComparer, out fullKeys))
            {
                return false;
            }

            if (subResults.Count == 1)
            {
                return true;
            }

            for (int i = 0; i < subResults.Count; i++)
            {
                if (i != minPos)
                {
                    if (!TryGetFilteredFullKeys(GetAtOrNegative(positions, i), subKeys[i], subResults[i], FullKeyComparer, out ISet<K> filteredSubResult))
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }

                    fullKeys.IntersectWith(filteredSubResult);

                    if (fullKeys.Count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }
                }
            }

            return true;
        }

        private int GetAtOrNegative(IList<int> positions, int pos)
        {
            return pos < positions.Count ? positions[pos] : -1;
        }


        internal protected virtual bool TryGetFilteredFullKeys(int position, T subkey, ISet<K> source, IEqualityComparer<K> comparer, out ISet<K> target)
        {
            if (source.Count == 0)
            {
                target = default(ISet<K>);
                return false;
            }

            if (position < 0)
            {
                target = source;
                return true;
            }

            target = new HashSet<K>(comparer);

            foreach (K fullKey in source)
            {
                if (subkey.Equals(fullKey.ElementAtOrDefault(position)))
                {
                    target.Add(fullKey);
                }
            }

            if (target.Count == 0)
            {
                target = default(ISet<K>);
                return false;
            }

            return true;
        }

        #endregion
        #region single sub-key TryGet queries

        internal protected virtual bool TryGetValuesByPartialKey(T partialKey, out ICollection<V> values)
        {
            if (TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                values = fullKeys.Select(key => fullMap.TryGetValue(key, out V value) ? value : default(V)).ToList();
                return true;
            }

            values = default(ICollection<V>);
            return false;
        }

        internal protected virtual bool TryGetEntriesByPartialKey(T partialKey, out ICollection<KeyValuePair<K, V>> entries)
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

        internal protected virtual bool TryGetFullKeysByPartialKey(T partialKey, out ISet<K> fullKeys)
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

        internal protected virtual void AddPartial(K key)
        {
            foreach (T subKey in key)
            {
                partMap.Add(subKey, key);
            }
        }

        internal protected virtual void DeletePartial(K key)
        {
            foreach (T subKey in key)
            {
                partMap.Remove(subKey, key);
            }
        }


        internal protected virtual void ClearPartial()
        {
                partMap.Clear();
        }

        #endregion

        public virtual V this[K key]
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

        public virtual ICollection<K> Keys => fullMap.Keys;

        public virtual ICollection<V> Values => fullMap.Values;

        public virtual int Count => fullMap.Count;

        public virtual bool IsReadOnly => fullMap.IsReadOnly;

        public virtual void Add(K key, V value, IEnumerable<bool> positions)
        {
            if (positions == null) throw new ArgumentNullException("positions");
            Add(key, value);
        }

        public virtual void Add(K key, V value)
        {
            fullMap.Add(key, value);
            AddPartial(key);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            fullMap.Add(item);
            AddPartial(item.Key);
        }

        public virtual void Clear()
        {
            fullMap.Clear();
            ClearPartial();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            return fullMap.Contains(item);
        }

        public virtual bool ContainsKey(K key)
        {
            return fullMap.ContainsKey(key);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            fullMap.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(K key)
        {
            if (fullMap.Remove(key))
            {
                DeletePartial(key);
                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            if (fullMap.Remove(item))
            {
                DeletePartial(item.Key);
                return true;
            }

            return false;
        }

        public virtual bool TryGetValue(K key, out V value)
        {
            return fullMap.TryGetValue(key, out value);
        }

        public virtual IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return fullMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fullMap.GetEnumerator();
        }
    }
}
