using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.PositionMask;

namespace GitHub.Protobufel.MultiKeyMap.Base
{
    [Serializable]
    internal abstract class PositionalBaseMaskMultiKeyMap<T, K, V> : BaseMultiKeyMap<T, K, V>, IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        [NonSerialized]
        protected ILiteSetMultimap<ISubKeyMask<T>, K> partMap;

        protected PositionalBaseMaskMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null,
            IDictionary<K, V> fullMap = null, ILiteSetMultimap<ISubKeyMask<T>, K> partMap = null)
            : base(subKeyComparer, fullKeyComparer, fullMap)
        {
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

        protected virtual ILiteSetMultimap<ISubKeyMask<TSubKey>, TKey> CreateLiteSetMultimap<TSubKey, TKey>(
            IEqualityComparer<TSubKey> subKeyComparer, IEqualityComparer<TKey> fullKeyComparer)
            where TKey : IEnumerable<TSubKey>
        {
            return CreateSupportDictionary<ISubKeyMask<TSubKey>, ISet<TKey>>(subKeyComparer.ToSubKeyMaskComparer())
                .ToSetMultimap(fullKeyComparer);
        }

        protected virtual IEqualityComparer<ISubKeyMask<T>> SubKeyMaskComparer => subKeyComparer.ToSubKeyMaskComparer();

        public override bool TryGetFullKeysByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<K> fullKeys)
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
                    if (!ReferenceEquals(colSet, minSubResult)) // check by reference!
                    {
                        if (!SetHelpers.IntersectWith(resultSet, colSet, out var newResultSet))
                        {
                            fullKeys = default(IEnumerable<K>);
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
                fullKeys = default(IEnumerable<K>);
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

        #region Implementation of the partial map helpers

        protected override void AddPartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                ISubKeyMask<T> subKeyMask = subKey.ToSubKeyMask(i++);
                partMap.Add(subKeyMask, key);
                RegisterSubKeyPosition(subKeyMask);
            }
        }

        protected override void DeletePartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                ISubKeyMask<T> subKeyMask = subKey.ToSubKeyMask(i++);

                if (partMap.Remove(subKeyMask, key, out bool removedEntireKey) && removedEntireKey)
                {
                    RemoveSubKeyPosition(subKeyMask, out bool removedEntireSubKey);
                }
            }
        }

        protected override void ClearPartial()
        {
            partMap.Clear();
            ClearSubKeyPositions();
        }

        #endregion

        protected override void OnDeserializedHelper(StreamingContext context)
        {
            base.OnDeserializedHelper(context);

            partMap = CreateLiteSetMultimap(subKeyComparer, fullKeyComparer);

            foreach (var entry in fullMap)
            {
                AddPartial(entry.Key);
            }
        }
    }
}
