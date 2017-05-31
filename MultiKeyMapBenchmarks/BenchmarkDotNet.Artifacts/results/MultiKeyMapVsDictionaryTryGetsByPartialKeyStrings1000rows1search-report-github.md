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
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |  **5,286.3 ns** | **33.1862 ns** | **31.0423 ns** |  **40.44** |     **0.24** | **0.2594** |    **1112 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |  7,983.5 ns | 62.8220 ns | 58.7638 ns |  61.07 |     0.45 | 0.4425 |    1896 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch | 10,466.3 ns | 59.6924 ns | 55.8363 ns |  80.06 |     0.45 | 0.6409 |    2720 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |    130.7 ns |  0.3066 ns |  0.2868 ns |   1.00 |     0.00 | 0.0303 |     128 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** | **13,759.0 ns** | **60.9413 ns** | **54.0229 ns** | **103.62** |     **1.69** | **1.8616** |    **7864 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |  9,041.2 ns | 31.3891 ns | 27.8256 ns |  68.09 |     1.10 | 1.2970 |    5480 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |  2,331.5 ns | 21.5581 ns | 20.1654 ns |  17.56 |     0.32 | 0.2975 |    1256 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |    132.8 ns |  2.3719 ns |  2.2187 ns |   1.00 |     0.00 | 0.0303 |     128 B |
