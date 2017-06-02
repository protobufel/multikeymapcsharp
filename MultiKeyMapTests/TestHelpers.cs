using System;
using System.Collections.Generic;
using System.Linq;
using GitHub.Protobufel.MultiKeyMap;

namespace MultiKeyMapTests
{
    public static class TestHelpers
    {
        #region creation helpers
        public static IMultiKeyMap<T, K, V> CreateMultiKeyMap<T, K, V>(K[] keys, V[] values) where K : IEnumerable<T>
        {
            return CreateMultiKeyMap<T, K, V>(keys, values, () => MultiKeyMaps.CreateMultiKeyDictionary<T, K, V>());
        }

        public static IMultiKeyMap<T, K, V> CreateMultiKeyMap<T, K, V>(K[] keys, V[] values, Func<IMultiKeyMap<T, K, V>> supplier) where K : IEnumerable<T>
        {
            var map = supplier.Invoke();

            foreach (var entry in keys.Zip(values, (k, v) => new KeyValuePair<K, V>(k, v)))
            {
                map.Add(entry);
            }

            return map;
        }

        public static IDictionary<K, V> CreateDictionary<T, K, V>(K[] keys, V[] values) where K : IEnumerable<T>
        {
            var map = keys.Zip(values, (k, v) => new KeyValuePair<K, V>(k, v)).ToDictionary(entry => entry.Key, entry => entry.Value);
            return map;
        }
        #endregion
        #region map/dictionary/partKeys population

        public static (
                IDictionary<K, V> dict, 
                IEnumerable<K> keys, 
                IList<IEnumerable<T>> partKeys, 
                IEnumerable<int> positivePositions, 
                IEnumerable<int> positions) 
            InitData<T, K, V>(
                IEqualityComparer<T> subKeyComparer,
                IEqualityComparer<K> keyComparer,
                MultiKeyMaps.MultiKeyCreationStrategy strategy,
                Func<int, T> subKeyConverter,
                Func<int, V> valueConverter,
                int searchCount,
                int recordCount,
                int keySize)
            where K : IEnumerable<T>
        {
            var dict = new Dictionary<K, V>(keyComparer);
            PopulateDictionary(dict as IDictionary<IEnumerable<T>, V>, recordCount, keySize, subKeyConverter, valueConverter);
            var keys = dict.Keys.Where((x, index) => index % 3 == 0).Take(searchCount).ToList();
            (var partKeys, var positivePositions, var positions) = PopulatePartialKeys((dict as IDictionary<IEnumerable<T>, V>).Keys, searchCount);
            return (dict, keys, partKeys, positivePositions, positions);
        }


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
        #endregion

        #region comparer helpers

        public static IEqualityComparer<T> SubKeyComparerFor<T>(this bool subKeyEqualityByRef) where T : class
        {
            return subKeyEqualityByRef
                            ? EqualityComparerExtensions.ReferenceEqualityComparerOf<T>()
                            : EqualityComparer<T>.Default;
        }

        public static IEqualityComparer<IEnumerable<T>> KeyComparerFor<T>(this bool keyEqualityByRef) where T : class
        {
            return keyEqualityByRef 
                ? EqualityComparerExtensions.ReferenceEqualityComparerOf<IEnumerable<T>>() 
                : EqualityComparer<T>.Default.EnumerableEqualityComparerOf<T, IEnumerable<T>>();
        }

        #endregion
    }
}
