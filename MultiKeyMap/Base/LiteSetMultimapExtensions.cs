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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap.Base
{
    internal static class LiteSetMultimapExtensions
    {
        public static ILiteSetMultimap<K, V> ToSetMultimap<K, V>(this IDictionary<K, ISet<V>> map, IEqualityComparer<V> valueComparer)
        {
            return new LiteSetMultimap<K, V>(map, valueComparer);
        }

        public static ILiteSetMultimap<K, V> ToSetMultimap<K, V>(this IDictionary<K, ISet<V>> map)
        {
            return ToSetMultimap(map, EqualityComparer<V>.Default);
        }

        internal class LiteSetMultimap<K, V> : ILiteSetMultimap<K, V>
        {
            protected readonly IDictionary<K, ISet<V>> map;
            protected readonly IEqualityComparer<V> valueComparer;

            public LiteSetMultimap(IDictionary<K, ISet<V>> map, IEqualityComparer<V> valueComparer)
            {
                this.map = map;
                this.valueComparer = valueComparer;
            }

            public virtual void Clear()
            {
                map.Clear();
            }

            public virtual bool TryGetValue(K key, out ISet<V> value)
            {
                return map.TryGetValue(key, out value);
            }

            public virtual bool Add(K key, V value)
            {
                if (map.TryGetValue(key, out ISet<V> col))
                {
                    col.Add(value);
                    return true;
                }

                col = new HashSet<V>(valueComparer) { value };
                map.Add(key, col);
                return false;
            }

            public virtual bool Remove(K key, V value)
            {
                return Remove(key, value, out bool removedEntireKey);
            }

            public virtual bool Remove(K key, V value, out bool removedEntireKey)
            {
                if (map.TryGetValue(key, out ISet<V> col) && col.Remove(value))
                {
                    if (col.Count == 0)
                    {
                        map.Remove(key);
                        removedEntireKey = true;
                    }

                    removedEntireKey = false;
                    return true;
                }

                removedEntireKey = false;
                return false;
            }

            public virtual int Count
            {
                get
                {
                    return map.Count;
                }
            }

            public virtual IEqualityComparer<V> ValueComparer => valueComparer;
        }
    }
}
