[![Build status](https://ci.appveyor.com/api/projects/status/b98onv6m5cb39mly?svg=true)](https://ci.appveyor.com/project/protobufel/multikeymapcsharp)
[![NuGet](https://img.shields.io/nuget/v/multikeymap.svg?style=plastic)](https://www.nuget.org/packages/multikeymap/)
[![license](https://img.shields.io/github/license/protobufel/multikeymapcsharp/apistatus.svg?style=plastic)](https://github.com/protobufel/multikeymapcsharp)

<!--- ([![NuGet Pre Release](https://img.shields.io/nuget/vpre/multikeymap.svg?style=plastic)](https://www.nuget.org/packages/multikeymap/)) --->

# MultiKeyMap C# Implementation #

C# implementation of the multi-key map.  It behaves like a regular generic IDictionary with the additional ability of getting its values by any combination of partial keys. For example, one can add any value with the complex key {"Hello", "the", "wonderful", "World!"} , and then query by any sequence of subkeys like {"wonderful", "Hello"}. 

In addition, you can query by a mixture of some any-position-sub-keys and positional sub-keys, as in the following example: 

```csharp
using System;
using System.Linq;
using System.Collections.Generic;
using GitHub.Protobufel.MultiKeyMap;
using static GitHub.Protobufel.MultiKeyMap.MultiKeyMaps.MultiKeyCreationStrategy;

namespace MultiKeyMapExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // add the latest NuGet MultiKeyMap package to your project first!
            // then add 'using GitHub.Protobufel.MultiKeyMap;' statement as above

            IMultiKeyMap<string, IEnumerable<string>, string> map = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>();

            // add a record
            map.Add(new string[] { "Hello", ",", "wonderful", "world" }, "You found me!");

            // or copy some data from the compatible IDictionary
            var dict = new Dictionary<IEnumerable<string>, string>()
            {
                { new List<string>() { "okay", "I", "am", "here" }, "or there!" },
                { new string[] { "okay", "I", "am", "not", "here" }, "for sure!" }
            };

            // adding the data from another IDictionary or IMultiKeyMap
            map.CopyFrom(dict);

            // setting (clearing this and adding) the data of another IDictionary or IMultiKeyMap
            map.Of(dict);

            // IMultiKeyMap interface extends IDictionary, and also adds TryGet{FullKeys|Values|Entries}ByPartialKey methods of its own

            if (map.TryGetValue(new List<string>() { "okay", "I", "am", "here" }, out var exactMatch))
            {
                Console.WriteLine($"This is a regular IDictionary method, looking for exact full key. Let's see the actual value: {exactMatch}");
            }

            //lets look by partial key anywhere within the full key (any sequence in any order of some sub-keys of the original full key we're looking for)
            if (map.TryGetValuesByPartialKey(new string[] { "not", "I" }, out var values))
            {
                Console.WriteLine($"Should be 1 record with value = 'for sure!'. Let's see the actual one: {values.First()}");
            }

            //lets look by partial key, wherein some sub-keys are looked at the particular 0-based positions ( >= 0), and others anywhere ( < 0)
            if (map.TryGetValuesByPartialKey(new string[] { "here", "I", "am" }, new int[] { 3, -1, -1 }, out values))
            {
                Console.WriteLine($"Should be 1 record with value = 'or there!'. Let's see the actual one: {values.First()}");
            }

            // you can also use (position, subKey) tuple sequence to the same effect
            if (map.TryGetValuesByPartialKey(new List<(int position, string subKey)> { (3, "here"), (-1, "I"), (-1, "am") }, out values))
            {
                Console.WriteLine($"Should be 1 record with value = 'or there!'. Let's see the actual one: {values.First()}");
            }

            // by default, the created IMultiKeyMap instance will be optimized for non-positional search
            // for position optimized search use the strategy optional parameter
            var positionOptimizedMap = MultiKeyMaps.CreateMultiKeyDictionary<string, IEnumerable<string>, string>(OptimizedForPositionalSearch);

            // in addition, the returned IMultiKeyMap instance is fully serializable.

            map.Clear();
            // Happy using!

            Console.Read();
        }
    }
}
```

See the performance results at [MultiKeyMapBenchmarks/BenchmarkDotNet.Artifacts/results](https://github.com/protobufel/multikeymapcsharp/tree/master/MultiKeyMapBenchmarks/BenchmarkDotNet.Artifacts/results)

Happy coding,

David Tesler
