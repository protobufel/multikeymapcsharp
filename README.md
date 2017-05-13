[![Build status](https://ci.appveyor.com/api/projects/status/b98onv6m5cb39mly?svg=true)](https://ci.appveyor.com/project/protobufel/multikeymapcsharp)
[![Build status](https://img.shields.io/nuget/v/Nuget.Core.svg)](https://www.nuget.org/packages/MultiKeyMap/)
# MultiKeyMap C# Implementation #

C# implementation of the multi-key map.  It behaves like a regular generic IDictionary with the additional ability of getting its values by any combination of partial keys. For example, one can add any value with the complex key {"Hello", "the", "wonderful", "World!"} , and then query by any sequence of subkeys like {"wonderful", "Hello"}.  
