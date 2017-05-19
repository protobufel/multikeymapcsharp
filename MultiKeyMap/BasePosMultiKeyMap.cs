using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class BasePosMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        internal protected IDictionary<K, V> fullMap;
        [NonSerialized]
        internal protected IPosSetMultimap<T, K> partMap;

        protected internal IEqualityComparer<K> fullKeyComparer;
        protected internal IEqualityComparer<T> subKeyComparer;

        internal BasePosMultiKeyMap(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer,
            IDictionary<K, V> fullMap, IPosSetMultimap<T, K> partMap)
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
            return TryGetFullKeysByPartialKey(AsList(partialKey), Enumerable.Empty<int>() as IList<int>, out fullKeys);
        }

        protected static IList<T> AsList(IEnumerable<T> enumerable)
        {
            return enumerable is IList<T> ? enumerable as IList<T> : new List<T>(enumerable);
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
            BitArray positionSet = positions.ToBitArray(); // which one to remove/use?

            for (int i = 0; i < subKeys.Count; i++)
            {
                if (!partMap.TryGetValue(subKeys[i], GetAtOrNegative(positions, i), out ISet<K> subResult, positionSet))
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

        private int GetAtOrNegative(IList<int> positions, int pos)
        {
            return (pos < positions.Count) ? positions[pos] : -1;
        }
        #endregion

        #region Implementation of the partial map helpers

        internal protected virtual void AddPartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Add(subKey, i++, key);
            }
        }

        internal protected virtual void DeletePartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Remove(subKey, i++, key);
            }
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

        public virtual void Add(K key, V value)
        {
            fullMap.Add(key, value);
            AddPartial(key);
        }

        public virtual void Add(KeyValuePair<K, V> item)
        {
            fullMap.Add(item);
            AddPartial(item.Key);
        }

        public virtual void Clear()
        {
            fullMap.Clear();
        }

        public virtual bool Contains(KeyValuePair<K, V> item)
        {
            return fullMap.Contains(item);
        }

        public virtual bool ContainsKey(K key)
        {
            return fullMap.ContainsKey(key);
        }

        public virtual void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
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

        public virtual bool Remove(KeyValuePair<K, V> item)
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
