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
using GitHub.Protobufel.MultiKeyMap.Base;

namespace GitHub.Protobufel.MultiKeyMap.Dictionaries
{
    [Serializable]
    internal class DictionaryMaskMultiKeyMap<T, K, V> : PositionalMaskMultiKeyMap<T, K, V> where K : class, IEnumerable<T>
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
        protected void OnDeserialized(StreamingContext context)
        {
            base.OnDeserializedHelper(context);
        }
    }
}
