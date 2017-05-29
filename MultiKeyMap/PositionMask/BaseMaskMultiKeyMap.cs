using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class BaseMaskMultiKeyMap<T, K, V> : BaseMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V> where K : IEnumerable<T>
    {
        protected const float CostRatioOfNewJoinOp = 2.0F;

        protected BaseMaskMultiKeyMap(IEqualityComparer<ISubKeyMask<T>> subKeyComparer, IEqualityComparer<IKeyMask<T, K>> fullKeyComparer,
            IDictionary<IKeyMask<T, K>, V> fullMap = null, ILiteSetMultimap<ISubKeyMask<T>, IKeyMask<T, K>> partMap = null)
            : base(subKeyComparer, fullKeyComparer, fullMap, partMap)
        {
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

        #region Implementation of the partial map helpers

        protected override void AddPartial(IKeyMask<T, K> key)
        {
            foreach (var subKeyMask in key)
            {
                partMap.Add(subKeyMask, key);
                RegisterSubKeyPosition(subKeyMask);
            }
        }

        protected override void DeletePartial(IKeyMask<T, K> key)
        {
            foreach (var subKeyMask in key)
            {
                if (partMap.Remove(subKeyMask, key, out bool removedEntireKey) && removedEntireKey)
                {
                    RemoveSubKeyPosition(subKeyMask, out bool removedEntireSubKey);
                }
            }
        }

        protected override void ClearPartial()
        {
            base.ClearPartial();
            ClearSubKeyPositions();
        }

        #endregion

        #region Filtered Queries Overrides

        /// <summary>
        /// Defers to the base class implementation without positions, wherein positions are already embedded in the <paramref name="subKeys"/>.
        /// </summary>
        /// <param name="subKeys">The sequence of <see cref="ISubKeyMask{T}"/> each already having (subKey, position) combination 
        /// sufficient for the positioned search</param>
        /// <param name="positions">not needed here, set to it to <c>null</c></param>
        /// <param name="fullKeys">A non-live non-empty set of the full keys satisfying the partial key criteria, or the default value of
        /// the result type if not found.</param>
        /// <returns>true if the partial key is found, false otherwise.</returns>
        public override bool TryGetFullKeysByPartialKey(IEnumerable<ISubKeyMask<T>> subKeys, IEnumerable<int> positions, out IEnumerable<IKeyMask<T, K>> fullKeys)
        {
            return base.TryGetFullKeysByPartialKey(subKeys, out fullKeys);
        }

        public override bool TryGetFullKeysByPartialKey(IEnumerable<ISubKeyMask<T>> subKeys, out IEnumerable<IKeyMask<T, K>> fullKeys)
        {
            if (subKeys == null) throw new ArgumentNullException("subKeys");

            if (!subKeys.Any())
            {
                fullKeys = default(ISet<IKeyMask<T, K>>);
                return false;
            }

            IList<ISet<IKeyMask<T, K>>> sets = new List<ISet<IKeyMask<T, K>>>();
            IList<IEnumerable<ISet<IKeyMask<T, K>>>> colSets = new List<IEnumerable<ISet<IKeyMask<T, K>>>>();

            //BitArray positionSet = positions.ToBitArray();
            int minSize = int.MaxValue;
            IEnumerable minSubResult = null;

            foreach (var subKeyMask in subKeys)
            {
                if (subKeyMask.Position >= 0)
                {
                    if (!partMap.TryGetValue(subKeyMask, out ISet<IKeyMask<T, K>> value))
                    {
                        fullKeys = default(IEnumerable<IKeyMask<T, K>>);
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
                    if (!TryGetPositions(subKeyMask.SubKey, out IBitList positionMask))
                    {
                        fullKeys = default(IEnumerable<IKeyMask<T, K>>);
                        return false;
                    }
                    else
                    {
                        IList<ISet<IKeyMask<T, K>>> colSet = new List<ISet<IKeyMask<T, K>>>();

                        int i = -1;
                        var subKey = subKeyMask.SubKey;
                        int count = 0;

                        foreach (bool posExists in positionMask)
                        {
                            i++;

                            if (posExists)
                            {
                                if (!partMap.TryGetValue(new SubKeyMask<T>(subKey, i), out ISet<IKeyMask<T, K>> value))
                                {
                                    // we shouldn't be getting here in the normal circumstances!
                                    fullKeys = default(IEnumerable<IKeyMask<T, K>>);
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

            HashSet<IKeyMask<T, K>> resultSet = ToSet<IKeyMask<T, K>>(minSubResult);

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
                            fullKeys = default(ISet<IKeyMask<T, K>>);
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
                        fullKeys = default(IEnumerable<IKeyMask<T, K>>);
                        return false;
                    }
                }
            }

            if (resultSet.Count == 0)
            {
                fullKeys = default(IEnumerable<IKeyMask<T, K>>);
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
    }
}
