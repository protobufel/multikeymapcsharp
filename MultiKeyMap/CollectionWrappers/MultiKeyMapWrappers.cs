using System;
using System.Linq;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    public static partial class EnumerableExtensions
    {
        public static IMultiKeyMap<T1, K1, V1> ToMultiKeyMapWrapper<T1, T2, K1, V1, K2, V2>(
            this IMultiKeyMap<T2, K2, V2> source,
            Func<T2, T1> subKeySelector1, Func<T1, T2> subKeySelector2,
            Func<K2, K1> keySelector1, Func<K1, K2> keySelector2,
            Func<V2, V1> valueSelector1, Func<V1, V2> valueSelector2)
        where K1 : IEnumerable<T1>
        where K2 : IEnumerable<T2>
        {
            return new VirtualMultiKeyMap<T1, T2, K1, V1, K2, V2>(source, subKeySelector1,
                subKeySelector2, keySelector1, keySelector2, valueSelector1, valueSelector2);
        }

        public static IMultiKeyMap<T1, K1, V> ToMultiKeyMapWrapper<T1, T2, K1, K2, V>(
            this IMultiKeyMap<T2, K2, V> source,
            Func<T2, T1> subKeySelector1, Func<T1, T2> subKeySelector2,
            Func<K2, K1> keySelector1, Func<K1, K2> keySelector2)
        where K1 : IEnumerable<T1>
        where K2 : IEnumerable<T2>
        {
            return new VirtualMultiKeyMap<T1, T2, K1, K2, V>(source, subKeySelector1, subKeySelector2,
                keySelector1, keySelector2);
        }
    }

    [Serializable]
    internal class VirtualMultiKeyMap<T1, T2, K1, K2, V> : VirtualMultiKeyMap<T1, T2, K1, V, K2, V>
        where K1 : IEnumerable<T1>
        where K2 : IEnumerable<T2>
    {
        static protected readonly Func<V, V> ValueIdentity = x => x;

        public VirtualMultiKeyMap(IMultiKeyMap<T2, K2, V> source,
            Func<T2, T1> subKeySelector1,
            Func<T1, T2> subKeySelector2,
            Func<K2, K1> keySelector1, Func<K1, K2> keySelector2) : base(source, subKeySelector1, subKeySelector2,
                keySelector1, keySelector2, ValueIdentity, ValueIdentity)
        {
        }
    }

    [Serializable]
    internal class VirtualMultiKeyMap<T1, T2, K1, V1, K2, V2> : VirtualDictionary<K1, V1, K2, V2>, IMultiKeyMap<T1, K1, V1>
        where K1 : IEnumerable<T1>
        where K2 : IEnumerable<T2>
    {
        [NonSerialized]
        protected Func<T2, T1> subKeySelector1;
        [NonSerialized]
        protected Func<T1, T2> subKeySelector2;

        public VirtualMultiKeyMap(IMultiKeyMap<T2, K2, V2> source,
            Func<T2, T1> subKeySelector1,
            Func<T1, T2> subKeySelector2,
            Func<K2, K1> keySelector1, Func<K1, K2> keySelector2,
            Func<V2, V1> valueSelector1, Func<V1, V2> valueSelector2
            ) : base(source, keySelector1, keySelector2, valueSelector1, valueSelector2)
        {
            this.subKeySelector1 = subKeySelector1 ?? throw new ArgumentNullException("subKeySelector1");
            this.subKeySelector2 = subKeySelector2 ?? throw new ArgumentNullException("subKeySelector2");
        }

        protected virtual IMultiKeyMap<T2, K2, V2> Source => source as IMultiKeyMap<T2, K2, V2>;

        public void Add(K1 key, V1 value, IEnumerable<bool> positions)
        {
            Source.Add(key.To(keySelector2), value.To(valueSelector2), positions);
        }

        public bool TryGetEntriesByPartialKey(IEnumerable<T1> partialKey, out ICollection<KeyValuePair<K1, V1>> entries)
        {
            if (Source.TryGetEntriesByPartialKey(partialKey.Select(x => x.To(subKeySelector2)), out var entries2))
            {
                entries = entries2.Select(kv => kv.To(keySelector1, valueSelector1)).ToReadOnlyCollection();
                return true;
            }

            entries = default(ICollection<KeyValuePair<K1, V1>>);
            return false;
        }

        public bool TryGetEntriesByPartialKey(IList<T1> partialKey, IList<int> positions, out ICollection<KeyValuePair<K1, V1>> entries)
        {
            if (Source.TryGetEntriesByPartialKey(partialKey.Select(x => x.To(subKeySelector2)).ToListWrapper(), positions, out var entries2))
            {
                entries = entries2.Select(kv => kv.To(keySelector1, valueSelector1)).ToReadOnlyCollection();
                return true;
            }

            entries = default(ICollection<KeyValuePair<K1, V1>>);
            return false;
        }

        public bool TryGetFullKeysByPartialKey(IEnumerable<T1> partialKey, out ISet<K1> fullKeys)
        {
            if (Source.TryGetFullKeysByPartialKey(partialKey.Select(x => x.To(subKeySelector2)), out var fullKeys2))
            {
                fullKeys = new HashSet<K1>(fullKeys2.Select(k => k.To(keySelector1)));
                return true;
            }

            fullKeys = default(ISet<K1>);
            return false;
        }

        public bool TryGetFullKeysByPartialKey(IList<T1> partialKey, IList<int> positions, out ISet<K1> fullKeys)
        {
            if (Source.TryGetFullKeysByPartialKey(partialKey.Select(x => x.To(subKeySelector2)).ToListWrapper(), positions, out var fullKeys2))
            {
                fullKeys = new HashSet<K1>(fullKeys2.Select(k => k.To(keySelector1)));
                return true;
            }

            fullKeys = default(ISet<K1>);
            return false;
        }

        public bool TryGetValuesByPartialKey(IEnumerable<T1> partialKey, out ICollection<V1> values)
        {
            if (Source.TryGetValuesByPartialKey(partialKey.Select(x => x.To(subKeySelector2)), out var values2))
            {
                values = values2.Select(v => v.To(valueSelector1)).ToReadOnlyCollection();
                return true;
            }

            values = default(ICollection<V1>);
            return false;
        }

        public bool TryGetValuesByPartialKey(IList<T1> partialKey, IList<int> positions, out ICollection<V1> values)
        {
            if (Source.TryGetValuesByPartialKey(partialKey.Select(x => x.To(subKeySelector2)).ToListWrapper(), positions, out var values2))
            {
                values = values2.Select(v => v.To(valueSelector1)).ToReadOnlyCollection();
                return true;
            }

            values = default(ICollection<V1>);
            return false;
        }
    }
}
