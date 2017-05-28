using System;
using System.Linq;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    class PositionOptimizedMultiKeyMap<T, K, V> : VirtualMultiKeyMap<T, ISubKeyMask<T>, K, IKeyMask<T, K>, V>
        where K : IEnumerable<T>
    {
        public PositionOptimizedMultiKeyMap(IMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V> source)
            : base(source, GetSubKeySelector1<T>(), GetSubKeySelector2<T>(), GetKeySelector1<T, K>(), GetKeySelector2<T, K>())
        {
        }

        static Func<ISubKeyMask<TSubKey>, TSubKey> GetSubKeySelector1<TSubKey>()
        {
            return x => x.SubKey;
        }

        static Func<TSubKey, ISubKeyMask<TSubKey>> GetSubKeySelector2<TSubKey>()
        {
            return x => x.ToSubKeyMask();
        }


        static Func<IKeyMask<TSubKey, TKey>, TKey> GetKeySelector1<TSubKey, TKey>()
        where TKey : IEnumerable<TSubKey>
        {
            return x => x.Key;
        }

        static Func<TKey, IKeyMask<TSubKey, TKey>> GetKeySelector2<TSubKey, TKey>()
        where TKey : IEnumerable<TSubKey>
        {
            return x => x.ToKeyMask<TSubKey, TKey>();
        }

        public override bool TryGetFullKeysByPartialKey(IList<T> partialKey, IList<int> positions, out ISet<K> fullKeys)
        {
            var subKeyMasks = partialKey.Zip(positions.Concat(Enumerable.Repeat(-1, int.MaxValue)),
                (subKey, pos) => subKey.ToSubKeyMask(pos));

            if (Source.TryGetFullKeysByPartialKey(subKeyMasks, out var fullKeys2))
            {
                fullKeys = new HashSet<K>(fullKeys2.Select(k => k.To(keySelector1)));
                return true;
            }

            fullKeys = default(ISet<K>);
            return false;
        }
    }
}
