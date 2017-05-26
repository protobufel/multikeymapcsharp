using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections;

namespace GitHub.Protobufel.MultiKeyMap
{
    public static partial class EnumerableExtensions
    {
        public static IDictionary<K1, V1> ToDictionaryWrapper<K1, V1, K2, V2>(this IDictionary<K2, V2> source,
            Func<K2, K1> keySelector1, Func<V2, V1> valueSelector1,
            Func<K1, K2> keySelector2, Func<V1, V2> valueSelector2)
        {
            return new VirtualDictionary<K1, V1, K2, V2>(source, keySelector1, valueSelector1, 
                keySelector2, valueSelector2);
        }

        public static IDictionary<K1, V1> ToReadOnlyDictionary<K1, V1, K2, V2>(this IDictionary<K2, V2> source,
            Func<K2, K1> keySelector1, Func<V2, V1> valueSelector1,
            Func<K1, K2> keySelector2, Func<V1, V2> valueSelector2)
        {
            return new ReadOnlyDictionary<K1, V1>(new VirtualDictionary<K1, V1, K2, V2>(source, keySelector1, valueSelector1,
                keySelector2, valueSelector2));
        }
    }

    internal class VirtualDictionary<K1, V1, K2, V2> : IDictionary<K1, V1>
    {
        protected IDictionary<K2, V2> source;
        protected Func<K2, K1> keySelector1;
        protected Func<V2, V1> valueSelector1;
        protected Func<K1, K2> keySelector2;
        protected Func<V1, V2> valueSelector2;

        protected ICollection<K1> keys;
        protected ICollection<V1> values;

        public VirtualDictionary(IDictionary<K2, V2> source,
            Func<K2, K1> keySelector1, Func<V2, V1> valueSelector1,
            Func<K1, K2> keySelector2, Func<V1, V2> valueSelector2
            )
        {
            this.source = source ?? throw new ArgumentNullException("source");
            this.keySelector1 = keySelector1 ?? throw new ArgumentNullException("keySelector1");
            this.valueSelector1 = valueSelector1 ?? throw new ArgumentNullException("valueSelector1");
            this.keySelector2 = keySelector2 ?? throw new ArgumentNullException("keySelector2");
            this.valueSelector2 = valueSelector2 ?? throw new ArgumentNullException("valueSelector2");
        }

        public V1 this[K1 key]
        {
            get => source[key.To(keySelector2)].To(valueSelector1);
            set => source[key.To(keySelector2)] = value.To(valueSelector2);
        }

        public ICollection<K1> Keys
        {
            get
            {
                if (keys == null) keys = source.Keys.ToReadOnlyCollection(keySelector1);
                return keys;
            }
        }

        public ICollection<V1> Values
        {
            get
            {
                if (values == null) values = source.Values.ToReadOnlyCollection(valueSelector1);
                return values;
            }
        }

        public int Count => source.Count;

        public bool IsReadOnly => source.IsReadOnly;

        public void Add(K1 key, V1 value)
        {
            source.Add(key.To(keySelector2), value.To(valueSelector2));
        }

        public void Add(KeyValuePair<K1, V1> item)
        {
            source.Add(item.Key.To(keySelector2), item.Value.To(valueSelector2));
        }

        public void Clear()
        {
            source.Clear();
        }

        public bool Contains(KeyValuePair<K1, V1> item)
        {
            return source.Contains(item.To(keySelector2, valueSelector2));
        }

        public bool ContainsKey(K1 key)
        {
            return source.ContainsKey(key.To(keySelector2));
        }

        public void CopyTo(KeyValuePair<K1, V1>[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException("array");
            if (array.GetLowerBound(0) != 0) throw new ArgumentException("array has non-zero lower bound");
            if (array.Length - arrayIndex < keys.Count) throw new ArgumentException("array with offset is too small");
            if ((arrayIndex < 0) || (arrayIndex > array.Length)) throw new ArgumentOutOfRangeException("arrayIndex");

            foreach (var pair in source)
            {
                array[arrayIndex++] = pair.To(keySelector1, valueSelector1);
            }
        }

        public bool Remove(K1 key)
        {
            return source.Remove(key.To(keySelector2));
        }

        public bool Remove(KeyValuePair<K1, V1> item)
        {
            return source.Remove(item.To(keySelector2, valueSelector2));
        }

        public bool TryGetValue(K1 key, out V1 value)
        {
            if (source.TryGetValue(key.To(keySelector2), out var value2))
            {
                value = value2.To(valueSelector1);
                return true;
            }

            value = default(V1);
            return false;
        }

        public IEnumerator<KeyValuePair<K1, V1>> GetEnumerator()
        {
            foreach (var pair in source)
            {
                yield return pair.To(keySelector1, valueSelector1);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    static class VirtualDictionaryHelperExtensions
    {
        public static TResult To<TSource, TResult>(this TSource source, Func<TSource, TResult> selector)
        {
            if (selector == null) throw new ArgumentNullException("selector");
            if (source == null) throw new ArgumentNullException("source");
            return selector.Invoke(source);
        }

        public static KeyValuePair<K2, V2> To<K1, V1, K2, V2>(this KeyValuePair<K1, V1> source,
            Func<K1, K2> keySelector, Func<V1, V2> valueSelector)
        {
            if (keySelector == null) throw new ArgumentNullException("keySelector");
            if (valueSelector == null) throw new ArgumentNullException("valueSelector");
            return new KeyValuePair<K2, V2>(source.Key.To(keySelector), source.Value.To(valueSelector));
        }
    }
}
