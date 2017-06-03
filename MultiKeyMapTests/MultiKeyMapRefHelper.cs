﻿using System;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap;

namespace MultiKeyMapTests
{
    public class MultiKeyMapRefHelper<T, K, V> : MultiKeyMapBaseHelper<T, K, V> where K : class, IEnumerable<T> where T : class
    {
        public MultiKeyMapRefHelper(Func<IMultiKeyMap<T, K, V>> supplier, K k1, K k2, V v1 = default(V), V v2 = default(V)) : base(supplier, k1, k2, v1, v2)
        {
        }
    }
}
