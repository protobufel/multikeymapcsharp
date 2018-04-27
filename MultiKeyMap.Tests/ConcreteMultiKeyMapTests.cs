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
using System.Linq;
using FluentAssertions;
using GitHub.Protobufel.MultiKeyMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GitHub.Protobufel.MultiKeyMap.MultiKeyMaps.MultiKeyCreationStrategy;
using static MultiKeyMapTests.CommonHelpers;
using static MultiKeyMapTests.ConcreteMultiKeyMapTests.ComparersStrategy;

namespace MultiKeyMapTests
{
    [TestClass]
    public class ConcreteMultiKeyMapTests
    {

        public enum ComparersStrategy
        {
            BothNull,
            BothDefault,
            StructuralFullKeyOnly,
            StructuralBoth
        }

        private MultiKeyMapBaseHelper<int, int[], object> helper1;
        private MultiKeyMapBaseHelper<string, string[], long> helper2;
        private MultiKeyMapBaseHelper<Class1<string>, List<Class1<string>>, string> helper3;
        private MultiKeyMapBaseHelper<ValueTuple<double>, List<ValueTuple<double>>, bool> helper4;

        //[ClassInitialize]
        public ConcreteMultiKeyMapTests()
        {
            helper1 = InitHelper<int, int[], object>(new[] { 1, 2, 3, 4 }, new[] { 2, 5, 6, 3 }, new { A = 1, B = "hi" }, new { A = 2, B = "bye" });
            helper2 = InitHelper<string, string[], long>(new[] { "1", "2", "6", "hi" }, new[] { "hi", "a", "c", "2" }, 99, 199);
            helper3 = InitHelper<Class1<string>, List<Class1<string>>, string>(
                new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") },
                new List<Class1<string>>() { GetClass1("C"), GetClass1("a"), GetClass1("M"), GetClass1("Y") },
                "first", "second");
            helper4 = InitHelper<ValueTuple<double>, List<ValueTuple<double>>, bool>(
                new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) },
                new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) },
                true, false);
        }

        #region Data Test Initialization

        private MultiKeyMapBaseHelper<T, K, V> InitHelper<T, K, V>(K k1, K k2, V v1 = default(V), V v2 = default(V)) where K : class, IEnumerable<T>
        {
            return new MultiKeyMapBaseHelper<T, K, V>(() => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(), k1, k2, v1, v2);
        }

        public Func<IMultiKeyMap<T, K, V>> Supplier<T, K, V>(
            MultiKeyMaps.MultiKeyCreationStrategy strategy = OptimizedForNonPositionalSearch,
            ComparersStrategy compStrategy = BothNull) where K : class, IEnumerable<T>
        {
            switch (compStrategy)
            {
                case BothNull:
                case BothDefault:
                    return () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(strategy);
                case StructuralFullKeyOnly:
                    return () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(EqualityComparer<T>.Default,
                        new EnumerableEqualityComparer<T, K>(EqualityComparer<T>.Default), strategy);
                case StructuralBoth:
                    return () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(EqualityComparer<T>.Default,
                        new EnumerableEqualityComparer<T, K>(EqualityComparer<T>.Default), strategy);
                default:
                    throw new ArgumentOutOfRangeException("compStrategy");
            }
        }

        [Serializable]
        public class EnumerableEqualityComparer<T, K> : EqualityComparer<K> where K : class, IEnumerable<T>
        {
            private IEqualityComparer<T> elementComparer;

            public EnumerableEqualityComparer(IEqualityComparer<T> elementComparer)
            {
                this.elementComparer = elementComparer;
            }

            public EnumerableEqualityComparer() : this(EqualityComparer<T>.Default)
            {
            }

            public override bool Equals(K col1, K col2)
            {
                return Enumerable.SequenceEqual(col1, col2, elementComparer);
            }

            public override int GetHashCode(K col)
            {
                return StructuralComparisons.StructuralEqualityComparer.GetHashCode(col);
            }
        }

        public void InitAll(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            helper1.Init(Supplier<int, int[], object>(strategy, compStrategy));
            helper2.Init(Supplier<string, string[], long>(strategy, compStrategy));
            helper3.Init(Supplier<Class1<string>, List<Class1<string>>, string>(strategy, compStrategy));
            helper4.Init(Supplier<ValueTuple<double>, List<ValueTuple<double>>, bool>(strategy, compStrategy));

            helper1.Init(
                new int[][]{
                    new int[]{ 1, 2, 3, 4 }, new int[] { 4, 3, 5 }, new int[] { 5, 1, 8 },
                    new int[] { 1, 2, 3, 4, 5, 6 }, new int[] { 30, -10 } },

                new object[]{
                    new { A = 1, B = "hi" }, new { A = 1, B = "hi2" }, new { A = 1, B = "hi3" },
                    new { A = 1, B = "hi4" },new { A = 1, B = "hi5" }
                });
            helper2.Init(
                new string[][]{
                    new string[]{ "1", "2", "3", "4" }, new string[] { "4", "3", "5" }, new string[] { "5", "1", "8" },
                    new string[] { "1", "2", "3", "4", "5", "6" }, new string[] { "30", "-10" } },

                new long[]{
                    1, 2, 3, 4, 5
                });
            helper3.Init(
               new List<Class1<string>>[] {
                   new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") },
                   new List<Class1<string>>() { GetClass1("C"), GetClass1("M"), GetClass1("a"), GetClass1("Y") },
                   new List<Class1<string>>() { GetClass1("d"), GetClass1("v"), GetClass1("Y"), GetClass1("M") }},

                   new string[] { "1", "2", "3" }
                );
            helper4.Init(
                new List<ValueTuple<double>>[] {
                    new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) },
                    new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) },
                    new List<ValueTuple<double>>() { GetValueTuple(4.4), GetValueTuple(4.0), GetValueTuple(7.0), GetValueTuple(12.5) },
                    new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(-2.0), GetValueTuple(99.34) },
                },
                new bool[]
                {
                    true,false,true, true
                }
                );
        }
        #endregion


        [TestCleanup]
        public void Cleanup()
        {
            helper1.Cleanup();
            helper2.Cleanup();
            helper3.Cleanup();
            helper4.Cleanup();
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void EmptyInstanceTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertEmptyInstance();
            helper2.AssertEmptyInstance();
            helper3.AssertEmptyInstance();
            helper4.AssertEmptyInstance();
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void AddTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertAdd();
            helper2.AssertAdd();
            helper3.AssertAdd();
            helper4.AssertAdd();
        }

        //[DataTestMethod]
        //[DataRow(OptimizedForPositionalSearch, BothNull)]
        //[DataRow(OptimizedForPositionalSearch, BothDefault)]
        //[DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        //[DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        //[DataRow(OptimizedForNonPositionalSearch, BothNull)]
        //[DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        //[DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        //[DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        //public void AddWithPositionsTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        //{
        //    InitAll(strategy, compStrategy);

        //    helper1.AssertAddWithPositions();
        //    helper2.AssertAddWithPositions();
        //    helper3.AssertAddWithPositions();
        //    helper4.AssertAddWithPositions();
        //}

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void RemoveTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertRemove();
            helper2.AssertRemove();
            helper3.AssertRemove();
            helper4.AssertRemove();
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void AddTwiceSameTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertAddTwiceSame();
            helper2.AssertAddTwiceSame();
            helper3.AssertAddTwiceSame();
            helper4.AssertAddTwiceSame();
        }


        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void AddNullKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertAddNullKey();
            helper2.AssertAddNullKey();
            helper3.AssertAddNullKey();
            helper4.AssertAddNullKey();
        }


        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void SetIndexerTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertSetIndexer();
            helper2.AssertSetIndexer();
            helper3.AssertSetIndexer();
            helper4.AssertSetIndexer();
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void SetIndexerTwiceTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertSetIndexerTwice();
            helper2.AssertSetIndexerTwice();
            helper3.AssertSetIndexerTwice();
            helper4.AssertSetIndexerTwice();
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void TryGetFullKeysByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertTryGetFullKeysByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertTryGetFullKeysByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertTryGetFullKeysByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertTryGetFullKeysByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) });
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void TryGetValuesByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertTryGetValuesByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertTryGetValuesByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertTryGetValuesByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertTryGetValuesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) });
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void TryGetEntriesByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertTryGetEntriesByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertTryGetEntriesByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertTryGetEntriesByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertTryGetEntriesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) });
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void PositionedTryGetFullKeysByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper4.AssertTryGetFullKeysByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) }, new int[] { -1, -1 }, new HashSet<List<ValueTuple<double>>>() {
                new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) },
                new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) }
            });
            helper3.AssertTryGetFullKeysByPartialKey(new[] { GetClass1("M"), GetClass1("b") }, new int[] { 2, -1 },
                new HashSet<List<Class1<string>>>() { new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") } });
            helper1.AssertTryGetFullKeysByPartialKey(new List<int>() { 3, 2 }, new int[] { -1, 1 }, new HashSet<int[]>() { new int[] { 1, 2, 3, 4 }, new int[] { 1, 2, 3, 4, 5, 6 } });
            helper2.AssertTryGetFullKeysByPartialKey(new List<string>() { "6", "2" }, new int[] { 5, -1 }, new HashSet<string[]>() { new string[] { "1", "2", "3", "4", "5", "6" } });
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void PositionedTryGetValuesByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertTryGetValuesByPartialKey(new List<int>() { 3, 2 }, new int[] { -1, 1 }, new List<object>() { new { A = 1, B = "hi" }, new { A = 1, B = "hi4" } });
            helper2.AssertTryGetValuesByPartialKey(new List<string>() { "6", "2" }, new int[] { 5, -1 }, new List<long>() { 4 });
            helper3.AssertTryGetValuesByPartialKey(new[] { GetClass1("M"), GetClass1("b") }, new int[] { 2, -1 }, new List<string>() { "1" });
            helper4.AssertTryGetValuesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) }, new int[] { -1, -1 }, new List<bool>() { true, false });
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void PositionedTryGetEntriesByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            helper1.AssertTryGetEntriesByPartialKey(new List<int>() { 3, 2 }, new int[] { -1, 1 },
                new List<KeyValuePair<int[], object>>() {
                    new KeyValuePair<int[], object>(new int[] { 1, 2, 3, 4 }, new { A = 1, B = "hi" }),
                    new KeyValuePair<int[], object>(new int[] { 1, 2, 3, 4, 5, 6 }, new { A = 1, B = "hi4" })
                });

            helper2.AssertTryGetEntriesByPartialKey(new List<string>() { "6", "2" }, new int[] { 5, -1 },
                new List<KeyValuePair<string[], long>>() { new KeyValuePair<string[], long>(new string[] { "1", "2", "3", "4", "5", "6" }, 4) });

            helper3.AssertTryGetEntriesByPartialKey(new[] { GetClass1("M"), GetClass1("b") }, new int[] { 2, -1 },
                new List<KeyValuePair<List<Class1<string>>, string>>() {
                    new KeyValuePair<List<Class1<string>>, string>(new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") }, "1")});

            helper4.AssertTryGetEntriesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) }, new int[] { -1, -1 },
                new List<KeyValuePair<List<ValueTuple<double>>, bool>>() {
                    new KeyValuePair<List<ValueTuple<double>>, bool>(new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) }, true),
                    new KeyValuePair<List<ValueTuple<double>>, bool>(new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) }, false) });
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, BothNull)]
        [DataRow(OptimizedForPositionalSearch, BothDefault)]
        [DataRow(OptimizedForPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForPositionalSearch, StructuralFullKeyOnly)]
        [DataRow(OptimizedForNonPositionalSearch, BothNull)]
        [DataRow(OptimizedForNonPositionalSearch, BothDefault)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralBoth)]
        [DataRow(OptimizedForNonPositionalSearch, StructuralFullKeyOnly)]
        public void SerializationTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, ComparersStrategy compStrategy)
        {
            InitAll(strategy, compStrategy);

            var myObj1 = MultiKeyMaps.CreateMultiKeyDictionary<int, int[], string>();
            myObj1.Add(new int[] { 1, 2 }, "hi");
            myObj1.Add(new int[] { 3, 4 }, "bye");

            byte[] serialized = SerializeHelper(myObj1);
            var myObj2 = DeserializeHelper<IMultiKeyMap<int, int[], string>>(serialized);
            myObj2.ShouldAllBeEquivalentTo(myObj1);

        }

        [TestMethod]
        public void RemoveByFullKeyTests()
        {
            IMultiKeyMap<string, IEnumerable<string>, string> dict = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>();
            dict.Add(new[] { "first", "crazy", "1st" }, "one");
            dict.Add(new[] { "first", "other" }, "one again");
            dict.Add(new[] { "first", "something else" }, "one a third time");

            IEnumerable<string> values;
            Assert.IsTrue(dict.TryGetValuesByPartialKey(new[] { "first" }, out values));
            Assert.AreEqual(3, values.Count());

            IEnumerable<IEnumerable<string>> fullKeys = new[] { new[] { "first", "crazy", "1st" } };
            bool removedAny = false;
            foreach (var fullKey in fullKeys)
            {
                removedAny |= dict.Remove(fullKey);
            }

            Assert.IsTrue(removedAny);

            var tryGetValuesByPartialKey = dict.TryGetValuesByPartialKey(new[] { "first" }, out values); // Fails here even though the dictionary still contains two values that should match this partial key
            Assert.IsTrue(tryGetValuesByPartialKey);
            Assert.AreEqual(2, values.Count());
        }
    }
}
