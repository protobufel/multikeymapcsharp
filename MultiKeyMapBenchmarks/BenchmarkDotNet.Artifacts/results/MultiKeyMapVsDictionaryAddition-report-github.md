``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |          Method |                        Strategy |       Mean |     Error |    StdDev | Scaled | ScaledSD |   Gen 0 |   Gen 1 | Allocated |
 |---------------- |-------------------------------- |-----------:|----------:|----------:|-------:|---------:|--------:|--------:|----------:|
 | **MultiKeyMap_Add** | **OptimizedForNonPositionalSearch** | **214.843 us** | **1.7234 us** | **1.6121 us** |  **44.18** |     **0.38** | **34.1797** |  **0.2441** | **140.52 KB** |
 |  Dictionary_Add | OptimizedForNonPositionalSearch |   4.864 us | 0.0273 us | 0.0242 us |   1.00 |     0.00 |  2.4872 |       - |  10.21 KB |
 | **MultiKeyMap_Add** |    **OptimizedForPositionalSearch** | **403.821 us** | **2.1189 us** | **1.8784 us** |  **82.24** |     **0.58** | **67.2852** | **33.6263** | **388.84 KB** |
 |  Dictionary_Add |    OptimizedForPositionalSearch |   4.911 us | 0.0314 us | 0.0279 us |   1.00 |     0.00 |  2.5177 |       - |  10.32 KB |
