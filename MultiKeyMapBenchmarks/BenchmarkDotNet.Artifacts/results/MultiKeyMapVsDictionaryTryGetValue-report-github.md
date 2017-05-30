``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                  Method |                        Strategy |     Mean |     Error |    StdDev | Scaled |  Gen 0 | Allocated |
 |------------------------ |-------------------------------- |---------:|----------:|----------:|-------:|-------:|----------:|
 | **MultiKeyMap_TryGetValue** | **OptimizedForNonPositionalSearch** | **34.01 us** | **0.2007 us** | **0.1676 us** |   **1.00** | **3.0518** |  **12.56 KB** |
 |  Dictionary_TryGetValue | OptimizedForNonPositionalSearch | 33.84 us | 0.0840 us | 0.0744 us |   1.00 | 3.0518 |  12.56 KB |
 | **MultiKeyMap_TryGetValue** |    **OptimizedForPositionalSearch** | **33.54 us** | **0.1664 us** | **0.1390 us** |   **1.00** | **3.0518** |  **12.56 KB** |
 |  Dictionary_TryGetValue |    OptimizedForPositionalSearch | 33.66 us | 0.0592 us | 0.0525 us |   1.00 | 3.0518 |  12.56 KB |
