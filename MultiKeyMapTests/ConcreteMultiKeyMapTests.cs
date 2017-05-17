using System;
//using System.Linq;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MultiKeyMapTests
{
    [TestClass]
    public class ConcreteMultiKeyMapTests
    {
        private MultiKeyMapBaseHelper<int, int[], object> helper1;
        private MultiKeyMapBaseHelper<string, string[], long> helper2;
        private MultiKeyMapBaseHelper<Class1<string>, List<Class1<string>>, string> helper3;
        private MultiKeyMapBaseHelper<ValueTuple<double>, List<ValueTuple<double>>, bool> helper4;

        //[ClassInitialize]
        public ConcreteMultiKeyMapTests()
        {
            helper1 = InitHelper<int, int[], object>(new[] { 1, 2, 3, 4 }, new[] { 2, 5, 6, 3 }, new { A = 1, B = "hi" });
            helper2 = InitHelper<string, string[], long>(new[] { "1", "2", "6", "hi" }, new[] { "hi", "a", "c", "2" }, 99);
            helper3 = InitHelper<Class1<string>, List<Class1<string>>, string>(
                new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") },
                new List<Class1<string>>() { GetClass1("C"), GetClass1("a"), GetClass1("M"), GetClass1("Y") },
                "first", "second");
            helper4 = InitHelper<ValueTuple<double>, List<ValueTuple<double>>, bool>(
                new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) },
                new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) },
                true, false);
        }

        [TestInitialize]
        public void Init()
        {
            helper1.Init();
            helper2.Init();
            helper3.Init();
            helper4.Init();

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

        [TestCleanup]
        public void Cleanup()
        {
            helper1.Cleanup();
            helper2.Cleanup();
            helper3.Cleanup();
            helper4.Cleanup();
        }

        private MultiKeyMapBaseHelper<T, K, V> InitHelper<T, K, V>(K k1, K k2, V v1 = default(V), V v2 = default(V)) where K : IEnumerable<T>
        {
            return new MultiKeyMapBaseHelper<T, K, V>(() => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>(), k1, k2, v1, v2);
        }

        [TestMethod]
        public void EmptyInstanceTest()
        {
            helper1.AssertEmptyInstance();
            helper2.AssertEmptyInstance();
            helper3.AssertEmptyInstance();
            helper4.AssertEmptyInstance();
        }

        [TestMethod]
        public void AddTest()
        {
            helper1.AssertAdd();
            helper2.AssertAdd();
            helper3.AssertAdd();
            helper4.AssertAdd();
        }

        [TestMethod]
        public void RemoveTest()
        {
            helper1.AssertRemove();
            helper2.AssertRemove();
            helper3.AssertRemove();
            helper4.AssertRemove();
        }

        [TestMethod]
        public void AddTwiceSameTest()
        {
            helper1.AssertAddTwiceSame();
            helper2.AssertAddTwiceSame();
            helper3.AssertAddTwiceSame();
            helper4.AssertAddTwiceSame();
        }


        [TestMethod]
        public void AddNullKeyTest()
        {
            helper1.AssertAddNullKey();
            helper2.AssertAddNullKey();
            helper3.AssertAddNullKey();
            helper4.AssertAddNullKey();
        }


        [TestMethod]
        public void SetIndexerTest()
        {
            helper1.AssertSetIndexer();
            helper2.AssertSetIndexer();
            helper3.AssertSetIndexer();
            helper4.AssertSetIndexer();
        }

        [TestMethod]
        public void SetIndexerTwiceTest()
        {
            helper1.AssertSetIndexerTwice();
            helper2.AssertSetIndexerTwice();
            helper3.AssertSetIndexerTwice();
            helper4.AssertSetIndexerTwice();
        }

        [TestMethod]
        public void TryGetFullKeysByPartialKeyTest()
        {
            helper1.AssertTryGetFullKeysByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertTryGetFullKeysByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertTryGetFullKeysByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertTryGetFullKeysByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) });
        }

        [TestMethod]
        public void TryGetValuesByPartialKeyTest()
        {
            helper1.AssertTryGetValuesByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertTryGetValuesByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertTryGetValuesByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertTryGetValuesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) });
        }

        [TestMethod]
        public void TryGetEntriesByPartialKeyTest()
        {
            helper1.AssertTryGetEntriesByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertTryGetEntriesByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertTryGetEntriesByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertTryGetEntriesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) });
        }

        [TestMethod]
        public void PositionedTryGetFullKeysByPartialKeyTest()
        {
            helper4.AssertTryGetFullKeysByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) }, new int[] { -1, -1 }, new HashSet<List<ValueTuple<double>>>() {
                new List<ValueTuple<double>>() { GetValueTuple(1.1), GetValueTuple(2.2), GetValueTuple(3.3) },
                new List<ValueTuple<double>>() { GetValueTuple(3.3), GetValueTuple(4.0), GetValueTuple(5.5), GetValueTuple(2.2) }
            });
            helper3.AssertTryGetFullKeysByPartialKey(new[] { GetClass1("M"), GetClass1("b") }, new int[] { 2, -1 },
                new HashSet<List<Class1<string>>>() { new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M") } });
            helper1.AssertTryGetFullKeysByPartialKey(new List<int>() { 3, 2 }, new int[] { -1, 1 }, new HashSet<int[]>() { new int[] { 1, 2, 3, 4 }, new int[] { 1, 2, 3, 4, 5, 6 } });
            helper2.AssertTryGetFullKeysByPartialKey(new List<string>() { "6", "2" }, new int[] { 5, -1 }, new HashSet<string[]>() { new string[] { "1", "2", "3", "4", "5", "6" } });
        }

        [TestMethod]
        public void PositionedTryGetValuesByPartialKeyTest()
        {
            helper1.AssertTryGetValuesByPartialKey(new List<int>() { 3, 2 }, new int[] { -1, 1 }, new List<object>() { new { A = 1, B = "hi" }, new { A = 1, B = "hi4" } });
            helper2.AssertTryGetValuesByPartialKey(new List<string>() { "6", "2" }, new int[] { 5, -1 }, new List<long>() { 4 });
            helper3.AssertTryGetValuesByPartialKey(new[] { GetClass1("M"), GetClass1("b") }, new int[] { 2, -1 }, new List<string>() { "1" });
            helper4.AssertTryGetValuesByPartialKey(new[] { GetValueTuple(3.3), GetValueTuple(2.2) }, new int[] { -1, -1 }, new List<bool>() { true, false });
        }

        [TestMethod]
        public void PositionedTryGetEntriesByPartialKeyTest()
        {
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

        private Class1<T> GetClass1<T>(T any)
        {
            return new Class1<T>(any);
        }

        private ValueTuple<T> GetValueTuple<T>(T any)
        {
            return new ValueTuple<T>(any);
        }

        public class Class1<T> : Tuple<T>
        {

            public Class1(T name) : base(name)
            {
                Name = name;
            }

            public T Name { get; private set; }
        }
    }
}
