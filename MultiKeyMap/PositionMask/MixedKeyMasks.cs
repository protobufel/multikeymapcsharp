using System;
using System.Collections;
using System.Collections.Generic;

namespace GitHub.Protobufel.MultiKeyMap
{

    internal class MixedKeyMask<T, K> : KeyMask<T, K>, IEquatable<IKeyMask<T, K>>, IEquatable<K>, IEnumerable<ISubKeyMask<T>> where K : IEnumerable<T>
    {
        private readonly BitArray positions;

        public MixedKeyMask(K key) : base(key)
        {
            this.positions = null;
        }

        public MixedKeyMask(K key, IEnumerable<bool> positions) : base(key)
        {
            if (positions == null) throw new ArgumentNullException("positions");
            this.positions = positions.ToBitArray();
        }

        public MixedKeyMask(K key, BitArray positions) : base(key)
        {
            if (positions == null) throw new ArgumentNullException("positions");
            this.positions = new BitArray(positions);
        }

        public override BitArray Positions => positions ?? new BitArray(0);

        bool IEquatable<K>.Equals(K other)
        {
            return (other == null) ? false : Key.Equals(other);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as KeyMask<T, K>);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override IEnumerator<ISubKeyMask<T>> GetEnumerator()
        {
            if (Positions == null)
            {
                foreach (T subKey in Key)
                {
                    yield return new SubKeyMask<T>(subKey);
                }
            }
            else
            {
                int i = 0;

                foreach (T subKey in Key)
                {
                    if (Positions.TryGet(i++))
                    {
                        yield return new SubKeyMask<T>(subKey, i);
                    }
                    else
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
}
