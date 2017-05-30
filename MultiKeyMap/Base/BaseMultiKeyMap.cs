using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GitHub.Protobufel.MultiKeyMap.Base
{
    [Serializable]
    internal abstract class BaseMultiKeyMap<T, K, V> : IMultiKeyMap<T, K, V> where K : IEnumerable<T>
    {
        protected const float CostRatioOfNewJoinOp = 2.0F;

        protected IDictionary<K, V> fullMap;
        protected internal IEqualityComparer<K> fullKeyComparer;
        protected internal IEqualityComparer<T> subKeyComparer;

        protected BaseMultiKeyMap() { }

        protected BaseMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null,
            IDictionary<K, V> fullMap = null)
        {
            this.subKeyComparer = subKeyComparer ?? EqualityComparer<T>.Default;
            this.fullKeyComparer = fullKeyComparer ?? EqualityComparer<K>.Default;
            this.fullMap = fullMap ?? CreateDictionary<K, V>(fullKeyComparer);
        }


        public abstract bool TryGetFullKeysByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<K> fullKeys);

        #region Implementation of the partial map helpers

        protected abstract void AddPartial(K key);

        protected abstract void DeletePartial(K key);

        protected abstract void ClearPartial();

        #endregion

        protected abstract IDictionary<TKey, TValue> CreateDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer);

        protected virtual IDictionary<TKey, TValue> CreateSupportDictionary<TKey, TValue>(IEqualityComparer<TKey> comparer)
        {
            return CreateDictionary<TKey, TValue>(comparer);
        }

        //protected virtual ILiteSetMultimap<ISubKeyMask<TSubKey>, TKey> CreateLiteSetMultimap<TSubKey, TKey>(
        //    IEqualityComparer<TSubKey> subKeyComparer, IEqualityComparer<TKey> fullKeyComparer)
        //    where TKey : IEnumerable<TSubKey>
        //{
        //    return CreateSupportDictionary<ISubKeyMask<TSubKey>, ISet<TKey>>(subKeyComparer.ToSubKeyMaskComparer())
        //        .ToSetMultimap(fullKeyComparer);
        //}

        protected virtual IEqualityComparer<K> FullKeyComparer => fullKeyComparer;
        protected virtual IEqualityComparer<T> SubKeyComparer => subKeyComparer;

        #region non-positional TryGetsByPartialKey

        public virtual bool TryGetValuesByPartialKey(IEnumerable<T> partialKey, out IEnumerable<V> values)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out IEnumerable<K> fullKeys))
            {
                values = default(IEnumerable<V>);
                return false;
            }

            var result = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    result.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            values = result;
            return true;
        }

        public virtual bool TryGetEntriesByPartialKey(IEnumerable<T> partialKey, out IEnumerable<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(partialKey, out IEnumerable<K> fullKeys))
            {
                entries = default(IEnumerable<KeyValuePair<K, V>>);
                return false;
            }

            var result = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    result.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            entries = result;
            return true;
        }

        public virtual bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out IEnumerable<K> fullKeys)
        {
            return TryGetFullKeysByPartialKey(partialKey, Enumerable.Empty<int>(), out fullKeys);
        }

        #endregion

        #region positioned filtered queries

        public virtual bool TryGetValuesByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<V> values)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out IEnumerable<K> fullKeys))
            {
                values = default(IEnumerable<V>);
                return false;
            }

            var result = new List<V>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    result.Add(value);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            values = result;
            return true;
        }

        public virtual bool TryGetEntriesByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<KeyValuePair<K, V>> entries)
        {
            if (!TryGetFullKeysByPartialKey(subKeys, positions, out IEnumerable<K> fullKeys))
            {
                entries = default(IEnumerable<KeyValuePair<K, V>>);
                return false;
            }

            var result = new List<KeyValuePair<K, V>>();

            foreach (K fullKey in fullKeys)
            {
                if (fullMap.TryGetValue(fullKey, out V value))
                {
                    KeyValuePair<K, V> entry = new KeyValuePair<K, V>(fullKey, value);
                    result.Add(entry);
                }
                else
                {
                    // shouldn't normally happen; only in case of parallel use or as a bug!
                    throw new KeyNotFoundException($"fullMap doesn't have the fullKey '{fullKey}', but should!");
                }
            }

            entries = result;
            return true;
        }

        protected virtual int GetAtOrNegative(IList<int> positionList, int pos)
        {
            return pos < positionList.Count ? positionList[pos] : -1;
        }

        #endregion

        public virtual V this[K key]
        {
            get => fullMap[key];
            set
            {
                int count = fullMap.Count;
                fullMap[key] = value;

                if (fullMap.Count > count)
                {
                    AddPartial(key);
                }
            }
        }

        public virtual ICollection<K> Keys => fullMap.Keys;

        public virtual ICollection<V> Values => fullMap.Values;

        public virtual int Count => fullMap.Count;

        public virtual bool IsReadOnly => fullMap.IsReadOnly;

        public virtual void Add(K key, V value, IEnumerable<bool> positions)
        {
            if (positions == null) throw new ArgumentNullException("positions");
            Add(key, value);
        }

        public virtual void Add(K key, V value)
        {
            fullMap.Add(key, value);
            AddPartial(key);
        }

        void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
        {
            fullMap.Add(item);
            AddPartial(item.Key);
        }

        public virtual void Clear()
        {
            fullMap.Clear();
            ClearPartial();
        }

        bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
        {
            return fullMap.Contains(item);
        }

        public virtual bool ContainsKey(K key)
        {
            return fullMap.ContainsKey(key);
        }

        void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
        {
            fullMap.CopyTo(array, arrayIndex);
        }

        public virtual bool Remove(K key)
        {
            if (fullMap.Remove(key))
            {
                DeletePartial(key);
                return true;
            }

            return false;
        }

        bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
        {
            if (fullMap.Remove(item))
            {
                DeletePartial(item.Key);
                return true;
            }

            return false;
        }

        public virtual bool TryGetValue(K key, out V value)
        {
            return fullMap.TryGetValue(key, out value);
        }

        public virtual IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return fullMap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return fullMap.GetEnumerator();
        }

        protected virtual void OnDeserializedHelper(StreamingContext context)
        {
            (fullMap as IDeserializationCallback).OnDeserialization(null);
        }
    }
}
