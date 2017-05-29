using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap.PositionMask.Simple
{
    [Serializable]
    internal abstract class BaseMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        protected const float CostRatioOfNewJoinOp = 2.0F;

        internal protected IDictionary<K, V> fullMap;
        [NonSerialized]
        internal protected ILiteSetMultimap<ISubKeyMask<T>, K> partMap;

        protected internal IEqualityComparer<K> fullKeyComparer;
        protected internal IEqualityComparer<T> subKeyComparer;

        protected BaseMultiKeyMap() { }

        protected BaseMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null,
            IDictionary<K, V> fullMap = null, ILiteSetMultimap<ISubKeyMask<T>, K> partMap = null)
        {
            this.subKeyComparer = subKeyComparer ?? EqualityComparer<T>.Default;
            this.fullKeyComparer = fullKeyComparer ?? EqualityComparer<K>.Default;
            this.fullMap = fullMap ?? CreateDictionary<K, V>(fullKeyComparer);
            this.partMap = partMap ?? CreateLiteSetMultimap(subKeyComparer, fullKeyComparer);
        }

        #region SubKey Positions abstract hooks

        protected abstract bool AddSubKeyPosition(ISubKeyMask<T> subKeyMask);
        protected abstract bool RemoveSubKeyPosition(ISubKeyMask<T> subKeyMask, out bool removedEntireSubKey);
        protected abstract bool IsSubKeyPosition(ISubKeyMask<T> subKeyMask);
        protected abstract bool TryGetPositions(T subKey, out IBitList positionMask);
        protected abstract void ClearSubKeyPositions();


        protected virtual bool RegisterSubKeyPosition(ISubKeyMask<T> subKeyMask)
        {
            if (IsSubKeyPosition(subKeyMask)) return false;

            AddSubKeyPosition(subKeyMask);
            return true;
        }

        #endregion


        protected abstract IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer);

        protected virtual IDictionary<TKey, TValue> CreateSupportDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            return CreateDictionary<TKey, TValue>(comparer);
        }

        protected virtual ILiteSetMultimap<ISubKeyMask<TSubKey>, TKey> CreateLiteSetMultimap<TSubKey, TKey>(
            IEqualityComparer<TSubKey> subKeyComparer, IEqualityComparer<TKey> fullKeyComparer)
            where TKey : IEnumerable<TSubKey>
        {
            return CreateSupportDictionary<ISubKeyMask<TSubKey>, ISet<TKey>>(subKeyComparer.ToSubKeyMaskComparer())
                .ToSetMultimap(fullKeyComparer);
        }

        protected virtual IEqualityComparer<K> FullKeyComparer => fullKeyComparer;
        protected virtual IEqualityComparer<T> SubKeyComparer => subKeyComparer;
        protected virtual IEqualityComparer<ISubKeyMask<T>> SubKeyMaskComparer => subKeyComparer.ToSubKeyMaskComparer();

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
            return TryGetFullKeysByPartialKey(partialKey, Enumerable.Empty<int>(), out fullKeys);
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

            if (!subKeys.Any())
            {
                fullKeys = default(IEnumerable<K>);
                return false;
            }

            IList<ISet<K>> sets = new List<ISet<K>>();
            IList<IEnumerable<ISet<K>>> colSets = new List<IEnumerable<ISet<K>>>();

            IList<int> positionList = positions as IList<int> ?? positions.ToList();
            IList<T> subKeyList = subKeys as IList<T> ?? subKeys.ToList();
            //BitArray positionSet = positions.ToBitArray();
            int minSize = int.MaxValue;
            IEnumerable minSubResult = null;
            int i = 0;

            foreach (var subKey in subKeyList)
            {
                int position = GetAtOrNegative(positionList, i++);

                if (position >= 0)
                {
                    if (!partMap.TryGetValue(subKey.ToSubKeyMask(position), out ISet<K> value))
                    {
                        fullKeys = default(IEnumerable<K>);
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
                    if (!TryGetPositions(subKey, out IBitList positionMask))
                    {
                        fullKeys = default(IEnumerable<K>);
                        return false;
                    }
                    else
                    {
                        IList<ISet<K>> colSet = new List<ISet<K>>();

                        int j = -1;
                        int count = 0;

                        foreach (bool posExists in positionMask)
                        {
                            j++;

                            if (posExists)
                            {
                                if (!partMap.TryGetValue(subKey.ToSubKeyMask(j), out ISet<K> value))
                                {
                                    // we shouldn't be getting here in the normal circumstances!
                                    fullKeys = default(IEnumerable<K>);
                                    return false;
                                }

                                count += value.Count;
                                colSet.Add(value);
                            }
                        }

                        if (count < minSize)
                        {
                            minSize = count;
                            minSubResult = colSet;
                        }

                        colSets.Add(colSet);
                    }
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
                            fullKeys = default(IEnumerable<K>);
                            return false;
                        }
                    }
                }
            }

            if (colSets.Count > 0)
            {
                foreach (var colSet in colSets)
                {
                    if (!ReferenceEquals(colSet, minSubResult) && !IntersectWith(resultSet, colSet)) // check by reference!
                    {
                        fullKeys = default(IEnumerable<K>);
                        return false;
                    }
                }
            }

            if (resultSet.Count == 0)
            {
                fullKeys = default(IEnumerable<K>);
                return false;
            }

            fullKeys = resultSet;
            return true;
        }

        private int GetAtOrNegative(IList<int> positionList, int pos)
        {
            return pos < positionList.Count ? positionList[pos] : -1;
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

        protected virtual bool IntersectWith<E>(HashSet<E> set, IEnumerable<ISet<E>> colSet)
        {
            if ((set.Count == 0) || !colSet.Any())
            {
                return false;
            }

            if (IsJoinMoreExpensive(set, colSet, CostRatioOfNewJoinOp))
            {
                //set.RemoveWhere(e => !colSet.Any(s => s.Contains(e)));
                set.RemoveWhere(e =>
                {
                    foreach (var subSet in colSet)
                    {
                        if (subSet.Contains(e))
                        {
                            return false;
                        }
                    }

                    return true;
                });
            }
            else
            {
                if (!TryJoinSets(colSet, out ISet<E> joinedSet))
                {
                    set.Clear();
                    return false;
                }
            }

            return (set.Count > 0);
        }

        private bool IsJoinMoreExpensive<E>(HashSet<E> first, IEnumerable<ISet<E>> second, float ratio)
        {
            long secondSize = 0;
            int secondCount = 0;

            foreach (var col in second)
            {
                secondSize += col.Count;
                secondCount++;
            }

            return (CostRatioOfNewJoinOp * secondSize) > ((first.Count - 1) * secondCount);
        }

        private bool TryJoinSets<E>(IEnumerable<ISet<E>> sets, out ISet<E> result)
        {
            bool first = true;
            result = null;

            foreach (var set in sets)
            {
                if (first)
                {
                    first = false;
                    result = new HashSet<E>(set);
                }
                else
                {
                    result.UnionWith(set);
                }
            }

            return (result != null) && (result.Count > 0);
        }


        #endregion

        #region Implementation of the partial map helpers

        internal protected virtual void AddPartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Add(subKey.ToSubKeyMask(i++), key);
            }
        }

        internal protected virtual void DeletePartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Remove(subKey.ToSubKeyMask(i++), key);
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

        protected virtual void OnDeserializedHelper(StreamingContext context)
        {
            (fullMap as IDeserializationCallback).OnDeserialization(null);

            partMap = CreateLiteSetMultimap(subKeyComparer, fullKeyComparer);

            foreach (var entry in fullMap)
            {
                AddPartial(entry.Key);
            }
        }
    }
}
