using System;
using System.Collections;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class MaskMultiKeyMap<T, K, V> : BaseMaskMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        [NonSerialized]
        protected readonly IDictionary<T, BitArray> subKeyPositions;

        public MaskMultiKeyMap(IEqualityComparer<ISubKeyMask<T>> subKeyComparer, IEqualityComparer<IKeyMask<T, K>> fullKeyComparer,
            IDictionary<IKeyMask<T, K>, V> fullMap, ILiteSetMultimap<ISubKeyMask<T>, IKeyMask<T, K>> partMap, IDictionary<T, BitArray> subKeyPositions = null)
            : base(subKeyComparer, fullKeyComparer, fullMap, partMap)
        {
            this.subKeyPositions = subKeyPositions ?? new Dictionary<T, BitArray>();
        }

        protected override bool AddSubKeyPosition(ISubKeyMask<T> subKeyMask)
        {
            if (subKeyPositions.TryGetValue(subKeyMask.SubKey, out var positionMask))
            {
                positionMask.SetAndResize(subKeyMask.Position, true);
                return false;
            }
            else
            {
                positionMask = new BitArray(subKeyMask.Position);
                positionMask.Set(subKeyMask.Position, true);
                subKeyPositions.Add(subKeyMask.SubKey, positionMask);
                return true;
            }
        }

        protected override void ClearSubKeyPositions()
        {
            subKeyPositions.Clear();
        }

        protected override bool IsSubKeyPosition(ISubKeyMask<T> subKeyMask)
        {
            return subKeyPositions.TryGetValue(subKeyMask.SubKey, out var positionMask) && positionMask.TryGet(subKeyMask.Position);
        }

        protected override bool RemoveSubKeyPosition(ISubKeyMask<T> subKeyMask, out bool removedEntireSubKey)
        {
            removedEntireSubKey = false;

            if (subKeyPositions.TryGetValue(subKeyMask.SubKey, out var positionMask))
            {
                positionMask.SetAndResize(subKeyMask.Position, false);

                if (positionMask.IsFalse())
                {
                    subKeyPositions.Remove(subKeyMask.SubKey);
                    removedEntireSubKey = true;
                }

                return true;
            }

            return false;
        }

        protected override bool TryGetPositions(T subKey, out BitArray positionMask)
        {
            if (subKeyPositions.TryGetValue(subKey, out positionMask))
            {
                positionMask = new BitArray(positionMask);
                return true;
            }

            positionMask = default(BitArray);
            return false;
        }
    }
}
