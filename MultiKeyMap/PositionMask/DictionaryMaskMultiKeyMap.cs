using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace GitHub.Protobufel.MultiKeyMap
{
    internal class DictionaryMaskMultiKeyMap<T, K, V> : MaskMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        [OnDeserialized]
        protected internal void OnDeserializedCallParent(StreamingContext context)
        {
            OnDeserializedHelper(context);
        }
    }
}
