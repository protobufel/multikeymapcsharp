using System;
using System.Collections;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap.LiteSetMultimapExtensions;

namespace GitHub.Protobufel.MultiKeyMap
{
    // Consider IEquatable<K> and  IEquatable<T>, and use them as key types, then both the plain type and its mask wrapper would be acceptable by IDictionary!
    // It is a space saver, but more trouble in code.
    internal interface IKeyMask<T, K> : IEquatable<IKeyMask<T, K>>, IEquatable<K>, IEnumerable<ISubKeyMask<T>> where K : IEnumerable<T>
    {
        K Key { get;}
        BitArray Positions { get; }
    }

    internal interface ISubKeyMask<T> : IEquatable<ISubKeyMask<T>>, IEquatable<T>
    {
        T SubKey { get; }
        int Position { get; }
    }


    internal class KeyMask<T, K> : IKeyMask<T, K> where K : IEnumerable<T>
    {
        private readonly BitArray positions;

        public KeyMask(K key)
        {
            if (!(key is System.ValueType) && (key == null)) throw new ArgumentNullException("key");
            Key = key;
            this.positions = null;
        }

        public KeyMask(K key, IEnumerable<bool> positions)
        {
            if (!(key is System.ValueType) && (key == null)) throw new ArgumentNullException("key");
            if (positions == null) throw new ArgumentNullException("positions");
            Key = key;
            this.positions = positions.ToBitArray();
        }

        public KeyMask(K key, BitArray positions)
        {
            if (!(key is System.ValueType) && (key == null)) throw new ArgumentNullException("key");
            if (positions == null) throw new ArgumentNullException("positions");
            Key = key;
            this.positions = new BitArray(positions);
        }

        public K Key { get; }

        public BitArray Positions => positions ?? new BitArray(0);

        bool IEquatable<K>.Equals(K other)
        {
            return (other == null) ? false : Key.Equals(other);
        }

        public bool Equals(IKeyMask<T, K> other)
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

        public IEnumerator<ISubKeyMask<T>> GetEnumerator()
        {
            if (Positions == null)
            {
                foreach (T subKey in Key)
                {
                    yield return new SubKeyMask<T>(subKey);
                }
            } else
            {
                int i = 0;

                foreach (T subKey in Key)
                {
                    if (Positions.TryGet(i++))
                    {
                        yield return new SubKeyMask<T>(subKey, i);
                    } else
                    {
                        yield return new SubKeyMask<T>(subKey);
                    }
                }
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

        public static IKeyMask<T, K> ToKeyMaskWithBitArray<T, K>(this K key, BitArray positions) where K : IEnumerable<T>
        {
            return new KeyMask<T, K>(key, positions);
        }

        public static IKeyMask<T, K> ToKeyMask<T, K>(this K key, IEnumerable<bool> positions) where K : IEnumerable<T>
        {
            return new KeyMask<T, K>(key, positions);
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

        public static bool TryGet(this BitArray mask, int index)
        {
            return (index >= 0) && (index < mask.Length) && mask.Get(index);
        }
    }
}
