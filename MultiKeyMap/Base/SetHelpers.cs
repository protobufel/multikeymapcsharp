using System.Collections.Generic;
using System.Linq;

namespace GitHub.Protobufel.MultiKeyMap.Base
{
    static class SetHelpers
    {

        public static bool IntersectWith<E>(HashSet<E> set, IEnumerable<ISet<E>> sets, out HashSet<E> result)
        {
            if ((set.Count == 0) || !sets.Any())
            {
                result = default(HashSet<E>);
                return false;
            }

            result = new HashSet<E>(set.Comparer);

            foreach (var subSet in sets)
            {
                foreach (E item in subSet)
                {
                    if (set.Contains(item))
                    {
                        result.Add(item);
                    }
                }
            }

            if (result.Count == 0)
            {
                result = default(HashSet<E>);
                return false;
            }

            return true;
        }

        public static bool TryJoinSets<E>(IEnumerable<ISet<E>> sets, IEqualityComparer<E> comparer, out ISet<E> result)
        {
            bool first = true;
            result = null;

            foreach (var set in sets)
            {
                if (first)
                {
                    first = false;
                    result = new HashSet<E>(set, comparer);
                }
                else
                {
                    result.UnionWith(set);
                }
            }

            return (result != null) && (result.Count > 0);
        }
    }
}
