``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                      Method |                        Strategy |      Mean |     Error |    StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
 |---------------------------- |-------------------------------- |----------:|----------:|----------:|-------:|---------:|-------:|----------:|
 | **CreateDictionaryMultiKeyMap** | **OptimizedForNonPositionalSearch** | **168.54 ns** | **1.1452 ns** | **1.0713 ns** |   **9.74** |     **0.09** | **0.0627** |     **264 B** |
 |            CreateDictionary | OptimizedForNonPositionalSearch |  17.30 ns | 0.1213 ns | 0.1134 ns |   1.00 |     0.00 | 0.0190 |      80 B |
 | **CreateDictionaryMultiKeyMap** |    **OptimizedForPositionalSearch** | **210.38 ns** | **0.9357 ns** | **0.7814 ns** |  **12.27** |     **0.09** | **0.0894** |     **376 B** |
 |            CreateDictionary |    OptimizedForPositionalSearch |  17.15 ns | 0.1261 ns | 0.1118 ns |   1.00 |     0.00 | 0.0190 |      80 B |
