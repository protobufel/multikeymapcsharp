using System;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class BaseMaskMultiKeyMap<T, K, V> : BaseMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V> where K : IEnumerable<T>
    {
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

        internal protected override void AddPartial(IKeyMask<T, K> key)
        {
            foreach (var subKeyMask in key)
            {
                partMap.Add(subKeyMask, key);
                RegisterSubKeyPosition(subKeyMask);
            }
        }

        internal protected override void DeletePartial(IKeyMask<T, K> key)
        {
            foreach (var subKeyMask in key)
            {
                if (partMap.Remove(subKeyMask, key, out bool removedEntireKey) && removedEntireKey)
                {
                    RemoveSubKeyPosition(subKeyMask, out bool removedEntireSubKey);
                }
            }
        }

        protected internal override void ClearPartial()
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
        public override bool TryGetFullKeysByPartialKey(IList<ISubKeyMask<T>> subKeys, IList<int> positions, out ISet<IKeyMask<T, K>> fullKeys)
        {
            return base.TryGetFullKeysByPartialKey(subKeys, out fullKeys);
        }

        #endregion
    }
}
