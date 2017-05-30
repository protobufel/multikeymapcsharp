``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |          Method |                        Strategy |       Mean |     Error |    StdDev | Scaled | ScaledSD |   Gen 0 |  Gen 1 | Allocated |
 |---------------- |-------------------------------- |-----------:|----------:|----------:|-------:|---------:|--------:|-------:|----------:|
 | **MultiKeyMap_Add** | **OptimizedForNonPositionalSearch** | **213.237 us** | **1.2228 us** | **1.0840 us** |  **44.77** |     **0.35** | **34.1797** | **0.2441** | **140.52 KB** |
 |  Dictionary_Add | OptimizedForNonPositionalSearch |   4.764 us | 0.0344 us | 0.0305 us |   1.00 |     0.00 |  2.4872 |      - |  10.21 KB |
 | **MultiKeyMap_Add** |    **OptimizedForPositionalSearch** | **310.649 us** | **0.4653 us** | **0.3364 us** |  **64.90** |     **0.22** | **50.7813** |      **-** | **208.48 KB** |
 |  Dictionary_Add |    OptimizedForPositionalSearch |   4.787 us | 0.0207 us | 0.0162 us |   1.00 |     0.00 |  2.5177 |      - |  10.32 KB |
