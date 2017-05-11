using Microsoft.VisualStudio.TestTools.UnitTesting;
using GitHub.Protobufel.MultiKeyMap;
//using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
//using FluentAssertions.Types;
//using FluentAssertions.Collections;

namespace MultiKeyMapTests
{
    [TestClass]
    public class MultiKeyMapsTests
    {
 
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
         public void CreateMultiKeyDictionary_CreatesEmptyInstance()
        {
            CreatesEmptyInstanceHelper(1, new []{ 1,2,3}, new { A = 1, B = "hi"});
            CreatesEmptyInstanceHelper("1", new[] { "1", "2"}, 99);
            CreatesEmptyInstanceHelper(GetClass1("hi"), new List<Class1<string>>(){ GetClass1("a"), GetClass1("b") }, true);
            CreatesEmptyInstanceHelper(GetClass1(3.5), new HashSet<Class1<double>>() { GetClass1(1.1), GetClass1(2.2) }, true);
            CreatesEmptyInstanceHelper(GetStruct1(3.5), new HashSet<Struct1<double>>() { GetStruct1(1.1), GetStruct1(2.2) }, true);
        }

        private IMultiKeyMap<T,K,V> CreatesEmptyInstanceHelper<T,K,V>(T t, K k, V v) where K : IEnumerable<T>
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
            AddHelper(GetStruct1(3.5), new HashSet<Struct1<double>>() { GetStruct1(1.1), GetStruct1(2.2) }, true);
        }

        private IMultiKeyMap<T, K, V> AddHelper<T, K, V>(T t, K k, V v) where K : IEnumerable<T>
        {
            KeyValuePair<K, V> expectedEntry = new KeyValuePair<K, V>(k, v);

            var multiDict = MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>();
            multiDict.Should().NotBeNull().And.BeEmpty().And.BeAssignableTo(typeof(IMultiKeyMap<T, K, V>)).And.BeAssignableTo(typeof(IDictionary<K, V>));

            multiDict.Add(k, v);
            multiDict.Should().NotBeEmpty().And.ContainKey(k).And.ContainValue(v).And.Contain(expectedEntry).And.HaveCount(1);

            var actualKeys = multiDict.GetFullKeysByPartialKey(k);
            actualKeys.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0,k);

            var actualValues = multiDict.GetValuesByPartialKey(k);
            actualValues.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, v);

            var actualEntries = multiDict.GetEntriesByPartialKey(k);
            actualEntries.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, expectedEntry);

            return multiDict;
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
