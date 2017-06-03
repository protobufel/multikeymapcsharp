using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.Base;

namespace GitHub.Protobufel.MultiKeyMap.Dictionaries
{
    [Serializable]
    internal class DictionaryNonPositionalMultiKeyMap<T, K, V> : NonPositionalBaseMultiKeyMap<T, K, V> where K : class, IEnumerable<T>
    {
        public DictionaryNonPositionalMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null)
            : base(subKeyComparer, fullKeyComparer)
        {
        }

        protected override IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            return new Dictionary<TKey, TValue>(comparer ?? throw new ArgumentNullException("comparer"));
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context)
        {
            base.OnDeserializedHelper(context);
        }
    }
}
