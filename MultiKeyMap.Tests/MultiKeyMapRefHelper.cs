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
using GitHub.Protobufel.MultiKeyMap;

namespace MultiKeyMapTests
{
    public class MultiKeyMapRefHelper<T, K, V> : MultiKeyMapBaseHelper<T, K, V> where K : class, IEnumerable<T> where T : class
    {
        public MultiKeyMapRefHelper(Func<IMultiKeyMap<T, K, V>> supplier, K k1, K k2, V v1 = default(V), V v2 = default(V)) : base(supplier, k1, k2, v1, v2)
        {
        }
    }
}
