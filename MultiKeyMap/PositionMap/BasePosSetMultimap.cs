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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using static GitHub.Protobufel.MultiKeyMap.Base.LiteSetMultimapExtensions;
using GitHub.Protobufel.MultiKeyMap.PositionMask;

namespace GitHub.Protobufel.MultiKeyMap.PositionMap
{
    internal class BasePosSetMultimap<K, V> : IPosSetMultimap<K, V>
    {
        private readonly IDictionary<K, PosLiteSetMultimap<V>> map;
        private readonly IEqualityComparer<V> valueComparer;

        public BasePosSetMultimap(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer, IDictionary<K, PosLiteSetMultimap<V>> map = null)
        {
            this.map = map ?? new Dictionary<K, PosLiteSetMultimap<V>>(keyComparer);
            this.valueComparer = valueComparer;
        }

        protected virtual PosLiteSetMultimap<V> CreateLiteSetMultimap()
        {
            return new PosLiteSetMultimap<V>(new Dictionary<int, ISet<V>>(), valueComparer);
        }

        public virtual void Clear()
        {
            map.Clear();
        }

        public virtual bool TryGetValue(K key, int position, out ISet<V> value, BitArray excludePositions = null)
        {
            if (position < 0) throw new ArgumentOutOfRangeException("position");

            if (!map.TryGetValue(key, out PosLiteSetMultimap<V> liteMultimap))
            {
                value = default(ISet<V>);
                return false;
            }

            return liteMultimap.TryGetValue(position, out value);
        }

        public int TryGetAllValues(K key, out IEnumerable<ISet<V>> value, BitArray excludePositions = null)
        {
            if (!map.TryGetValue(key, out PosLiteSetMultimap<V> liteMultimap))
            {
                value = default(ICollection<ISet<V>>);
                return 0;
            }

            return liteMultimap.TryGetAllValues(out value, excludePositions);
        }

        public virtual bool Add(K key, int position, V value)
        {
            if (map.TryGetValue(key, out PosLiteSetMultimap<V> liteMultimap))
            {
                liteMultimap.Add(position, value);
                return true;
            }

            liteMultimap = CreateLiteSetMultimap();
            liteMultimap.Add(position, value);
            map.Add(key, liteMultimap);
            return false;
        }

        public virtual bool Remove(K key, int position, V value)
        {
            if (map.TryGetValue(key, out PosLiteSetMultimap<V> liteMultimap) && liteMultimap.Remove(position, value))
            {
                if (liteMultimap.Count == 0)
                {
                    map.Remove(key);
                }

                return true;
            }

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


        internal class PosLiteSetMultimap<TValue> : LiteSetMultimap<int, TValue>
        {
            private readonly BitArray fields;

            public PosLiteSetMultimap(IDictionary<int, ISet<TValue>> map, IEqualityComparer<TValue> valueComparer) : base(map, valueComparer)
            {
                fields = new BitArray(0);
            }

            public override bool Add(int key, TValue value)
            {
                if (!base.Add(key, value))
                {
                    fields.SetAndResize(key, true);
                    return false;
                }

                return true;
            }

            public override bool Remove(int key, TValue value)
            {
                return this.Remove(key, value, out bool removedEntireKey);
            }

            public override bool Remove(int key, TValue value, out bool removedEntireKey)
            {
                if (base.Remove(key, value, out removedEntireKey))
                {
                    if (removedEntireKey)
                    {
                        fields.Set(key, false);
                    }

                    return true;
                }

                return false;
            }

            public override void Clear()
            {
                base.Clear();
                fields.Length = 0;
            }

            public override bool TryGetValue(int key, out ISet<TValue> value)
            {
                if (Count == 0 || !fields.Get(key))
                {
                    value = default(ISet<TValue>);
                    return false;
                }

                return base.TryGetValue(key, out value);
            }

            public int TryGetAllValues(out IEnumerable<ISet<TValue>> value, BitArray excludePositions = null)
            {
                value = default(IEnumerable<ISet<TValue>>);

                if (Count == 0)
                {
                    return 0;
                }

                if (excludePositions == null)
                {
                    value = map.Values;
                    return map.Count;
                }

                value = map.Where(kv => !excludePositions[kv.Key]).Select(kv => kv.Value);

                int count = 0;

                foreach (var set in value)
                {
                    count += set.Count;
                }

                if (count == 0)
                {
                    value = default(IEnumerable<ISet<TValue>>);
                }

                return count;
            }

            public BitArray Keys => new BitArray(fields);
        }
    }
}
