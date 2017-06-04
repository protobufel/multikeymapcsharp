/*
Copyright 2017 David Tesler

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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
