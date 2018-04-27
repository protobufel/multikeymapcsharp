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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GitHub.Protobufel.MultiKeyMap.Base
{
    [Serializable]
    internal abstract class NonPositionalBaseMultiKeyMap<T, K, V> : BaseMultiKeyMap<T, K, V>, IMultiKeyMap<T, K, V> where K : class, IEnumerable<T>
    {
        [NonSerialized]
        protected ILiteSetMultimap<T, K> partMap;

        protected NonPositionalBaseMultiKeyMap(IEqualityComparer<T> subKeyComparer = null, IEqualityComparer<K> fullKeyComparer = null,
            IDictionary<K, V> fullMap = null, ILiteSetMultimap<T, K> partMap = null) : base(subKeyComparer, fullKeyComparer, fullMap)
        {
            this.partMap = partMap ?? CreateLiteSetMultimap(subKeyComparer, fullKeyComparer);
        }

        protected virtual ILiteSetMultimap<TSubKey, TKey> CreateLiteSetMultimap<TSubKey, TKey>(
            IEqualityComparer<TSubKey> subKeyComparer, IEqualityComparer<TKey> fullKeyComparer)
            where TKey : class, IEnumerable<TSubKey>
        {
            return CreateSupportDictionary<TSubKey, ISet<TKey>>(subKeyComparer).ToSetMultimap(fullKeyComparer);
            //return CreateSupportDictionary<TSubKey, ISet<TKey>>(subKeyComparer).ToSetMultimap(EqualityComparerExtensions.ReferenceEqualityComparerOf<TKey>());
        }

        #region non-positional TryGetsByPartialKey

        public override bool TryGetFullKeysByPartialKey(IEnumerable<T> partialKey, out IEnumerable<K> fullKeys)
        {
            if (partialKey == null) throw new ArgumentNullException();

            if (partMap.Count == 0)
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (T subKey in partialKey)
            {
                if (!partMap.TryGetValue(subKey, out ISet<K> subResult))
                {
                    fullKeys = default(ISet<K>);
                    return false;
                }
                else if (subResult.Count < minSize)
                {
                    minSize = subResult.Count;
                    minPos = subResults.Count;
                }

                subResults.Add(subResult);
            }

            if (subResults.Count == 0)
            {
                fullKeys = default(ISet<K>);
                return false;
            }

            ISet<K> result = new HashSet<K>(subResults[minPos], EqualityComparerExtensions.ReferenceEqualityComparerOf<K>());

            if (subResults.Count == 1)
            {
                fullKeys = result;
                return true;
            }

            for (int i = 0; i < subResults.Count; i++)
            {
                if (i != minPos)
                {
                    result.IntersectWith(subResults[i]);

                    if (result.Count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }
                }
            }

            fullKeys = result;
            return true;
        }

        #endregion

        #region positioned filtered queries

        public override bool TryGetFullKeysByPartialKey(IEnumerable<T> subKeys, IEnumerable<int> positions, out IEnumerable<K> fullKeys)
        {
            if (subKeys == null) throw new ArgumentNullException("subKeys");
            if (positions == null) throw new ArgumentNullException("positions");

            if (!subKeys.Any())
            {
                fullKeys = default(IEnumerable<K>);
                return false;
            }

            IList<int> positionList = positions as IList<int> ?? positions.ToList();
            IList<T> subKeyList = subKeys as IList<T> ?? subKeys.ToList();
            IList<ISet<K>> subResults = new List<ISet<K>>();
            int minSize = int.MaxValue;
            int minPos = -1;

            foreach (var subKey in subKeyList)
            {
                if (!partMap.TryGetValue(subKey, out ISet<K> subResult))
                {
                    fullKeys = default(IEnumerable<K>);
                    return false;
                }

                if (subResult.Count < minSize)
                {
                    minSize = subResult.Count;
                    minPos = subResults.Count;
                }

                subResults.Add(subResult);
            }

            if (subResults.Count == 0)
            {
                fullKeys = default(IEnumerable<K>);
                return false;
            }

            HashSet<K> result;

            if (GetAtOrNegative(positionList, minPos) < 0)
            {
                result = new HashSet<K>(subResults[minPos], EqualityComparerExtensions.ReferenceEqualityComparerOf<K>());

            }
            else if (!TryGetFilteredFullKeys(GetAtOrNegative(positionList, minPos), subKeyList[minPos], subResults[minPos], out result))
            {
                fullKeys = default(IEnumerable<K>);
                return false;
            }

            if (subResults.Count == 1)
            {
                fullKeys = result;
                return true;
            }

            for (int i = 0; i < subResults.Count; i++)
            {
                if (i != minPos)
                {
                    result.IntersectWith(subResults[i]);

                    if (result.Count == 0)
                    {
                        fullKeys = default(ISet<K>);
                        return false;
                    }
                }
            }

            var subKeyPosList = subKeyList
                .Zip<T, int, (T subKey, int position)>(positionList, (subKey, position) => (subKey, position))
                .Where((tuple, index) => (index != minPos) && (tuple.position > 0))
                .ToList();

            if (subKeyPosList.Count != 0)
            {
                // could be streamed via IEnumerable result.Where(<the predicate opposite the one below>) but then subKeyList and positionList should always be copied
                result.RemoveWhere(key =>
                {
                    foreach (var subKeyPos in subKeyPosList)
                    {
                        if (!SubKeyComparer.Equals(subKeyPos.subKey, key.ElementAtOrDefault(subKeyPos.position)))
                        {
                            return true;
                        }
                    }

                    return false;
                });

                if (result.Count == 0)
                {
                    fullKeys = default(IEnumerable<K>);
                    return false;
                }
            }

            fullKeys = result;
            return true;
        }

        protected virtual bool TryGetFilteredFullKeys(int position, T subKey, ISet<K> source, out HashSet<K> target)
        {
            if (source.Count == 0)
            {
                target = default(HashSet<K>);
                return false;
            }

            target = new HashSet<K>(EqualityComparerExtensions.ReferenceEqualityComparerOf<K>());

            foreach (K fullKey in source)
            {
                if (SubKeyComparer.Equals(subKey, fullKey.ElementAtOrDefault(position)))
                {
                    target.Add(fullKey);
                }
            }

            if (target.Count == 0)
            {
                target = default(HashSet<K>);
                return false;
            }

            return true;
        }

        #endregion

        #region Implementation of the partial map helpers

        protected override void AddPartial(K key)
        {
            foreach (T subKey in key)
            {
                partMap.Add(subKey, key);
            }
        }

        protected override void DeletePartial(K key)
        {
            foreach (T subKey in key)
            {
                partMap.Remove(subKey, key);
            }
        }


        protected override void ClearPartial()
        {
            partMap.Clear();
        }

        #endregion

        protected override void OnDeserializedHelper(StreamingContext context)
        {
            base.OnDeserializedHelper(context);

            partMap = CreateLiteSetMultimap(subKeyComparer, fullKeyComparer);

            foreach (var entry in fullMap)
            {
                AddPartial(entry.Key);
            }
        }
    }
}
