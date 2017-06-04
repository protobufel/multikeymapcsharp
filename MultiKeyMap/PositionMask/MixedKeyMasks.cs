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

namespace GitHub.Protobufel.MultiKeyMap.PositionMask
{

    internal class MixedKeyMask<T, K> : KeyMask<T, K>, IEquatable<IKeyMask<T, K>>, IEquatable<K>, IEnumerable<ISubKeyMask<T>> where K : class, IEnumerable<T>
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

        public virtual BitArray Positions => positions ?? new BitArray(0);

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
