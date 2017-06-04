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
using System.Linq;
using GitHub.Protobufel.MultiKeyMap;
using static GitHub.Protobufel.MultiKeyMap.MultiKeyMaps.MultiKeyCreationStrategy;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;
using static MultiKeyMapBenchmarks.BenchMarkHelpers;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Loggers;

namespace MultiKeyMapBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {

            var switcher = new BenchmarkSwitcher(new[] 
            {
                typeof(MultiKeyMapVsDictionaryCreation),
                typeof(MultiKeyMapVsDictionaryAddition),
                typeof(MultiKeyMapVsDictionaryRemoval),
                typeof(MultiKeyMapVsDictionaryTryGetValue),
                typeof(MultiKeyMapTryGetsByPartialKeyStrings),
                typeof(MultiKeyMapTryGetsByPartialKeyEmployees),
                typeof(MultiKeyMapTryGetsByPartialKeyEmployeesBaseline)
            });

            switcher.Run(args);
        }
    }

    public static class BenchMarkHelpers
    {
        public static void PopulateDictionary<T, V>(IDictionary<IEnumerable<T>, V> map, int count, int keySize,
            Func<int, T> subKeyConverter, Func<int, V> valueConverter)
        {
            for (int i = 0; i < count; i++)
            {
                map.Add(Enumerable.Range(i, keySize).Select(x => subKeyConverter.Invoke(x)).ToList(),
                    valueConverter.Invoke(i));
            }
        }

        public static (IList<IEnumerable<T>> partKeys, IList<int> positivePositions, IList<int> mixedPositions) PopulatePartialKeys<T>(
            IEnumerable<IEnumerable<T>> keys, int count, int partKeySizePercent = 75, int nthKey = 7)
        {
            var random = new Random();
            var positions = new HashSet<int>();
            int keySize = keys.First().Count();
            int partKeySize = keySize * partKeySizePercent / 100;

            var forceCount = Enumerable.Range(0, int.MaxValue)
                .TakeWhile(x =>
                {
                    positions.Add(random.Next(keySize));
                    return positions.Count < partKeySize;
                })
                .Count();

            var positivePositions = positions.OrderBy(x => x).ToList();
            var mixedPositions = positivePositions.Select((x, index) => (index % 2 == 0) ? -1 : x).ToList();

            IList<IEnumerable<T>> partKeys = keys.Where((x, index) => index % nthKey == 0)
                .Take(count)
                .Select(key => key.Where((subKey, pos) => positions.Contains(pos)).ToList().AsEnumerable())
                .ToList();

            return (partKeys as IList<IEnumerable<T>>, positivePositions, mixedPositions);
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryCreation
    {
        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Benchmark]
        public IMultiKeyMap<string, string[], int> CreateDictionaryMultiKeyMap()
        {
            return MultiKeyMaps.CreateMultiKeyDictionary<string, string[], int>(Strategy);
        }

        [Benchmark(Baseline = true)]
        public Dictionary<string[], int> CreateDictionary()
        {
            return new Dictionary<string[], int>();
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryAddition
    {
        private IMultiKeyMap<int, IEnumerable<int>, string> map;
        private IDictionary<IEnumerable<int>, string> dict;

        private IList<IEnumerable<int>> keys;
        private IList<string> values;


        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Setup]
        public void Setup()
        {
            int count = 100;
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(
                x => new List<int>() { x, x + 1, x + 2, x + 3, x + 4, x + 5, x + 6, x + 7, x + 8, x + 9 }).ToList();
            values = Enumerable.Range(1, count).Select<int, string>(x => String.Format("{0}", x)).ToList();
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<int, IEnumerable<int>, string>(Strategy);
            dict = new Dictionary<IEnumerable<int>, string>();
        }

        [Benchmark]
        public IMultiKeyMap<int, IEnumerable<int>, string> MultiKeyMap_Add()
        {
            Init();

            for (int i = 0; i < keys.Count; i++)
            {
                map.Add(keys[i], values[i]);
            }

            return map;
        }

        [Benchmark(Baseline = true)]
        public IDictionary<IEnumerable<int>, string> Dictionary_Add()
        {
            Init();

            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], values[i]);
            }

            return dict;
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryRemoval
    {
        private IMultiKeyMap<int, IEnumerable<int>, string> map;
        private IDictionary<IEnumerable<int>, string> dict;

        private IList<IEnumerable<int>> keys;
        private IList<string> values;


        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Setup]
        public void Setup()
        {
            int count = 100;
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(
                x => new List<int>() { x, x + 1, x + 2, x + 3, x + 4, x + 5, x + 6, x + 7, x + 8, x + 9 }).ToList();
            values = Enumerable.Range(1, count).Select(x => String.Format("{0}", x)).ToList();
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<int, IEnumerable<int>, string>(Strategy);
            dict = new Dictionary<IEnumerable<int>, string>();

            MultiKeyMap_AddLoop(map);
            Dictionary_AddLoop(dict);
        }

        public IMultiKeyMap<int, IEnumerable<int>, string> MultiKeyMap_AddLoop(IMultiKeyMap<int, IEnumerable<int>, string> map)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                map.Add(keys[i], values[i]);
            }

            return map;
        }

        public IDictionary<IEnumerable<int>, string> Dictionary_AddLoop(IDictionary<IEnumerable<int>, string> dict)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], values[i]);
            }

            return dict;
        }

        [Benchmark]
        public IMultiKeyMap<int, IEnumerable<int>, string> MultiKeyMap_Remove()
        {
            Init();

            for (int i = 0; i < keys.Count; i++)
            {
                map.Remove(keys[i]);
            }

            return map;
        }

        [Benchmark(Baseline = true)]
        public IDictionary<IEnumerable<int>, string> Dictionary_Remove()
        {
            Init();

            for (int i = 0; i < keys.Count; i++)
            {
                dict.Remove(keys[i]);
            }

            return dict;
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryTryGetValue
    {
        private IMultiKeyMap<int, IEnumerable<int>, string> map;
        private IDictionary<IEnumerable<int>, string> dict;

        private IList<IEnumerable<int>> keys;
        private IList<string> values;


        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Setup]
        public void Setup()
        {
            int count = 100;
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(
                x => new List<int>() { x, x + 1, x + 2, x + 3, x + 4, x + 5, x + 6, x + 7, x + 8, x + 9 }).ToList();
            values = Enumerable.Range(1, count).Select<int, string>(x => String.Format("{0}", x)).ToList();

            Init();
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<int, IEnumerable<int>, string>(Strategy);
            dict = new Dictionary<IEnumerable<int>, string>();

            MultiKeyMap_AddLoop(map);
            Dictionary_AddLoop(dict);
        }

        public IMultiKeyMap<int, IEnumerable<int>, string> MultiKeyMap_AddLoop(IMultiKeyMap<int, IEnumerable<int>, string> map)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                map.Add(keys[i], values[i]);
            }

            return map;
        }

        public IDictionary<IEnumerable<int>, string> Dictionary_AddLoop(IDictionary<IEnumerable<int>, string> dict)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                dict.Add(keys[i], values[i]);
            }

            return dict;
        }

        [Benchmark]
        public IEnumerable<string> MultiKeyMap_TryGetValue()
        {
            var result = new List<string>(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                if (map.TryGetValue(keys[i], out var value))
                {
                    result.Add(value);
                }

            }

            return result;
        }

        [Benchmark(Baseline = true)]
        public IEnumerable<string> Dictionary_TryGetValue()
        {
            var result = new List<string>(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                if (map.TryGetValue(keys[i], out var value))
                {
                    result.Add(value);
                }

            }

            return result;
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapTryGetsByPartialKeyStrings
    {
        private IMultiKeyMap<string, IEnumerable<string>, string> map;
        private IList<IEnumerable<string>> keys;
        private IList<IEnumerable<string>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;


        public ILogger Logger { get; set; }

        public MultiKeyMapTryGetsByPartialKeyStrings()
        {
            Logger = new ConsoleLogger();
        }

        [Params(1_000_000)]
        public int RecordCount { get; set; }

        [Params(5)]
        public int SearchCount { get; set; }

        [Params(10)]
        public int KeySize { get; set; }

        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Params(true, false)]
        public bool SubKeyEqualityByRef { get; set; }

        [Params(true, false)]
        public bool KeyEqualityByRef { get; set; }


        public IEqualityComparer<string> SubKeyEqualityComparer => EqualityComparerExtensions.ReferenceEqualityComparerOf<string>();
        public IEqualityComparer<IEnumerable<string>> KeyEqualityComparer => KeyEqualityByRef
            ? EqualityComparerExtensions.ReferenceEqualityComparerOf<IEnumerable<string>>()
            : EqualityComparer<string>.Default.EnumerableEqualityComparerOf<string, IEnumerable<string>>();

        [Setup]
        public void Setup()
        {
            int count = SearchCount;
            Init();
            keys = map.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(map.Keys, count);
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>(SubKeyEqualityComparer, KeyEqualityComparer, Strategy);
            PopulateDictionary(map, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
        }


        [Benchmark]
        public bool TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, out var value)) return false;

            }

            return true;
        }

        [Benchmark]
        public bool Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, positions, out var value)) return false;

            }

            return true;
        }

        [Benchmark]
        public bool Only_Positional_TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, positivePositions, out var value)) return false;

            }

            return true;
        }

        [Benchmark(Baseline = true)]
        public bool TryGetValue()
        {
            foreach (var key in keys)
            {
                if (!map.TryGetValue(key, out var value)) return false;

            }

            return true;
        }
    }

    [ShortRunJob]
    [MemoryDiagnoser]
    public class MultiKeyMapTryGetsByPartialKeyEmployees
    {
        private IMultiKeyMap<Employee<int>, IEnumerable<Employee<int>>, string> map;
        private IList<IEnumerable<Employee<int>>> keys;
        private IList<IEnumerable<Employee<int>>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;

        public ILogger Logger { get; set; }

        public MultiKeyMapTryGetsByPartialKeyEmployees()
        {
            Logger = new ConsoleLogger();
        }

        [Params(1_000_000)]
        public int RecordCount { get; set; }

        [Params(5)]
        public int SearchCount { get; set; }

        [Params(10)]
        public int KeySize { get; set; }

        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Params(true, false)]
        public bool SubKeyEqualityByRef { get; set; }

        [Params(true, false)]
        public bool KeyEqualityByRef { get; set; }

        public IEqualityComparer<Employee<int>> SubKeyEqualityComparer => SubKeyEqualityByRef
            ? EqualityComparerExtensions.ReferenceEqualityComparerOf<Employee<int>>()
            : EqualityComparer<Employee<int>>.Default;
        public IEqualityComparer<IEnumerable<Employee<int>>> KeyEqualityComparer => KeyEqualityByRef
            ? EqualityComparerExtensions.ReferenceEqualityComparerOf<IEnumerable<Employee<int>>>()
            : EqualityComparer<Employee<int>>.Default.EnumerableEqualityComparerOf<Employee<int>, IEnumerable<Employee<int>>>();

        [Setup]
        public void Setup()
        {
            int count = SearchCount;
            Init();
            keys = map.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(map.Keys, count);
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<Employee<int>, IEnumerable<Employee<int>>, string>(SubKeyEqualityComparer, KeyEqualityComparer, Strategy);
            PopulateDictionary(map, RecordCount, KeySize, x => new Employee<int>(String.Join("", Enumerable.Range(x, 10)), x), x => x.ToString());
        }

        [Benchmark]
        public bool TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, out var value)) return false;

            }

            return true;
        }

        [Benchmark]
        public bool Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, positions, out var value)) return false;

            }

            return true;
        }

        [Benchmark]
        public bool Only_Positional_TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, positivePositions, out var value)) return false;

            }

            return true;
        }

        [Benchmark(Baseline = true)]
        public bool TryGetValue()
        {
            foreach (var key in keys)
            {
                if (!map.TryGetValue(key, out var value)) return false;

            }

            return true;
        }
    }

    [ShortRunJob]
    [MemoryDiagnoser]
    public class MultiKeyMapTryGetsByPartialKeyEmployeesBaseline
    {
        private IMultiKeyMap<Employee<int>, IEnumerable<Employee<int>>, string> map;
        private IList<IEnumerable<Employee<int>>> keys;
        private IList<IEnumerable<Employee<int>>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;

        public ILogger Logger { get; set; }

        public MultiKeyMapTryGetsByPartialKeyEmployeesBaseline()
        {
            Logger = new ConsoleLogger();
        }

        [Params(1_000)]
        public int RecordCount { get; set; }

        [Params(1)]
        public int SearchCount { get; set; }

        [Params(10)]
        public int KeySize { get; set; }

        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Params(false)]
        public bool SubKeyEqualityByRef { get; set; }

        [Params(false)]
        public bool KeyEqualityByRef { get; set; }

        public IEqualityComparer<Employee<int>> SubKeyEqualityComparer => SubKeyEqualityByRef
            ? EqualityComparerExtensions.ReferenceEqualityComparerOf<Employee<int>>()
            : EqualityComparer<Employee<int>>.Default;
        public IEqualityComparer<IEnumerable<Employee<int>>> KeyEqualityComparer => KeyEqualityByRef
            ? EqualityComparerExtensions.ReferenceEqualityComparerOf<IEnumerable<Employee<int>>>()
            : EqualityComparer<Employee<int>>.Default.EnumerableEqualityComparerOf<Employee<int>, IEnumerable<Employee<int>>>();

        [Setup]
        public void Setup()
        {
            int count = SearchCount;
            Init();
            keys = map.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(map.Keys, count);
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<Employee<int>, IEnumerable<Employee<int>>, string>(SubKeyEqualityComparer, KeyEqualityComparer, Strategy);
            PopulateDictionary(map, RecordCount, KeySize, x => new Employee<int>(String.Join("", Enumerable.Range(x, 10)), x), x => x.ToString());
        }

        [Benchmark(Baseline = true)]
        public bool TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, out var value)) return false;

            }

            return true;
        }

        [Benchmark]
        public bool Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, positions, out var value)) return false;

            }

            return true;
        }

        [Benchmark]
        public bool Only_Positional_TryGetFullKeysByPartialKey()
        {
            foreach (var partKey in partKeys)
            {
                if (!map.TryGetFullKeysByPartialKey(partKey, positivePositions, out var value)) return false;

            }

            return true;
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