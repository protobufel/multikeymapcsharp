using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GitHub.Protobufel.MultiKeyMap.Base;
using GitHub.Protobufel.MultiKeyMap.PositionMask;

namespace GitHub.Protobufel.MultiKeyMap.PositionMap
{
    [Serializable]
    internal abstract class BasePosMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        private const float CostRatioOfNewJoinOp = 2.0F;

        protected IDictionary<K, V> fullMap;
        [NonSerialized]
        protected IPosSetMultimap<T, K> partMap;

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
        public virtual bool TryGetValuesByPartialKey(IEnumerable<T> partialKey, out IEnumerable<V> values)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out IEnumerable<K> fullKeys))
            {
                values = default(IEnumerable<V>);
                return false;
            }

            var result = new List<V>();

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

            values = result;
            return true;
        }

        public virtual bool TryGetEntriesByPartialKey(IEnumerable<T> partialKey, out IEnumerable<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out IEnumerable<K> fullKeys))
            {
                entries = default(IEnumerable<KeyValuePair<K, V>>);
                return false;
            }

            var result = new List<KeyValuePair<K, V>>();

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

            entries = result;
            return true;
        }

        public virtual bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out IEnumerable<K> fullKeys)
        {
            if (partialKey == null) throw new ArgumentNullException();
            return TryGetFullKeysByPartialKey(partialKey, Enumerable.Empty<int>(), out fullKeys);
        }

        protected static IList<T> AsList(IEnumerable<T> enumerable)
        {
            return (enumerable as IList<T>) ?? new List<T>(enumerable);
        }

        #endregion

        #region positioned filtered queries

        public virtual bool TryGetValuesByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<V> values)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out IEnumerable<K> fullKeys))
            {
                values = default(IEnumerable<V>);
                return false;
            }

            var result = new List<V>();

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

            values = result;
            return true;
        }

        public virtual bool TryGetEntriesByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out IEnumerable<K> fullKeys))
            {
                entries = default(IEnumerable<KeyValuePair<K, V>>);
                return false;
            }

            var result = new List<KeyValuePair<K, V>>();

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

            entries = result;
            return true;
        }

        public virtual bool TryGetFullKeysByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<K> fullKeys)
        {
            if (subKeys == null) throw new ArgumentNullException("subKeys");
            if (positions == null) throw new ArgumentNullException("positions");

            if (!subKeys.Any())
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            IList<T> subKeyList = subKeys as IList<T> ?? subKeys.ToList();
            IList<int> positionList = positions as IList<int> ?? positions.ToList();

            IList<ISet<K>> sets = new List<ISet<K>>();
            IList<IEnumerable<ISet<K>>> colSets = new List<IEnumerable<ISet<K>>>();

            BitArray positionSet = positions.ToBitArray();
            int minSize = int.MaxValue;
            IEnumerable minSubResult = null;

            for (int i = 0; i < subKeyList.Count; i++)
            {
                if ((i < positionList.Count) && (positionList[i] >= 0))
                {
                    if (!partMap.TryGetValue(subKeyList[i], positionList[i], out ISet<K> value, positionSet))
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }

                    if (value.Count < minSize)
                    {
                        minSize = value.Count;
                        minSubResult = value;
                    }

                    sets.Add(value);
                }
                else
                {
                    int count = partMap.TryGetAllValues(subKeyList[i], out IEnumerable<ISet<K>> value, positionSet);

                    if (count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }

                    if (count < minSize)
                    {
                        minSize = count;
                        minSubResult = value;
                    }

                    colSets.Add(value);
                }
            }

            HashSet<K> resultSet = ToSet<K>(minSubResult);

            if ((sets.Count + colSets.Count) == 1)
            {
                fullKeys = resultSet;
                return true;
            }

            //prefer sets to colSets!
            if (sets.Count > 0)
            {
                foreach (var set in sets)
                {
                    if (!ReferenceEquals(set, minSubResult)) // check by reference!
                    {
                        resultSet.IntersectWith(set);

                        if (resultSet.Count == 0)
                        {
                            fullKeys = default(ISet<K>);
                            return false;
                        }
                    }
                }
            }

            if (colSets.Count > 0)
            {
                foreach (var colSet in colSets)
                {
                    if (!ReferenceEquals(colSet, minSubResult)) // check by reference!
                    {
                        if (!SetHelpers.IntersectWith(resultSet, colSet, out var newResultSet)) // check by reference!
                        {
                            fullKeys = default(ISet<K>);
                            return false;
                        }
                        else
                        {
                            resultSet = newResultSet;
                        }
                    }
                }
            }

            if (resultSet.Count == 0)
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            fullKeys = resultSet;
            return true;
        }

        protected virtual HashSet<E> ToSet<E>(IEnumerable source, IEqualityComparer<E> comparer = null)
        {
            switch (source)
            {
                case ISet<E> set:
                    return new HashSet<E>(set, comparer);
                case IEnumerable<ISet<E>> colSet:
                    HashSet<E> result = new HashSet<E>(comparer);

                    foreach (var set in colSet)
                    {
                        result.UnionWith(set);
                    }

                    return result;
                default:
                    throw new InvalidOperationException("unknown source type");
            }
        }
        #endregion

        #region Implementation of the partial map helpers

        protected virtual void AddPartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Add(subKey, i++, key);
            }
        }

        protected virtual void DeletePartial(K key)
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
