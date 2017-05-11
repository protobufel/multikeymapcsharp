using System;
using GitHub.Protobufel.MultiKeyMap;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;

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

        public void Cleanup()
        {
            multiDict = null;
            mirrorDict = null;
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

            var actualKeys = multiDict.GetFullKeysByPartialKey(k);
            actualKeys.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, k);

            var actualValues = multiDict.GetValuesByPartialKey(k);
            actualValues.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, v);

            var actualEntries = multiDict.GetEntriesByPartialKey(k);
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

            var actualKeys = multiDict.GetFullKeysByPartialKey(k);
            actualKeys.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, k);

            var actualValues = multiDict.GetValuesByPartialKey(k);
            actualValues.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, v);

            var actualEntries = multiDict.GetEntriesByPartialKey(k);
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
            removed.ShouldBeEquivalentTo(true);
            multiDict.Should().HaveCount(0);

            removed = multiDict.Remove(k);
            removed.ShouldBeEquivalentTo(false);
            multiDict.Should().HaveCount(0);

            object nullK = null;
            Action act = () => multiDict.Remove((K)nullK);
            act.ShouldThrow<ArgumentNullException>();
        }

        public void AssertGetFullKeysByPartialKey(IEnumerable<T> partialKey)
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
            var actualKeys = multiDict.GetFullKeysByPartialKey(partialKey);


            actualKeys.Should().NotBeNull().And.OnlyHaveUniqueItems().And.NotContainNulls().And.HaveCount(expectedCount);

            if (k1HasPartialKey)
            {
                actualKeys.Should().Contain(k1);
            }

            if (k2HasPartialKey)
            {
                actualKeys.Should().Contain(k2);
            }
        }

        public void AssertGetValuesByPartialKey(IEnumerable<T> partialKey)
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
            var actualValues = multiDict.GetValuesByPartialKey(partialKey);


            actualValues.Should().NotBeNull().And.OnlyHaveUniqueItems().And.HaveCount(expectedCount);

            if (k1HasPartialKey)
            {
                actualValues.Should().Contain(v1);
            }

            if (k2HasPartialKey)
            {
                actualValues.Should().Contain(v2);
            }
        }

        public void AssertGetEntriesByPartialKey(IEnumerable<T> partialKey)
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
            var actualEntries = multiDict.GetEntriesByPartialKey(partialKey);


            actualEntries.Should().NotBeNull().And.OnlyHaveUniqueItems().And.NotContainNulls().And.HaveCount(expectedCount);

            if (k1HasPartialKey)
            {
                actualEntries.Should().Contain(expectedEntry1);
            }

            if (k2HasPartialKey)
            {
                actualEntries.Should().Contain(expectedEntry1);
            }
        }
    }
}
