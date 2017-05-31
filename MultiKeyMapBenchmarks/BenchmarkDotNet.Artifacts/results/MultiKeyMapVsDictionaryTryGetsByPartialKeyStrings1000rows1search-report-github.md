``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                                                  Method |                        Strategy |        Mean |      Error |     StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
 |-------------------------------------------------------- |-------------------------------- |------------:|-----------:|-----------:|-------:|---------:|-------:|----------:|
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |  **5,176.8 ns** | **35.2651 ns** | **32.9870 ns** |  **39.87** |     **0.26** | **0.2594** |    **1112 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |  8,644.0 ns | 41.3801 ns | 38.7070 ns |  66.57 |     0.33 | 0.4883 |    2056 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch | 10,103.6 ns | 21.8971 ns | 20.4825 ns |  77.81 |     0.23 | 0.6409 |    2720 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |    129.8 ns |  0.3481 ns |  0.3086 ns |   1.00 |     0.00 | 0.0303 |     128 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** | **14,046.4 ns** | **45.8984 ns** | **40.6877 ns** | **107.24** |     **0.41** | **1.8921** |    **8024 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch | 12,917.5 ns | 28.6502 ns | 23.9242 ns |  98.62 |     0.31 | 1.2054 |    5104 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |  3,074.5 ns |  6.0024 ns |  5.0123 ns |  23.47 |     0.07 | 0.2975 |    1256 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |    131.0 ns |  0.3932 ns |  0.3485 ns |   1.00 |     0.00 | 0.0303 |     128 B |
