``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |             Method |                        Strategy |     Mean |     Error |    StdDev | Scaled |   Gen 0 |  Gen 1 | Allocated |
 |------------------- |-------------------------------- |---------:|----------:|----------:|-------:|--------:|-------:|----------:|
 | **MultiKeyMap_Remove** | **OptimizedForNonPositionalSearch** | **603.8 us** | **3.5025 us** | **3.1049 us** |   **2.75** | **68.3594** |      **-** | **283.21 KB** |
 |  Dictionary_Remove | OptimizedForNonPositionalSearch | 219.7 us | 0.4560 us | 0.3808 us |   1.00 | 36.6211 |      - |  150.4 KB |
 | **MultiKeyMap_Remove** |    **OptimizedForPositionalSearch** | **727.5 us** | **2.3995 us** | **2.0037 us** |   **2.31** | **96.6797** | **0.9766** | **398.04 KB** |
 |  Dictionary_Remove |    OptimizedForPositionalSearch | 315.0 us | 2.0312 us | 1.8006 us |   1.00 | 53.2227 | 0.4883 | 218.35 KB |
