using System;
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
    }
}
