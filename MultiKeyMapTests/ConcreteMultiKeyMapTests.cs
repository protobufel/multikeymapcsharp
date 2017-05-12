using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitHub.Protobufel.MultiKeyMap;
//using System.Linq;
using System.Collections;
using System.Collections.Generic;
using FluentAssertions;

namespace MultiKeyMapTests
{
    [TestClass]
    public class ConcreteMultiKeyMapTests
    {
        private MultiKeyMapBaseHelper<int, int[], object> helper1;
        private MultiKeyMapBaseHelper<string, string[], long> helper2;
        private MultiKeyMapBaseHelper<Class1<string>, List<Class1<string>>, string> helper3;
        private MultiKeyMapBaseHelper<Struct1<double>, List<Struct1<double>>, bool> helper4;

        //[ClassInitialize]
        public ConcreteMultiKeyMapTests()
        {
            helper1 = InitHelper<int, int[], object>(new[] { 1, 2, 3, 4 }, new[] { 2, 5, 6, 3 }, new { A = 1, B = "hi" });
            helper2 = InitHelper<string, string[], long>(new[] { "1", "2", "6", "hi" }, new[] { "hi", "a", "c", "2" }, 99);
            helper3 = InitHelper<Class1<string>, List<Class1<string>>, string>(
                new List<Class1<string>>() { GetClass1("a"), GetClass1("b"), GetClass1("M")},
                new List<Class1<string>>() { GetClass1("C"), GetClass1("a"), GetClass1("M"), GetClass1("Y") }, 
                "first", "second");
            helper4 = InitHelper<Struct1<double>, List<Struct1<double>>, bool>(
                new List<Struct1<double>>() { GetStruct1(1.1), GetStruct1(2.2), GetStruct1(3.3) },
                new List<Struct1<double>>() { GetStruct1(3.3), GetStruct1(4.0), GetStruct1(5.5), GetStruct1(2.2) },
                true, false);
        }

        [TestInitialize]
        public void Init()
        {
            helper1.Init();
            helper2.Init();
            helper3.Init();
            helper4.Init();
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
        public void GetFullKeysByPartialKeyTest()
        {
            helper1.AssertGetFullKeysByPartialKey(new List<int> ()  { 3, 2 });
            helper2.AssertGetFullKeysByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertGetFullKeysByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertGetFullKeysByPartialKey(new[] { GetStruct1(3.3), GetStruct1(2.2) });
        }


        [TestMethod]
        public void GetValuesByPartialKeyTest()
        {
            helper1.AssertGetValuesByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertGetValuesByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertGetValuesByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertGetValuesByPartialKey(new[] { GetStruct1(3.3), GetStruct1(2.2) });
        }

        [TestMethod]
        public void GetEntriesByPartialKeyTest()
        {
            helper1.AssertGetEntriesByPartialKey(new List<int>() { 3, 2 });
            helper2.AssertGetEntriesByPartialKey(new List<string>() { "6", "2" });
            helper3.AssertGetEntriesByPartialKey(new[] { GetClass1("M"), GetClass1("b") });
            helper4.AssertGetEntriesByPartialKey(new[] { GetStruct1(3.3), GetStruct1(2.2) });
        }

        private Class1<T> GetClass1<T>(T any)
        {
            return new Class1<T>(any);
        }

        private Struct1<T> GetStruct1<T>(T any)
        {
            return new Struct1<T>(any);
        }

        public class Class1<T>
        {
            public Class1()
            {
                Name = default(T);
            }

            public Class1(T name)
            {
                Name = name;
            }

            public T Name { get; private set; }
        }

        public struct Struct1<T>
        {
            public Struct1(T name)
            {
                Name = name;
            }

            public T Name { get; private set; }
        }
    }
}
