using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GitHub.Protobufel.MultiKeyMap;
using GitHub.Protobufel.MultiKeyMap.Extensions;

namespace MultiKeyMapTests
{
    public class MultiKeyMapBaseHelper<T, K, V> where K : IEnumerable<T>
    {

        private IMultiKeyMap<T, K, V> multiDict;
        private IDictionary<K, V> mirrorDict;
        private K k1 = default(K);
        private V v1 = default(V);
        private K k2 = default(K);
        private V v2 = default(V);

        // an improved test MultiKeyMap
        private IMultiKeyMap<T, K, V> queryMap1;
        private IMultiKeyMap<T, K, V> queryMap2;

        public IMultiKeyMap<T, K, V> QueryMap1 => queryMap1;
        public IMultiKeyMap<T, K, V> QueryMap2 => queryMap2;


        public MultiKeyMapBaseHelper(Func<IMultiKeyMap<T, K, V>> supplier, K k1, K k2, V v1 = default(V), V v2 = default(V))
        {
            Supplier = supplier;
            this.k1 = k1;
            this.v1 = v1;
            this.k2 = k2;
            this.v2 = v2;
        }

        public void Init()
        {
            multiDict = Supplier.Invoke();
            mirrorDict = new Dictionary<K, V>();
        }


        public void Init(K[] keys, V[] values, Func<IMultiKeyMap<T, K, V>> supplier = null)
        {
            supplier = supplier ?? Supplier;
            queryMap1 = TestHelpers.CreateMultiKeyMap(keys, values, supplier);
            queryMap2 = TestHelpers.CreateMultiKeyMap(keys, values, supplier);
        }

        public void Cleanup()
        {
            multiDict = null;
            mirrorDict = null;
            queryMap1 = null;
            queryMap2 = null;
        }

        // () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>();
        public Func<IMultiKeyMap<T, K, V>> Supplier { get; set; }

        public void AssertEmptyInstance()
        {
            multiDict.Should().NotBeNull().And.BeEmpty()
                .And.BeAssignableTo(typeof(IMultiKeyMap<T, K, V>)).And.BeAssignableTo(typeof(IDictionary<K, V>))
                .And.HaveCount(0);
        }

        public void AssertAdd()
        {
            K k = k1;
            V v = v1;
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            multiDict.Add(k, v);
            multiDict.Should().NotBeEmpty().And.ContainKey(k).And.ContainValue(v).And.Contain(expectedEntry).And.HaveCount(1);

            bool result = multiDict.TryGetFullKeysByPartialKey(k, out var actualKeys);
            result.Should().BeTrue();
            actualKeys.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, k);

            result = multiDict.TryGetValuesByPartialKey(k, out var actualValues);
            result.Should().BeTrue();
            actualValues.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, v);

            result = multiDict.TryGetEntriesByPartialKey(k, out var actualEntries);
            result.Should().BeTrue();
            actualEntries.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, expectedEntry);
        }

        public void AssertAddTwiceSame()
        {
            K k = k1;
            V v = v1;
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            multiDict.Add(k, v);

            Action act = () => multiDict.Add(k, v);
            act.ShouldThrow<ArgumentException>();
        }

        public void AssertAddNullKey()
        {
            K k = k1;
            V v = v1;
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            object nullK = null;
            Action act = () => multiDict.Add((K)nullK, v);
            act.ShouldThrow<ArgumentNullException>();
        }

        public void AssertSetIndexer()
        {
            K k = k1;
            V v = v1;
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            multiDict[k] = v;
            multiDict.Should().NotBeEmpty().And.ContainKey(k).And.ContainValue(v).And.Contain(expectedEntry).And.HaveCount(1);

            bool result = multiDict.TryGetFullKeysByPartialKey(k, out var actualKeys);
            result.Should().BeTrue();
            actualKeys.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, k);

            result = multiDict.TryGetValuesByPartialKey(k, out var actualValues);
            result.Should().BeTrue();
            actualValues.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, v);

            result = multiDict.TryGetEntriesByPartialKey(k, out var actualEntries);
            result.Should().BeTrue();
            actualEntries.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, expectedEntry);
        }

        public void AssertSetIndexerTwice()
        {
            K k = k1;
            V v = v1;
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            multiDict[k] = v;

            Action act = () => multiDict[k] = v;
            act.ShouldNotThrow<ArgumentException>();
            multiDict.Should().HaveCount(1);
            multiDict[k].ShouldBeEquivalentTo(v);
        }

        public void AssertRemove()
        {
            K k = k1;
            V v = v1;
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            multiDict.Add(k, v);
            bool removed = multiDict.Remove(k);
            removed.Should().Be(true);
            multiDict.Should().HaveCount(0);

            removed = multiDict.Remove(k);
            removed.Should().Be(false);
            multiDict.Should().HaveCount(0);

            object nullK = null;
            Action act = () => multiDict.Remove((K)nullK);
            act.ShouldThrow<ArgumentNullException>();
        }

        public void AssertTryGetFullKeysByPartialKey(IEnumerable<T> partialKey)
        {
            //KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);
            //Assumptions:
            k1.Should().HaveCount((x) => x >= 3, "ASSUMPTION!");
            k2.Should().HaveCount((x) => x >= 3, "ASSUMPTION!");
            k1.Should().NotEqual(k2, "ASSUMPTION!");

            bool k1HasPartialKey = Enumerable.Intersect(k1, partialKey).Count().Equals(Enumerable.Count(partialKey));
            bool k2HasPartialKey = Enumerable.Intersect(k2, partialKey).Count().Equals(Enumerable.Count(partialKey));
            int expectedCount = (k1HasPartialKey ? 1 : 0) + (k2HasPartialKey ? 1 : 0);

            multiDict.Add(k1, v1);
            multiDict.Add(k2, v2);
            bool result = multiDict.TryGetFullKeysByPartialKey(partialKey, out var actualKeys);
            result.Should().Be(expectedCount > 0);

            if (!result)
            {
                actualKeys.Should().Equal(default(ISet<K>));
            }
            else
            {
                actualKeys.Should().NotBeNull().And.OnlyHaveUniqueItems().And.NotContainNulls().And.HaveCount(expectedCount);
            }

            if (k1HasPartialKey)
            {
                actualKeys.Should().Contain(k1);
            }

            if (k2HasPartialKey)
            {
                actualKeys.Should().Contain(k2);
            }
        }

        public void AssertTryGetValuesByPartialKey(IEnumerable<T> partialKey)
        {
            //KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);
            //Assumptions:
            k1.Should().HaveCount((x) => x >= 3, "ASSUMPTION!");
            k2.Should().HaveCount((x) => x >= 3, "ASSUMPTION!");
            k1.Should().NotEqual(k2, "ASSUMPTION!");

            bool k1HasPartialKey = Enumerable.Intersect(k1, partialKey).Count().Equals(Enumerable.Count(partialKey));
            bool k2HasPartialKey = Enumerable.Intersect(k2, partialKey).Count().Equals(Enumerable.Count(partialKey));
            int expectedCount = (k1HasPartialKey ? 1 : 0) + (k2HasPartialKey ? 1 : 0);

            multiDict.Add(k1, v1);
            multiDict.Add(k2, v2);
            bool result = multiDict.TryGetValuesByPartialKey(partialKey, out var actualValues);
            result.Should().Be(expectedCount > 0);

            if (!result)
            {
                actualValues.Should().Equal(default(ICollection<V>));
            }
            else
            {
                actualValues.Should().NotBeNull().And.OnlyHaveUniqueItems().And.HaveCount(expectedCount);
            }

            if (k1HasPartialKey)
            {
                actualValues.Should().Contain(v1);
            }

            if (k2HasPartialKey)
            {
                actualValues.Should().Contain(v2);
            }
        }

        public void AssertTryGetEntriesByPartialKey(IEnumerable<T> partialKey)
        {
            KeyValuePair<K, V> expectedEntry1 = new KeyValuePair<K, V>(k1, v1);
            KeyValuePair<K, V> expectedEntry2 = new KeyValuePair<K, V>(k2, v2);

            //Assumptions:
            k1.Should().HaveCount((x) => x >= 3, "ASSUMPTION!");
            k2.Should().HaveCount((x) => x >= 3, "ASSUMPTION!");
            k1.Should().NotEqual(k2, "ASSUMPTION!");

            bool k1HasPartialKey = Enumerable.Intersect(k1, partialKey).Count().Equals(Enumerable.Count(partialKey));
            bool k2HasPartialKey = Enumerable.Intersect(k2, partialKey).Count().Equals(Enumerable.Count(partialKey));
            int expectedCount = (k1HasPartialKey ? 1 : 0) + (k2HasPartialKey ? 1 : 0);

            multiDict.Add(k1, v1);
            multiDict.Add(k2, v2);
            bool result = multiDict.TryGetEntriesByPartialKey(partialKey, out var actualEntries);
            result.Should().Be(expectedCount > 0);

            if (!result)
            {
                actualEntries.Should().Equal(default(ICollection<V>));
            }
            else
            {
                actualEntries.Should().NotBeNull().And.OnlyHaveUniqueItems().And.NotContainNulls().And.HaveCount(expectedCount);
            }

            if (k1HasPartialKey)
            {
                actualEntries.Should().Contain(expectedEntry1);
            }

            if (k2HasPartialKey)
            {
                actualEntries.Should().Contain(expectedEntry1);
            }
        }

        public void AssertTryGetFullKeysByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, ISet<K> expectedOut)
        {
            IEnumerable<(int position, T subKey)> partialKey = subKeys.Zip(positions, (key, pos) => (pos, key));

            bool actualReturn = queryMap1.TryGetFullKeysByPartialKey(partialKey, out var actualOut);

            actualReturn.Should().Be(expectedOut != default(ISet<K>));
            actualOut.ShouldAllBeEquivalentTo(expectedOut);
        }

        public void AssertTryGetValuesByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, ICollection<V> expectedOut)
        {
            IEnumerable<(int position, T subKey)> partialKey = subKeys.Zip(positions, (key, pos) => (pos, key));

            bool actualReturn = queryMap1.TryGetValuesByPartialKey(partialKey, out ICollection<V> actualOut);

            actualReturn.Should().Be(expectedOut != default(ICollection<V>));
            actualOut.ShouldAllBeEquivalentTo(expectedOut);
        }

        public void AssertTryGetEntriesByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, ICollection<KeyValuePair<K, V>> expectedOut)
        {
            IEnumerable<(int position, T subKey)> partialKey = subKeys.Zip(positions, (key, pos) => (pos, key));

            bool actualReturn = queryMap1.TryGetEntriesByPartialKey(partialKey, out ICollection<KeyValuePair<K, V>> actualOut);

            actualReturn.Should().Be(expectedOut != default(ICollection<KeyValuePair<K, V>>));
            actualOut.ShouldAllBeEquivalentTo(expectedOut);
        }


        public void AssertSerialization()
        {
            //queryMap1.Should().
        }
    }
}
