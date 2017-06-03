using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GitHub.Protobufel.MultiKeyMap.PositionMap;

namespace GitHub.Protobufel.MultiKeyMap.Dictionaries
{
    [Serializable]
    internal class DictionaryBasePosMultiKeyMap<T, K, V> : BasePosMultiKeyMap<T, K, V> where K : class, IEnumerable<T>
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
        protected void OnDeserializedRestorePartialMap(StreamingContext context)
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
