using System;
using System.Linq;
using GitHub.Protobufel.MultiKeyMap;
using static GitHub.Protobufel.MultiKeyMap.MultiKeyMaps.MultiKeyCreationStrategy;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Loggers;
using System.Collections.Generic;
using static MultiKeyMapBenchmarks.BenchMarkHelpers;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Validators;
using BenchmarkDotNet.Attributes.Jobs;

namespace MultiKeyMapBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            //var summary1 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryCreation>();
            //var summary2 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryAddition>();
            //var summary3 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryRemoval>();
            //var summary4 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryTryGetValue>();
            // var summary5 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryTryGetsByPartialKeyStrings1000000rows100searches>();
            //var summary6 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryTryGetsByPartialKeyStrings1000rows1search>();
            var summary7 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryTryGetsByPartialKeyEmployees1000rows1search>();
            //var summary0 = BenchmarkRunner.Run<BenchmarkPlayground1>();
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
    public class MultiKeyMapVsDictionaryTryGetsByPartialKeyStrings1000000rows100searches
    {
        private IMultiKeyMap<string, IEnumerable<string>, string> map;
        private IDictionary<IEnumerable<string>, string> dict;

        private IList<IEnumerable<string>> keys;
        private IList<IEnumerable<string>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;

        private const int SearchCount = 100;
        private const int RecordCount = 1_000_000;
        private const int KeySize = 10;


        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Setup]
        public void Setup()
        {
            int count = SearchCount;
            Init();
            keys = dict.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(map.Keys, count);
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>(Strategy);
            dict = new Dictionary<IEnumerable<string>, string>();

            PopulateDictionary(map, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
            PopulateDictionary(dict, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
        }

        [Benchmark]
        public IEnumerable<IEnumerable<IEnumerable<string>>> MultiKeyMap_TryGetFullKeysByPartialKey()
        {
            var result = new List<IEnumerable<IEnumerable<string>>>(partKeys.Count);

            foreach (var partKey in partKeys)
            {
                if (map.TryGetFullKeysByPartialKey(partKey, out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<IEnumerable<string>>> MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            var result = new List<IEnumerable<IEnumerable<string>>>(partKeys.Count);

            foreach (var partKey in partKeys)
            {
                if (map.TryGetFullKeysByPartialKey(partKey, positions, out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<IEnumerable<string>>> MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey()
        {
            var result = new List<IEnumerable<IEnumerable<string>>>(partKeys.Count);

            foreach (var partKey in partKeys)
            {
                if (map.TryGetFullKeysByPartialKey(partKey, positivePositions, out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }

        [Benchmark(Baseline = true)]
        public IEnumerable<string> Dictionary_TryGetValue()
        {
            var result = new List<string>(partKeys.Count);

            foreach (var key in keys)
            {
                if (dict.TryGetValue(key, out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryTryGetsByPartialKeyStrings1000rows1searchOld
    {
        private IMultiKeyMap<string, IEnumerable<string>, string> nonPosMap;
        private IMultiKeyMap<string, IEnumerable<string>, string> posMap;
        private IDictionary<IEnumerable<string>, string> dict;

        private IList<IEnumerable<string>> keys;
        private IList<IEnumerable<string>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;

        private const int SearchCount = 1;
        private const int RecordCount = 1_000;
        private const int KeySize = 10;


        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        public IMultiKeyMap<string, IEnumerable<string>, string> ActiveMap =>
            (Strategy == OptimizedForPositionalSearch ? posMap : nonPosMap);

        [Setup]
        public void Setup()
        {
            int count = SearchCount;
            Init();
            keys = dict.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(nonPosMap.Keys, count);
        }

        public void Init()
        {
            nonPosMap = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>(OptimizedForNonPositionalSearch);
            posMap = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>(OptimizedForPositionalSearch);
            dict = new Dictionary<IEnumerable<string>, string>();

            PopulateDictionary(nonPosMap, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
            PopulateDictionary(posMap, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
            PopulateDictionary(dict, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_TryGetFullKeysByPartialKey()
        {
            ActiveMap.TryGetFullKeysByPartialKey(partKeys.First(), out var value);
            return value;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            ActiveMap.TryGetFullKeysByPartialKey(partKeys.First(), positions, out var value);
            return value;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey()
        {
            ActiveMap.TryGetFullKeysByPartialKey(partKeys.First(), positivePositions, out var value);
            return value;
        }

        [Benchmark(Baseline = true)]
        public IEnumerable<string> Dictionary_TryGetValue()
        {
            var result = new List<string>(partKeys.Count);

            foreach (var key in keys.Take(KeySize))
            {
                if (dict.TryGetValue(key, out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }

    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryTryGetsByPartialKeyStrings1000rows1search
    {
        private IMultiKeyMap<string, IEnumerable<string>, string> map;
        private IDictionary<IEnumerable<string>, string> dict;

        private IList<IEnumerable<string>> keys;
        private IList<IEnumerable<string>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;

        private const int SearchCount = 1;
        private const int RecordCount = 1_000;
        private const int KeySize = 10;


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
            keys = dict.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(dict.Keys, count);
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>(SubKeyEqualityComparer, KeyEqualityComparer, Strategy);
            dict = new Dictionary<IEnumerable<string>, string>(KeyEqualityComparer);

            PopulateDictionary(map, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
            PopulateDictionary(dict, RecordCount, KeySize, x => String.Join("", Enumerable.Range(x, 10)), x => x.ToString());
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_TryGetFullKeysByPartialKey()
        {
            map.TryGetFullKeysByPartialKey(partKeys.First(), out var value);
            return value;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            map.TryGetFullKeysByPartialKey(partKeys.First(), positions, out var value);
            return value;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey()
        {
            map.TryGetFullKeysByPartialKey(partKeys.First(), positivePositions, out var value);
            return value;
        }

        [Benchmark(Baseline = true)]
        public IEnumerable<string> Dictionary_TryGetValue()
        {
            var result = new List<string>(partKeys.Count);

            foreach (var key in keys.Take(KeySize))
            {
                if (dict.TryGetValue(key, out var value))
                {
                    result.Add(value);
                }
            }

            return result;
        }
    }

    [ShortRunJob]
    [MemoryDiagnoser]
    public class MultiKeyMapVsDictionaryTryGetsByPartialKeyEmployees1000rows1search
    {
        private IMultiKeyMap<Employee<int>, IEnumerable<Employee<int>>, string> map;
        private IDictionary<IEnumerable<Employee<int>>, string> dict;

        private IList<IEnumerable<Employee<int>>> keys;
        private IList<IEnumerable<Employee<int>>> partKeys;
        private IList<int> positions;
        private IList<int> positivePositions;

        private const int SearchCount = 1;
        private const int RecordCount = 1_000;
        private const int KeySize = 10;

        public ILogger Logger { get; set; }

        public MultiKeyMapVsDictionaryTryGetsByPartialKeyEmployees1000rows1search()
        {
            Logger = new AccumulationLogger();
        }

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
            keys = dict.Keys.Where((x, index) => index % 3 == 0).Take(count).ToList();
            (partKeys, positivePositions, positions) = PopulatePartialKeys(dict.Keys, count);
            //Logger.WriteLine($"map count = {map.Count}");
            //Logger.WriteLine($"dict count = {dict.Count}");
            //Logger.WriteLine($"partKeys count = {partKeys.Count}");

            //if (partKeys.Count == 0)
            //{
            //    Logger.WriteError("partKeys.Count == 0!");
            //}
            //else
            //{
            //    Logger.WriteLine($"partKey = {String.Join(",", partKeys.First())}");
            //}
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<Employee<int>, IEnumerable<Employee<int>>, string>(SubKeyEqualityComparer, KeyEqualityComparer, Strategy);
            dict = new Dictionary<IEnumerable<Employee<int>>, string>(KeyEqualityComparer);

            PopulateDictionary(map, RecordCount, KeySize, x => new Employee<int>(String.Join("", Enumerable.Range(x, 10)), x), x => x.ToString());
            PopulateDictionary(dict, RecordCount, KeySize, x => new Employee<int>(String.Join("", Enumerable.Range(x, 10)), x), x => x.ToString());
        }

        [Benchmark]
        public IEnumerable<IEnumerable<Employee<int>>> MultiKeyMap_TryGetFullKeysByPartialKey()
        {
            //Logger.WriteLine($"map count = {map.Count}");

            if (map.TryGetFullKeysByPartialKey(partKeys.First(), out var value))
            {
                //Logger.WriteLine($"partKey = {String.Join(",", partKeys.First())}");
                //Logger.WriteLine($"value = {String.Join(",", value)}");
            }
            else
            {
                //Logger.WriteLine($"result of TryGetFullKeysByPartialKey is false!");
            }

            return value;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<Employee<int>>> MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey()
        {
            map.TryGetFullKeysByPartialKey(partKeys.First(), positions, out var value);
            return value;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<Employee<int>>> MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey()
        {
            map.TryGetFullKeysByPartialKey(partKeys.First(), positivePositions, out var value);
            return value;
        }

        [Benchmark(Baseline = true)]
        public string Dictionary_TryGetValue()
        {
            dict.TryGetValue(keys.First(), out var value);
            return value;
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