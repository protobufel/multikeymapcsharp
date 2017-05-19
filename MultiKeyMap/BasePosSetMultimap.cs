using System.Collections;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;
using static GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    internal class BasePosSetMultimap<K, V> : IPosSetMultimap<K, V>
    {
        private readonly IDictionary<K, PosLiteSetMultimap<V>> map;
        private readonly IEqualityComparer<V> valueComparer;

        public BasePosSetMultimap(IEqualityComparer<K> keyComparer, IEqualityComparer<V> valueComparer) 
            : this(new Dictionary<K, PosLiteSetMultimap<V>>(keyComparer), valueComparer)
        {
        }

        public BasePosSetMultimap(IDictionary<K, PosLiteSetMultimap<V>> map, IEqualityComparer<V> valueComparer)
        {
            this.valueComparer = valueComparer;
            this.map = map;
        }

        internal protected virtual PosLiteSetMultimap<V> CreateLiteSetMultimap()
        {
            return new PosLiteSetMultimap<V>(new Dictionary<int, ISet<V>>(), valueComparer);
        }

        public virtual void Clear()
        {
            map.Clear();
        }

        public virtual bool TryGetValue(K key, int position, out ISet<V> value, BitArray excludePositions = null)
        {
            if (!map.TryGetValue(key, out PosLiteSetMultimap<V> liteMultimap))
            {
                value = default(ISet<V>);
                return false;
            }

            if (position >= 0)
            {
                return liteMultimap.TryGetValue(position, out value);
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

            public bool TryGetAllValues(out ISet<TValue> value, BitArray excludePositions = null)
            {
                value = default(ISet<TValue>);

                if (Count == 0)
                {
                    return false;
                }

                bool first = true;
                int i = 0;

                foreach (var entry in map)
                {
                    if ((excludePositions == null) || !excludePositions[i++])
                    {
                        if (first)
                        {
                            first = false;
                            value = new HashSet<TValue>(entry.Value, ValueComparer);
                        }
                        else
                        {
                            value.UnionWith(entry.Value);
                        }
                    }
                }

                return true;
            }

            public BitArray Keys { get { BitArray result = new BitArray(fields); result.Length = Count; return result; } }
        }
    }
}
