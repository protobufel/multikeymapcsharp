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
 | **MultiKeyMap_TryGetValue** | **OptimizedForNonPositionalSearch** | **33.57 us** | **0.0561 us** | **0.0406 us** |   **1.00** | **3.0518** |  **12.56 KB** |
 |  Dictionary_TryGetValue | OptimizedForNonPositionalSearch | 33.73 us | 0.3124 us | 0.2922 us |   1.00 | 3.0518 |  12.56 KB |
 | **MultiKeyMap_TryGetValue** |    **OptimizedForPositionalSearch** | **34.29 us** | **0.1376 us** | **0.1287 us** |   **1.00** | **3.0518** |  **12.56 KB** |
 |  Dictionary_TryGetValue |    OptimizedForPositionalSearch | 34.24 us | 0.2583 us | 0.2416 us |   1.00 | 3.0518 |  12.56 KB |
