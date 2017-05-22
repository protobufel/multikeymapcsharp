using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal class DictionaryBasePosMaskMultiKeyMap<T, K, V> : BasePosMaskMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        public DictionaryBasePosMaskMultiKeyMap(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer)
            : base(new DictionaryBaseMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V>(subKeyComparer.ToSubKeyMaskComparer(),
                fullKeyComparer.ToKeyMaskComparer<T, K>()))
        {
        }

        //[OnDeserialized]
        //protected internal void OnDeserializedRestorePartialMap(StreamingContext context)
        //{
        //}
    }
}
