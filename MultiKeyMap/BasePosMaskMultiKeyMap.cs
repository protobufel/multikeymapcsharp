using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    [Serializable]
    internal abstract class BasePosMaskMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        protected IMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V> map;
        [NonSerialized]
        protected KeyCollection keys;

        internal BasePosMaskMultiKeyMap(IMultiKeyMap<ISubKeyMask<T>, IKeyMask<T, K>, V> map)
        {
            this.map = map ?? throw new ArgumentNullException("map");
        }

        public virtual V this[K key] { get => map[key.ToKeyMask<T, K>()]; set => map[key.ToKeyMask<T, K>()] = value; }

        public virtual ICollection<K> Keys
        {
            get
            {
                if (keys == null) keys = new KeyCollection(map.Keys);
                return keys;
            }
        }

        public virtual ICollection<V> Values => map.Values;

        public virtual int Count => map.Count;

        public virtual bool IsReadOnly => map.IsReadOnly;

        public virtual void Add(K key, V value)
        {
            if (key == null) throw new ArgumentNullException("key");
            map.Add(key.ToKeyMask<T, K>(), value);
        }

        public virtual void Add(K key, V value, IEnumerable<bool> positions)
        {
            if (key == null) throw new ArgumentNullException("key");
            IEnumerable<bool> posMask = positions ?? throw new ArgumentNullException("positions");
            map.Add(key.ToKeyMask<T, K>(posMask), value);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> entry)
        {
            if (entry.Key == null) throw new ArgumentNullException("entry.Key");
            map.Add(entry.Key.ToKeyMask<T, K>(), entry.Value);
        }

        public virtual void Clear()
        {
            map.Clear();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> entry)
        {
            if (entry.Key == null) throw new ArgumentNullException("entry.Key");
            return map.Contains(entry.ToKeyValuePair<T, K, V>());
        }

        public virtual bool ContainsKey(K key)
        {
            if (key == null) throw new ArgumentNullException("key");
            return map.ContainsKey(key.ToKeyMask<T, K>());
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (array.Length - arrayIndex < keys.Count) throw new ArgumentException("array with offset is too small");
            if ((arrayIndex < 0) || (arrayIndex > array.Length)) throw new ArgumentOutOfRangeException("arrayIndex");

            int i = 0;

            foreach (var keyMask in map)
            {
                array[i++] = keyMask.ToKeyValuePair();
            }
        }

        IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
        {
            foreach (KeyValuePair<IKeyMask<T, K>, V> entry in map)
            {
                yield return entry.ToKeyValuePair();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<KeyValuePair<K, V>>).GetEnumerator();
        }

        public virtual bool Remove(K key)
        {
            if (key == null) throw new ArgumentNullException("key");
            return map.Remove(key.ToKeyMask<T, K>());
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> entry)
        {
            if (entry.Key == null) throw new ArgumentNullException("entry.Key");
            return map.Remove(entry.ToKeyValuePair<T, K, V>());
        }

        public virtual bool TryGetEntriesByPartialKey(IEnumerable<T> partialKey, out ICollection<KeyValuePair<K, V>> entries)
        {
            //return map.TryGetEntriesByPartialKey(partialKey, out entries);

            if (!TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                entries = default(ICollection<KeyValuePair<K, V>>);
                return false;
            }

            entries = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    entries.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public virtual bool TryGetEntriesByPartialKey(IList<T> subKeys, IList<int> positions, out ICollection<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out ISet<K> fullKeys))
            {
                entries = default(ICollection<KeyValuePair<K, V>>);
                return false;
            }

            entries = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    entries.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public virtual bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out ISet<K> fullKeys)
        {
            if (partialKey == null) throw new ArgumentNullException();
            return TryGetFullKeysByPartialKey(AsList(partialKey), Enumerable.Empty<int>() as IList<int>, out fullKeys);
        }

        protected static IList<T> AsList(IEnumerable<T> enumerable)
        {
            return (enumerable as IList<T>) ?? new List<T>(enumerable);
        }

        public virtual bool TryGetFullKeysByPartialKey(IList<T> subKeys, IList<int> positions, out ISet<K> fullKeys)
        {
            var subKeyMasks = subKeys.Select<T, ISubKeyMask<T>>((subKey, index) =>
            {
                if ((index < positions.Count) && (positions[index] >= 0))
                {
                    return new SubKeyMask<T>(subKey, positions[index]);
                }
                else
                {
                    return new SubKeyMask<T>(subKey);
                }
            });

            if (!map.TryGetFullKeysByPartialKey(subKeyMasks, out ISet<IKeyMask<T, K>> keyMasks))
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            fullKeys = new HashSet<K>(keyMasks.Select(keyMask => keyMask.Key)); //TODO: will default equality comparer be enough?
            return true;
        }

        public virtual bool TryGetValue(K key, out V value)
        {
            return map.TryGetValue(key.ToKeyMask<T, K>(), out value);
        }

        public virtual bool TryGetValuesByPartialKey(IEnumerable<T> partialKey, out ICollection<V> values)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out ISet<K> fullKeys))
            {
                values = default(ICollection<V>);
                return false;
            }

            values = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (TryGetValue(fullKey, out V value))
                {
                    values.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }

        public virtual bool TryGetValuesByPartialKey(IList<T> subKeys, IList<int> positions, out ICollection<V> values)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out ISet<K> fullKeys))
            {
                values = default(ICollection<V>);
                return false;
            }

            values = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (TryGetValue(fullKey, out V value))
                {
                    values.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            return true;
        }


        sealed public class KeyCollection : ICollection<K>, ICollection, IReadOnlyCollection<K>
        {
            private ICollection<IKeyMask<T, K>> keys;

            public KeyCollection(ICollection<IKeyMask<T, K>> keys)
            {
                this.keys = keys ?? throw new ArgumentNullException("keys");
            }

            public int Count => keys.Count;

            bool ICollection<K>.IsReadOnly => keys.IsReadOnly;

            bool ICollection.IsSynchronized => (keys as ICollection).IsSynchronized;

            object ICollection.SyncRoot => (keys as ICollection).SyncRoot;

            void ICollection<K>.Add(K item)
            {
                throw new NotSupportedException("Add");
            }

            void ICollection<K>.Clear()
            {
                throw new NotSupportedException("Clear");
            }

            bool ICollection<K>.Contains(K item)
            {
                return keys.Contains(item.ToKeyMask<T, K>());
            }

            public void CopyTo(K[] array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException("array");
                if (array.Length - arrayIndex < keys.Count) throw new ArgumentException("array with offset is too small");
                if ((arrayIndex < 0) || (arrayIndex > array.Length)) throw new ArgumentOutOfRangeException("arrayIndex");

                int i = 0;

                foreach (var keyMask in keys)
                {
                    array[i++] = keyMask.Key;
                }
            }

            void ICollection.CopyTo(Array array, int arrayIndex)
            {
                if (array == null) throw new ArgumentNullException("array");
                if (array.Rank != 1) throw new ArgumentException("array rank is not 1");
                if (array.GetLowerBound(0) != 0) throw new ArgumentException("array lower bound is not 0");
                if ((arrayIndex < 0) || (arrayIndex > array.Length)) throw new ArgumentOutOfRangeException("arrayIndex");


                switch (array)
                {
                    case K[] typedArray:
                        CopyTo(typedArray, arrayIndex);
                        return;
                    case object[] objArray:
                        int i = 0;

                        foreach (var keyMask in keys)
                        {
                            objArray[i++] = keyMask.Key;
                        }

                        return;
                    default:
                        throw new ArgumentException("invalid array type");
                }
            }

            IEnumerator<K> IEnumerable<K>.GetEnumerator()
            {
                foreach (var keyMask in keys)
                {
                    yield return keyMask.Key;
                }
            }

            bool ICollection<K>.Remove(K item)
            {
                throw new NotSupportedException("Remove");
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return keys.GetEnumerator();
            }
        }
    }
}
