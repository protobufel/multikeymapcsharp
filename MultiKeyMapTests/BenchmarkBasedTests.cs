using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GitHub.Protobufel.MultiKeyMap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static GitHub.Protobufel.MultiKeyMap.MultiKeyMaps.MultiKeyCreationStrategy;
using static MultiKeyMapTests.CommonHelpers;
using static MultiKeyMapTests.TestHelpers;

namespace MultiKeyMapTests
{
    [TestClass]
    public class BenchmarkBasedTests
    {
        private IMultiKeyMap<Employee<int>, IEnumerable<Employee<int>>, string> map;
        private IDictionary<IEnumerable<Employee<int>>, string> dict;

        private IEnumerable<IEnumerable<Employee<int>>> searchKeys;
        private IEnumerable<IEnumerable<Employee<int>>> partKeys;
        private IEnumerable<int> positions;
        private IEnumerable<int> positivePositions;

        private const int SearchCount = 2;
        private const int RecordCount = 30;
        private const int KeySize = 10;

        public void Init(MultiKeyMaps.MultiKeyCreationStrategy strategy, bool subKeyEqualityByRef, bool keyEqualityByRef)
        {
            (map, dict, searchKeys, partKeys, positivePositions, positions) = Setup(strategy, subKeyEqualityByRef, keyEqualityByRef);
        }

        public (
                IMultiKeyMap<Employee<int>, IEnumerable<Employee<int>>, string> map,
                IDictionary<IEnumerable<Employee<int>>, string> dict,
                IEnumerable<IEnumerable<Employee<int>>> keys,
                IList<IEnumerable<Employee<int>>> partKeys,
                IEnumerable<int> positivePositions,
                IEnumerable<int> positions)
            Setup(MultiKeyMaps.MultiKeyCreationStrategy strategy, bool subKeyEqualityByRef, bool keyEqualityByRef)
        {
            var subKeyComparer = subKeyEqualityByRef.SubKeyComparerFor<Employee<int>>();
            var keyComparer = keyEqualityByRef.KeyComparerFor<Employee<int>>();

            Func<int, Employee<int>> subKeyConverter = x => new Employee<int>(String.Join("", Enumerable.Range(x, 10)), x);
            Func<int, string> valueConverter = x => x.ToString();

            var tuple = InitData(subKeyComparer, keyComparer, strategy, subKeyConverter, valueConverter, SearchCount, RecordCount, KeySize);

            var map = MultiKeyMaps.CreateMultiKeyDictionary<Employee<int>, IEnumerable<Employee<int>>, string>(subKeyComparer, keyComparer, strategy);
            map.CopyFrom(tuple.dict);
            return (map, tuple.dict, tuple.keys, tuple.partKeys, tuple.positivePositions, tuple.positions);
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, false, false)]
        [DataRow(OptimizedForPositionalSearch, false, true)]
        [DataRow(OptimizedForPositionalSearch, true, false)]
        [DataRow(OptimizedForPositionalSearch, true, true)]
        [DataRow(OptimizedForNonPositionalSearch, false, false)]
        [DataRow(OptimizedForNonPositionalSearch, false, true)]
        [DataRow(OptimizedForNonPositionalSearch, true, false)]
        [DataRow(OptimizedForNonPositionalSearch, true, true)]
        public void TryGetFullKeysByPartialKeyTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, bool subKeyEqualityByRef, bool keyEqualityByRef)
        {
            Init(strategy, subKeyEqualityByRef, keyEqualityByRef);
            var subKeyComparer = subKeyEqualityByRef.SubKeyComparerFor<Employee<int>>();
            var keyComparer = keyEqualityByRef.KeyComparerFor<Employee<int>>();

            foreach (var key in searchKeys)
            {
                bool result = map.TryGetFullKeysByPartialKey(key, out var value);
                result.Should().Be(true);
                value.Should().NotBeNullOrEmpty().And.HaveCount(1).And.HaveElementAt(0, key);
            }

            foreach (var partKey in partKeys)
            {
                bool result = map.TryGetFullKeysByPartialKey(partKey, out var value);
                result.Should().Be(true);
                value.Should().NotBeNullOrEmpty()
                    .And.HaveCount(x => x > 0)
                    .And.NotContainNulls()
                    .And.OnlyHaveUniqueItems()
                    .And.OnlyContain(key => map.Keys.Contains(key, keyComparer))
                    .And.OnlyContain(key => partKey.Intersect(key, subKeyComparer).Count() == partKey.Count());
            }
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, false, false)]
        [DataRow(OptimizedForPositionalSearch, false, true)]
        [DataRow(OptimizedForPositionalSearch, true, false)]
        [DataRow(OptimizedForPositionalSearch, true, true)]
        [DataRow(OptimizedForNonPositionalSearch, false, false)]
        [DataRow(OptimizedForNonPositionalSearch, false, true)]
        [DataRow(OptimizedForNonPositionalSearch, true, false)]
        [DataRow(OptimizedForNonPositionalSearch, true, true)]
        public void TryGetFullKeysByPartialKeyMixedPositionsTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, bool subKeyEqualityByRef, bool keyEqualityByRef)
        {
            Init(strategy, subKeyEqualityByRef, keyEqualityByRef);
            var subKeyComparer = subKeyEqualityByRef.SubKeyComparerFor<Employee<int>>();
            var keyComparer = keyEqualityByRef.KeyComparerFor<Employee<int>>();

            foreach (var partKey in partKeys)
            {
                bool result = map.TryGetFullKeysByPartialKey(partKey, positions, out var value);
                result.Should().Be(true);
                value.Should().NotBeNullOrEmpty()
                    .And.HaveCount(x => x > 0)
                    .And.NotContainNulls()
                    .And.OnlyHaveUniqueItems()
                    .And.OnlyContain(key => map.Keys.Contains(key, keyComparer))
                    .And.OnlyContain(key => partKey.Intersect(key, subKeyComparer).Count() == partKey.Count());
            }
        }

        [DataTestMethod]
        [DataRow(OptimizedForPositionalSearch, false, false)]
        [DataRow(OptimizedForPositionalSearch, false, true)]
        [DataRow(OptimizedForPositionalSearch, true, false)]
        [DataRow(OptimizedForPositionalSearch, true, true)]
        [DataRow(OptimizedForNonPositionalSearch, false, false)]
        [DataRow(OptimizedForNonPositionalSearch, false, true)]
        [DataRow(OptimizedForNonPositionalSearch, true, false)]
        [DataRow(OptimizedForNonPositionalSearch, true, true)]
        public void TryGetFullKeysByPartialKeyPositivePositionsTest(MultiKeyMaps.MultiKeyCreationStrategy strategy, bool subKeyEqualityByRef, bool keyEqualityByRef)
        {
            Init(strategy, subKeyEqualityByRef, keyEqualityByRef);
            var subKeyComparer = subKeyEqualityByRef.SubKeyComparerFor<Employee<int>>();
            var keyComparer = keyEqualityByRef.KeyComparerFor<Employee<int>>();

            foreach (var partKey in partKeys)
            {
                bool result = map.TryGetFullKeysByPartialKey(partKey, positivePositions, out var value);
                result.Should().Be(true);
                value.Should().NotBeNullOrEmpty()
                    .And.HaveCount(x => x > 0)
                    .And.NotContainNulls()
                    .And.OnlyHaveUniqueItems()
                    .And.OnlyContain(key => map.Keys.Contains(key, keyComparer))
                    .And.OnlyContain(key => partKey.Intersect(key, subKeyComparer).Count() == partKey.Count());
            }
        }
    }

    public class Employee<T> : Tuple<string, T>
    {

        public Employee(string name, T department) : base(name, department)
        {
            Name = name;
            Department = department;
        }

        public string Name { get; private set; }
        public T Department { get; private set; }

        public override string ToString()
        {
            return $"Employee(name={Name}, department={Department})";
        }
    }
}
