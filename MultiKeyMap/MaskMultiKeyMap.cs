using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using static GitHub.Protobufel.MultiKeyMap.KeyMaskExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class MaskMultiKeyMap<T, K, V> : BaseMaskMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        [NonSerialized]
        protected IDictionary<T, IBitList> subKeyPositions;

        protected MaskMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null,
            IDictionary<IKeyMask<T, K>, V> fullMap = null, ILiteSetMultimap<ISubKeyMask<T>, IKeyMask<T, K>> partMap = null)
            : base((subKeyComparer ?? EqualityComparer<T>.Default).ToSubKeyMaskComparer(),
                  (fullKeyComparer ?? EqualityComparer<K>.Default).ToKeyMaskComparer<T, K>(), 
                  fullMap, partMap)
        {
            subKeyPositions = CreateSupportDictionary<T, IBitList>(OriginalSubKeyComparer);
        }

        protected override bool AddSubKeyPosition(ISubKeyMask<T> subKeyMask)
        {
            if (subKeyPositions.TryGetValue(subKeyMask.SubKey, out IBitList positionMask))
            {
                positionMask.Set(subKeyMask.Position, true);
                return false;
            }
            else
            {
                positionMask = new BitList(subKeyMask.Position);
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
                positionMask.Set(subKeyMask.Position, false);

                if (positionMask.CountTrue == 0)
                {
                    subKeyPositions.Remove(subKeyMask.SubKey);
                    removedEntireSubKey = true;
                }

                return true;
            }

            return false;
        }

        protected override bool TryGetPositions(T subKey, out IBitList positionMask)
        {
            if (subKeyPositions.TryGetValue(subKey, out positionMask))
            {
                positionMask = new BitList(positionMask);
                return true;
            }

            positionMask = default(IBitList);
            return false;
        }

        protected virtual IEqualityComparer<T> OriginalSubKeyComparer => (subKeyComparer as SubKeyMaskComparer<T>).SubKeyComparer;
        protected virtual IEqualityComparer<K> OriginalFullKeyComparer => (fullKeyComparer as KeyMaskComparer<T, K>).KeyComparer;

        protected override void OnDeserializedHelper(StreamingContext context)
        {
            subKeyPositions = CreateSupportDictionary<T, IBitList>(OriginalSubKeyComparer);
            base.OnDeserializedHelper(context);
        }
    }
}
