using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class BaseMaskMultiKeyMap<T, K, V> : BaseMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V> where K : IEnumerable<T>
    {
        public BaseMaskMultiKeyMap(IEqualityComparer<ISubKeyMask<T>> subKeyComparer, IEqualityComparer<IKeyMask<T, K>> fullKeyComparer,
            IDictionary<IKeyMask<T, K>, V> fullMap, ILiteSetMultimap<ISubKeyMask<T>, IKeyMask<T, K>> partMap)
            : base(subKeyComparer, fullKeyComparer, fullMap, partMap)
        {
        }

        #region SubKey Positions abstract hooks

        protected abstract bool AddSubKeyPosition(ISubKeyMask<T> subKeyMask);
        protected abstract bool RemoveSubKeyPosition(ISubKeyMask<T> subKeyMask, out bool removedEntireSubKey);
        protected abstract bool IsSubKeyPosition(ISubKeyMask<T> subKeyMask);
        protected abstract bool TryGetPositions(T subKey, out BitArray positionMask);
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
    }
}
