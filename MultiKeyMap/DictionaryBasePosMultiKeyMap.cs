using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal class DictionaryBasePosMultiKeyMap<T, K, V> : BasePosMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        public DictionaryBasePosMultiKeyMap(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) 
            : base(subKeyComparer, fullKeyComparer, 
                  new Dictionary<K, V>(fullKeyComparer), 
                  new BasePosSetMultimap<T, K>(subKeyComparer, fullKeyComparer))
        {
        }

        //public DictionaryBasePosMultiKeyMap(IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) : base(
        //    subKeyComparer, fullKeyComparer,
        //    new Dictionary<K, V>(fullKeyComparer), new Dictionary<T, ISet<K>>(subKeyComparer).ToSetMultimap())
        //{
        //}

        [OnDeserialized]
        protected internal void OnDeserializedRestorePartialMap(StreamingContext context)
        {
            (fullMap as IDeserializationCallback).OnDeserialization(null);

            partMap = new BasePosSetMultimap<T, K>(subKeyComparer, fullKeyComparer);

            foreach (var entry in fullMap)
            {
                AddPartial(entry.Key);
            }
        }
    }
}
