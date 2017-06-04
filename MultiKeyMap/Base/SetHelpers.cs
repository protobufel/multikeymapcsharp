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

using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap.Base
{
    static class SetHelpers
    {

        public static bool IntersectWith<E>(HashSet<E> set, IEnumerable<ISet<E>> sets, out HashSet<E> result)
        {
            if ((set.Count == 0) || !sets.Any())
            {
                result = default(HashSet<E>);
                return false;
            }

            result = new HashSet<E>(set.Comparer);

            foreach (var subSet in sets)
            {
                foreach (E item in subSet)
                {
                    if (set.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }

            if (result.Count == 0)
            {
                result = default(HashSet<E>);
                return false;
            }

            return true;
        }

        public static bool TryJoinSets<E>(IEnumerable<ISet<E>> sets, IEqualityComparer<E> comparer, out ISet<E> result)
        {
            bool first = true;
            result = null;

            foreach (var set in sets)
            {
                if (first)
                {
                    first = false;
                    result = new HashSet<E>(set, comparer);
                }
                else
                {
                    result.UnionWith(set);
                }
            }

            return (result != null) && (result.Count > 0);
        }
    }
}
