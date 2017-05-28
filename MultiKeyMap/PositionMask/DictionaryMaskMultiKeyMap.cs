using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GitHub.Protobufel.MultiKeyMap
{
    internal static partial class EnumerableExtensions
    {
        internal static IMultiKeyMap<T, K, V> CreateDictionaryPositionOptimizedMultiKeyMap<T, K, V>(
            IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null)
        where K : IEnumerable<T>
        {
            return new PositionOptimizedMultiKeyMap<T, K, V>(new DictionaryMaskMultiKeyMap<T, K, V>(
                subKeyComparer, fullKeyComparer));
        }
    }

    internal class DictionaryMaskMultiKeyMap<T, K, V> : MaskMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        public DictionaryMaskMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null)
            : base(subKeyComparer, fullKeyComparer)
        {
        }

        protected override IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            return new Dictionary<TKey, TValue>(comparer ?? throw new ArgumentNullException("comparer"));
        }

        [OnDeserialized]
        protected internal void OnDeserializedCallParent(StreamingContext context)
        {
            OnDeserializedHelper(context);
        }
    }
}
