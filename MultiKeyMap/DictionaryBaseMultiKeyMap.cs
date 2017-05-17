using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal class DictionaryBaseMultiKeyMap<T, K, V> : BaseMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        public DictionaryBaseMultiKeyMap(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) : base(
            subKeyComparer, fullKeyComparer,
            new Dictionary<K, V>(fullKeyComparer), new Dictionary<T, ISet<K>>(subKeyComparer).ToSetMultimap())
        {
        }

        [OnDeserialized]
        protected internal void OnDeserializedRestorePartialMap(StreamingContext context)
        {
            (fullMap as IDeserializationCallback).OnDeserialization(null);

            partMap = new Dictionary<T, ISet<K>>(subKeyComparer).ToSetMultimap(fullKeyComparer);

            foreach (var entry in fullMap)
            {
                AddPartial(entry.Key);
            }
        }
    }
}
