using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{
    // Consider IEquatable<K> and  IEquatable<T>, and use them as key types, then both the plain type and its mask wrapper would be acceptable by IDictionary!
    // It is a space saver, but more trouble in code.
    internal interface IKeyMask<T, K> : IEquatable<IKeyMask<T, K>>, IEquatable<K>, IEnumerable<ISubKeyMask<T>> where K : IEnumerable<T>
    {
        K Key { get; }
    }

    internal interface ISubKeyMask<T> : IEquatable<ISubKeyMask<T>>, IEquatable<T>
    {
        T SubKey { get; }
        int Position { get; }
    }

    internal class KeyMask<T, K> : IKeyMask<T, K> where K : IEnumerable<T>
    {
        public KeyMask(K key)
        {
            if (!(key is System.ValueType) && (key == null)) throw new ArgumentNullException("key");
            Key = key;
        }

        public K Key { get; }

        bool IEquatable<K>.Equals(K other)
        {
            return (other == null) ? false : Key.Equals(other);
        }

        public virtual bool Equals(IKeyMask<T, K> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return Key.Equals(other.Key);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyMask<T, K>);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public virtual IEnumerator<ISubKeyMask<T>> GetEnumerator()
        {
            int i = 0;

            foreach (T subKey in Key)
            {
                yield return new SubKeyMask<T>(subKey, i);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class SubKeyMask<T> : ISubKeyMask<T>
    {
        public const int NonPositional = -1;

        public SubKeyMask(T subKey) : this(subKey, NonPositional)
        {
        }

        public SubKeyMask(T subKey, int position)
        {
            if (!(subKey is System.ValueType) && (subKey == null)) throw new ArgumentNullException("key");
            SubKey = subKey;
            Position = position;
        }

        public T SubKey { get; }

        public int Position { get; }

        bool IEquatable<T>.Equals(T other)
        {
            return (other == null) ? false : SubKey.Equals(other);
        }

        public override int GetHashCode()
        {
            return SubKey.GetHashCode();
        }

        public bool Equals(ISubKeyMask<T> other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return SubKey.Equals(other.SubKey);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SubKeyMask<T>);
        }
    }

    internal static class KeyMaskExtensions
    {
        public static IKeyMask<T, K> ToKeyMask<T, K>(this K key) where K : IEnumerable<T>
        {
            return new KeyMask<T, K>(key);
        }

        public static ISubKeyMask<T> ToSubKeyMask<T>(this T subKey)
        {
            return new SubKeyMask<T>(subKey);
        }

        public static ISubKeyMask<T> ToSubKeyMask<T>(this T subKey, int position)
        {
            return new SubKeyMask<T>(subKey, position);
        }

        public static KeyValuePair<K, V> ToKeyValuePair<T, K, V>(this KeyValuePair<IKeyMask<T, K>, V> entry) where K : IEnumerable<T>
        {
            if (entry.Key == null) throw new ArgumentNullException("entry.Key");
            return new KeyValuePair<K, V>(entry.Key.Key, entry.Value);
        }

        public static KeyValuePair<IKeyMask<T, K>, V> ToKeyValuePair<T, K, V>(this KeyValuePair<K, V> entry) where K : IEnumerable<T>
        {
            if (entry.Key == null) throw new ArgumentNullException("entry.Key");
            return new KeyValuePair<IKeyMask<T, K>, V>(entry.Key.ToKeyMask<T, K>(), entry.Value);
        }

        #region IEqualityComparers

        public static IEqualityComparer<ISubKeyMask<T>> ToSubKeyMaskComparer<T>(this IEqualityComparer<T> subKeyComparer)
        {
            return new SubKeyMaskComparer<T>(subKeyComparer);
        }

        public class SubKeyMaskComparer<T> : EqualityComparer<ISubKeyMask<T>>
        {
            public SubKeyMaskComparer(IEqualityComparer<T> subKeyComparer)
            {
                SubKeyComparer = subKeyComparer ?? throw new ArgumentNullException("subKeyComparer");
            }

            public IEqualityComparer<T> SubKeyComparer { get; }

            public override bool Equals(ISubKeyMask<T> x, ISubKeyMask<T> y)
            {
                return SubKeyComparer.Equals(x.SubKey, y.SubKey);
            }

            public override int GetHashCode(ISubKeyMask<T> obj)
            {
                return obj?.SubKey?.GetHashCode() ?? 0;
            }
        }

        public static IEqualityComparer<IKeyMask<T, K>> ToKeyMaskComparer<T, K>(this IEqualityComparer<K> keyComparer) where K : IEnumerable<T>
        {
            return new KeyMaskComparer<T, K>(keyComparer);
        }

        public class KeyMaskComparer<T, K> : EqualityComparer<IKeyMask<T, K>> where K : IEnumerable<T>
        {
            public KeyMaskComparer(IEqualityComparer<K> keyComparer)
            {
                KeyComparer = keyComparer ?? throw new ArgumentNullException("keyComparer");
            }

            public IEqualityComparer<K> KeyComparer { get; }

            public override bool Equals(IKeyMask<T, K> x, IKeyMask<T, K> y)
            {
                return KeyComparer.Equals(x.Key, y.Key);
            }

            public override int GetHashCode(IKeyMask<T, K> obj)
            {
                return obj?.Key?.GetHashCode() ?? 0;
            }
        }

        #endregion
    }

    internal static class BitArrayExtensions
    {

        public static bool TryGet(this BitArray mask, int index)
        {
            return (index >= 0) && (index < mask.Length) && mask.Get(index);
        }

        public static int SetAndResize(this BitArray fields, int position, bool value)
        {
            if (position >= fields.Length)
            {
                fields.Length = position + 1;
                fields.Set(position, value);
                return value ? 1 : 0;
            }

            int result = (value == fields.Get(position)) ? 0 : (value ? 1 : -1);
            fields.Set(position, value);
            return result;
        }

        public static BitArray ToBitArray(this IEnumerable<int> list)
        {
            BitArray fields = new BitArray(32);

            foreach (int field in list)
            {
                if (field >= 0) SetAndResize(fields, field, true);
            }

            return fields;
        }

        public static BitArray ToBitArray(this IEnumerable<bool> list)
        {
            switch (list)
            {
                case bool[] boolArray:
                    return new BitArray(boolArray);
                default:
                    BitArray fields = new BitArray(list.Count());

                    int i = 0;

                    foreach (bool field in list)
                    {
                        fields.Set(i++, field);
                    }

                    return fields;
            }
        }

        public static bool IsFalse(this BitArray mask)
        {
            foreach (bool item in mask)
            {
                if (item)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsFalse2(this BitArray mask)
        {
            int[] array = new int[mask.Length / 32 + 1];

            (mask as ICollection).CopyTo(array, 0);

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public static ushort GetTrueCount(this BitArray mask)
        {
            ushort count = 0;

            foreach (var item in mask)
            {
                if ((bool)item)
                {
                    checked
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }

    interface IBitList : IEnumerable<bool>, IEnumerable
    {
        bool this[int index] { get; set; }
        ushort CountTrue { get; }
        ushort Length { get; set; }
        BitList And(BitList value);
        bool Get(int index);
        bool TryGet(int index);
        BitList Not();
        BitList Or(BitList value);
        BitList Set(int index, bool value);
        BitList SetAll(bool value);
        BitList Xor(BitList value);
        BitList Clear();
        BitArray ToBitArray();
    }

    internal class BitList : IBitList
    {
        private readonly BitArray bits;

        public ushort CountTrue { get; private set; }

        public ushort Length
        {
            get { return (ushort)bits.Length; }
            set { bits.Length = value; }
        }

        public bool this[int index] { get => bits.TryGet(index); set => bits.SetAndResize(index, value); }

        public BitList() : this(0)
        {
        }

        public BitList(BitArray bits)
        {
            this.bits = new BitArray(bits);
        }

        public BitList(IBitList bitList)
        {
            bits = bitList.ToBitArray();
            CountTrue = bitList.CountTrue;
        }

        public BitList(int length)
        {
            bits = new BitArray(length);
            CountTrue = 0;
        }

        public BitList(bool[] values)
        {
            bits = new BitArray(values);
            CountTrue = bits.GetTrueCount();
        }

        public BitList(byte[] bytes)
        {
            bits = new BitArray(bytes);
            CountTrue = bits.GetTrueCount();
        }

        public BitList(int[] values)
        {
            bits = new BitArray(values);
            CountTrue = bits.GetTrueCount();
        }

        public BitList(int length, bool defaultValue)
        {
            bits = new BitArray(length, defaultValue);
            CountTrue = defaultValue ? Length : (ushort)0;
        }

        public bool Get(int index)
        {
            return bits.Get(index);
        }

        public bool TryGet(int index)
        {
            return bits.TryGet(index);
        }

        public BitList And(BitList value)
        {
            bits.And(value.bits);
            CountTrue = bits.GetTrueCount();
            return this;
        }

        public BitList Not()
        {
            bits.Not();
            CountTrue = (ushort)(Length - CountTrue);
            return this;
        }

        public BitList Or(BitList value)
        {
            bits.Or(value.bits);
            CountTrue = bits.GetTrueCount();
            return this;
        }

        public BitList Xor(BitList value)
        {
            bits.Xor(value.bits);
            CountTrue = bits.GetTrueCount();
            return this;
        }

        public BitList SetAll(bool value)
        {
            bits.SetAll(value);
            CountTrue = value ? Length : (ushort)0;
            return this;
        }

        public BitList Set(int index, bool value)
        {
            CountTrue = (ushort)(CountTrue + bits.SetAndResize(index, value));
            return this;
        }

        public BitList Clear()
        {
            bits.Length = 0;
            return this;
        }

        public IEnumerator<bool> GetEnumerator()
        {
            foreach (var item in bits)
            {
                yield return (bool)item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return bits.ToString();
        }

        public BitArray ToBitArray()
        {
            return new BitArray(bits);
        }
    }
}
