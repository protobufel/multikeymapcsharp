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

using System.Collections;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    //implementation: IDictionary<K, IDictionary<int, V>> and BitArray as a mask of included keys
    internal interface IPosSetMultimap<K, V>
    {
        void Clear();

        bool Remove(K key, int position, V value);

        int Count { get; }

        bool TryGetValue(K key, int position, out ISet<V> value, BitArray excludePositions = default(BitArray));

        int TryGetAllValues(K key, out IEnumerable<ISet<V>> value, BitArray excludePositions = null);

        bool Add(K key, int position, V value);
    }
}
