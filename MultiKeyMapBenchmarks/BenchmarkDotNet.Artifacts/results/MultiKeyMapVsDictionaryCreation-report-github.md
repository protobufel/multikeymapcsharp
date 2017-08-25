``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507522 Hz, Resolution=285.1016 ns, Timer=TSC
.NET Core SDK=2.0.0
  [Host]     : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT
  DefaultJob : .NET Core 2.0.0 (Framework 4.6.00001.0), 64bit RyuJIT


```
 |                      Method |                        Strategy |      Mean |     Error |    StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
 |---------------------------- |-------------------------------- |----------:|----------:|----------:|-------:|---------:|-------:|----------:|
 | **CreateDictionaryMultiKeyMap** | **OptimizedForNonPositionalSearch** | **176.03 ns** | **0.5348 ns** | **0.5003 ns** |  **10.53** |     **0.04** | **0.0627** |     **264 B** |
 |            CreateDictionary | OptimizedForNonPositionalSearch |  16.72 ns | 0.0524 ns | 0.0490 ns |   1.00 |     0.00 | 0.0190 |      80 B |
 | **CreateDictionaryMultiKeyMap** |    **OptimizedForPositionalSearch** | **229.84 ns** | **1.3902 ns** | **1.3004 ns** |  **12.03** |     **0.07** | **0.0894** |     **376 B** |
 |            CreateDictionary |    OptimizedForPositionalSearch |  19.11 ns | 0.0596 ns | 0.0498 ns |   1.00 |     0.00 | 0.0190 |      80 B |
