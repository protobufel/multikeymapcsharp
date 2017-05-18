using System;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using GitHub.Protobufel.MultiKeyMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using static MultiKeyMapTests.CommonHelpers;

namespace MultiKeyMapTests
{
    [TestClass]
    public class MultiKeyMapsTest
    {
        //private Logger = new Logger()

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void CreateMultiKeyDictionaryWithComparers_Empty()
        {
            CreateMultiKeyDictionaryWithComparersEmptyHelper<int, int[], string>(EqualityComparer<int>.Default,
                EqualityComparer<int[]>.Default);
        }

        private void CreateMultiKeyDictionaryWithComparersEmptyHelper<T, K, V>(
            IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) where K : IEnumerable<T>
        {
            var multiDict = MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(subKeyComparer, fullKeyComparer);

            multiDict.Should().NotBeNull().And.BeEmpty().And.BeAssignableTo(typeof(IMultiKeyMap<T, K, V>))
                .And.BeAssignableTo(typeof(IDictionary<K, V>));

            multiDict.GetType().Should().BeDecoratedWith<SerializableAttribute>();

            Action act = () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(null, fullKeyComparer);
            act.ShouldThrow<ArgumentNullException>().Where(e => e.ParamName.Equals("subKeyComparer"));

            act = () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(subKeyComparer, null);
            act.ShouldThrow<ArgumentNullException>().Where(e => e.ParamName.Equals("fullKeyComparer"));
        }

        [TestMethod]
        public void CreateMultiKeyDictionaryWithComparers()
        {
            CreatesEmptyInstanceHelper(1, new[] { 1, 2, 3 }, new { A = 1, B = "hi" });
            CreatesEmptyInstanceHelper("1", new[] { "1", "2" }, 99);
            CreatesEmptyInstanceHelper(GetClass1("hi"), new List<Class1<string>>() { GetClass1("a"), GetClass1("b") }, true);
            CreatesEmptyInstanceHelper(GetClass1(3.5), new HashSet<Class1<double>>() { GetClass1(1.1), GetClass1(2.2) }, true);
            CreatesEmptyInstanceHelper(GetValueTuple(3.5), new HashSet<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2) }, true);
        }

        private IMultiKeyMap<T, K, V> CreateMultiKeyDictionaryWithComparersHelper<T, K, V>(
            IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer,
            T t, K k, V v) where K : IEnumerable<T>
        {
            var multiDict = MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(subKeyComparer, fullKeyComparer);
            Assert.IsNotNull(multiDict);
            Assert.IsInstanceOfType(multiDict, typeof(IMultiKeyMap<T, K, V>));
            Assert.IsInstanceOfType(multiDict, typeof(IDictionary<K, V>));
            Assert.IsTrue(multiDict.Count == 0);
            return multiDict;
        }

        [TestMethod]
        public void CreateMultiKeyDictionary_CreatesEmptyInstance()
        {
            CreatesEmptyInstanceHelper(1, new[] { 1, 2, 3 }, new { A = 1, B = "hi" });
            CreatesEmptyInstanceHelper("1", new[] { "1", "2" }, 99);
            CreatesEmptyInstanceHelper(GetClass1("hi"), new List<Class1<string>>() { GetClass1("a"), GetClass1("b") }, true);
            CreatesEmptyInstanceHelper(GetClass1(3.5), new HashSet<Class1<double>>() { GetClass1(1.1), GetClass1(2.2) }, true);
            CreatesEmptyInstanceHelper(GetValueTuple(3.5), new HashSet<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2) }, true);
        }

        private IMultiKeyMap<T, K, V> CreatesEmptyInstanceHelper<T, K, V>(T t, K k, V v) where K : IEnumerable<T>
        {
            var multiDict = MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>();
            Assert.IsNotNull(multiDict);
            Assert.IsInstanceOfType(multiDict, typeof(IMultiKeyMap<T, K, V>));
            Assert.IsInstanceOfType(multiDict, typeof(IDictionary<K, V>));
            Assert.IsTrue(multiDict.Count == 0);
            return multiDict;
        }

        [TestMethod]
        public void CreateMultiKeyDictionary_Add()
        {
            AddHelper(1, new[] { 1, 2, 3 }, new { A = 1, B = "hi" });
            AddHelper("1", new[] { "1", "2" }, 99);
            AddHelper(GetClass1("hi"), new List<Class1<string>>() { GetClass1("a"), GetClass1("b") }, true);
            AddHelper(GetClass1(3.5), new HashSet<Class1<double>>() { GetClass1(1.1), GetClass1(2.2) }, true);
            AddHelper(GetValueTuple(3.5), new HashSet<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2) }, true);
        }

        private IMultiKeyMap<T, K, V> AddHelper<T, K, V>(T t, K k, V v) where K : IEnumerable<T>
        {
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            var multiDict = MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>();
            multiDict.Should().NotBeNull().And.BeEmpty().And.BeAssignableTo(typeof(IMultiKeyMap<T, K, V>))
                .And.BeAssignableTo(typeof(IDictionary<K, V>));

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

            return multiDict;
        }

        [TestMethod]
        public void CreateMultiKeyDictionary_CheckComparers()
        {
            DictionaryBasedAddHelper<int, int[], object>(
               new int[][]{
                    new int[]{ 1, 2, 3, 4 }, new int[] { 4, 3, 5 }, new int[] { 5, 1, 8 },
                    new int[] { 1, 2, 3, 4, 5, 6 }, new int[] { 30, -10 } },

               new object[]{
                    new { A = 1, B = "hi" }, new { A = 1, B = "hi2" }, new { A = 1, B = "hi3" },
                    new { A = 1, B = "hi4" },new { A = 1, B = "hi5" }
               });

            DictionaryBasedAddHelper<string, string[], long>(
                new string[][]{
                    new string[]{ "1", "2", "3", "4" }, new string[] { "4", "3", "5" }, new string[] { "5", "1", "8" },
                    new string[] { "1", "2", "3", "4", "5", "6" }, new string[] { "30", "-10" } },

                new long[]{
                    1, 2, 3, 4, 5
                });

            DictionaryBasedAddHelper<Class1<string>, List<Class1<string>>, string>(
               new List<Class1<string>>[] {
                   new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") },
                   new List<Class1<string>>() { GetClass1("C"), GetClass1("M"), GetClass1("a"), GetClass1("Y") },
                   new List<Class1<string>>() { GetClass1("d"), GetClass1("v"), GetClass1("Y"), GetClass1("M") }},

                   new string[] { "1", "2", "3" }
                );

            DictionaryBasedAddHelper<ValueTuple<double>, List<ValueTuple<double>>, bool>(
                new List<ValueTuple<double>>[] {
                    new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) },
                    new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) },
                    new List<ValueTuple<double>>() { GetValueTuple(4.4), GetValueTuple(4.0), GetValueTuple(7.0), GetValueTuple(12.5) },
                    new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(-2.0), GetValueTuple(99.34) },
                },
                new bool[] { true, false, true, true }
                );
        }

        private void DictionaryBasedAddHelper<T, K, V>(K[] ks, V[] vs,
            IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null) where K : IEnumerable<T>
        {
            subKeyComparer = subKeyComparer ?? EqualityComparer<T>.Default;
            fullKeyComparer = fullKeyComparer ?? EqualityComparer<K>.Default;
            AddHelper(() => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(subKeyComparer, fullKeyComparer), ks, vs, subKeyComparer, fullKeyComparer);
        }


        private void AddHelper<T, K, V>(Func<IMultiKeyMap<T, K, V>> supplier, K[] ks, V[] vs,
        IEqualityComparer<T> subKeyComparer, IEqualityComparer<K> fullKeyComparer) where K : IEnumerable<T>
        {
            //assuming
            supplier.Should().NotBeNull();
            ks.Should().HaveSameCount(vs).And.NotContainNulls().And.OnlyHaveUniqueItems();

            var multiDict = supplier.Invoke();
            var expectedEntries = Enumerable.Zip(ks, vs, (k, v) => new KeyValuePair<K, V>(k, v));

            foreach (var entry in expectedEntries)
            {
                multiDict.Add(entry);
            }

            multiDict.Keys.ShouldAllBeEquivalentTo(ks);

            multiDict.Keys.ShouldAllBeEquivalentTo(ks, options => options.Using<K>(
                ctx => ctx.Subject.Should().Equal(ctx.Expectation, (actual, expected) => subKeyComparer.Equals(actual, expected))).WhenTypeIs<K>());

            multiDict.Keys.ShouldAllBeEquivalentTo(ks, options => options.Using<IEnumerable<K>>(
                ctx => ctx.Subject.Should().Equal(ctx.Expectation, (actual, expected) => fullKeyComparer.Equals(actual, expected))).WhenTypeIs<IEnumerable<K>>());


            //multiDict.Should().NotBeEmpty().And.ContainKey(k).And.ContainValue(v).And.Contain(expectedEntry).And.HaveCount(1);

            //bool result = multiDict.TryGetFullKeysByPartialKey(k, out var actualKeys);
            //result.Should().BeTrue();
            //actualKeys.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, k);

            //result = multiDict.TryGetValuesByPartialKey(k, out var actualValues);
            //result.Should().BeTrue();
            //actualValues.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, v);

            //result = multiDict.TryGetEntriesByPartialKey(k, out var actualEntries);
            //result.Should().BeTrue();
            //actualEntries.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, expectedEntry);

            //return multiDict;
        }
    }
}
