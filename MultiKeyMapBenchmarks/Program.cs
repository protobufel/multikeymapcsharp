using System;
using System.Linq;
using GitHub.Protobufel.MultiKeyMap;
using static GitHub.Protobufel.MultiKeyMap.MultiKeyMaps.MultiKeyCreationStrategy;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;

namespace MultiKeyMapBenchmarks
{
    public class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MultiKeyMapVsDictionaryCreation>();
            var summary2 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryAddition>();
            var summary3 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryRemoval>();
            var summary4 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryTryGetValue>();
            var summary5 = BenchmarkRunner.Run<MultiKeyMapVsDictionaryTryGetsByPartialKey>();
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
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(x => new List<int>() { x, x + 1, x + 2 }).ToList();
            values = Enumerable.Range(1, count).Select<int, string>(x => String.Format("{0}", x)).ToList();
        }

        public void Init()
        {
            map = MultiKeyMaps.CreateMultiKeyDictionary<int, IEnumerable<int>, string>(Strategy);
            dict = new Dictionary<IEnumerable<int>, string>();
        }

        [Benchmark]
        public IMultiKeyMap<int, IEnumerable<int>, string> MultiKeyMap_AddLoop()
        {
            Init();

            for (int i = 0; i < keys.Count; i++)
            {
                map.Add(keys[i], values[i]);
            }

            return map;
        }

        [Benchmark(Baseline = true)]
        public IDictionary<IEnumerable<int>, string> Dictionary_AddLoop()
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
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(x => new List<int>() { x, x + 1, x + 2 }).ToList();
            values = Enumerable.Range(1, count).Select<int, string>(x => String.Format("{0}", x)).ToList();
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
        public IMultiKeyMap<int, IEnumerable<int>, string> MultiKeyMap_RemoveLoop()
        {
            Init();

            for (int i = 0; i < keys.Count; i++)
            {
                map.Remove(keys[i]);
            }

            return map;
        }

        [Benchmark(Baseline = true)]
        public IDictionary<IEnumerable<int>, string> Dictionary_RemoveLoop()
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
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(x => new List<int>() { x, x + 1, x + 2 }).ToList();
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
        public IEnumerable<string> MultiKeyMap_TryGetValueLoop()
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
        public IEnumerable<string> Dictionary_TryGetValueLoop()
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
    public class MultiKeyMapVsDictionaryTryGetsByPartialKey
    {
        private IMultiKeyMap<int, IEnumerable<int>, string> map;
        private IDictionary<IEnumerable<int>, string> dict;

        private IList<IEnumerable<int>> keys;
        private IList<string> values;

        private IList<IEnumerable<int>> subKeys;
        private IEnumerable<int> positions;

        private const int SearchCount = 100;
        private const int RecordCount = 1_000_000;

        [Params(OptimizedForNonPositionalSearch, OptimizedForPositionalSearch)]
        public MultiKeyMaps.MultiKeyCreationStrategy Strategy { get; set; }

        [Setup]
        public void Setup()
        {
            int count = 100;
            keys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(x => new List<int>() { x, x + 1, x + 2 }).ToList();
            values = Enumerable.Range(1, count).Select<int, string>(x => String.Format("{0}", x)).ToList();

            subKeys = Enumerable.Range(1, count).Select<int, IEnumerable<int>>(x => new List<int>() { x + 1, x + 2 }).ToList();
            positions = new List<int>() { -1, 2 };

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
            for (int i = 0; i < RecordCount; i++)
            {
                map.Add(new List<int>() { i, i + 1, i + 2 }, String.Format("{0}", i));
            }

            return map;
        }

        public IDictionary<IEnumerable<int>, string> Dictionary_AddLoop(IDictionary<IEnumerable<int>, string> dict)
        {
            for (int i = 0; i < RecordCount; i++)
            {
                dict.Add(new List<int>() { i, i + 1, i + 2 }, String.Format("{0}", i));
            }

            return dict;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<IEnumerable<int>>> MultiKeyMap_TryGetFullKeysByPartialKeyLoop()
        {
            var result = new List<IEnumerable<IEnumerable<int>>>(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                if (map.TryGetFullKeysByPartialKey(keys[i], out var value))
                {
                    result.Add(value);
                }

            }

            return result;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_TryGetValuesByPartialKeyLoop()
        {
            var result = new List<IEnumerable<string>>(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                if (map.TryGetValuesByPartialKey(keys[i], out var value))
                {
                    result.Add(value);
                }

            }

            return result;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<IEnumerable<int>>> MultiKeyMap_PositionalTryGetFullKeysByPartialKeyLoop()
        {
            var result = new List<IEnumerable<IEnumerable<int>>>(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                if (map.TryGetFullKeysByPartialKey(keys[i], positions, out var value))
                {
                    result.Add(value);
                }

            }

            return result;
        }

        [Benchmark]
        public IEnumerable<IEnumerable<string>> MultiKeyMap_PositionalTryGetValuesByPartialKeyLoop()
        {
            var result = new List<IEnumerable<string>>(keys.Count);

            for (int i = 0; i < keys.Count; i++)
            {
                if (map.TryGetValuesByPartialKey(keys[i], positions, out var value))
                {
                    result.Add(value);
                }

            }

            return result;
        }

        [Benchmark(Baseline = true)]
        public IEnumerable<string> Dictionary_TryGetValueLoop()
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
}