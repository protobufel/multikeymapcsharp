using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap.PositionMask.Simple
{
    [Serializable]
    internal abstract class BaseMaskMultiKeyMap<T, K, V> : BaseMultiKeyMap<T, K, V>, IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        [NonSerialized]
        internal protected ILiteSetMultimap<ISubKeyMask<T>, K> partMap;

        protected BaseMaskMultiKeyMap() { }

        protected BaseMaskMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null,
            IDictionary<K, V> fullMap = null, ILiteSetMultimap<ISubKeyMask<T>, K> partMap = null)
        {
            this.subKeyComparer = subKeyComparer ?? EqualityComparer<T>.Default;
            this.fullKeyComparer = fullKeyComparer ?? EqualityComparer<K>.Default;
            this.fullMap = fullMap ?? CreateDictionary<K, V>(fullKeyComparer);
            this.partMap = partMap ?? CreateLiteSetMultimap(subKeyComparer, fullKeyComparer);
        }

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
        #region Implementation of the partial map helpers

        protected override void AddPartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Add(subKey.ToSubKeyMask(i++), key);
            }
        }

        protected override void DeletePartial(K key)
        {
            int i = 0;

            foreach (T subKey in key)
            {
                partMap.Remove(subKey.ToSubKeyMask(i++), key);
            }
        }


        protected override void ClearPartial()
        {
            partMap.Clear();
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
