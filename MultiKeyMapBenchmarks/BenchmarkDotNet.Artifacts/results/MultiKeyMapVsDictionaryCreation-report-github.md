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
 | **CreateDictionaryMultiKeyMap** | **OptimizedForNonPositionalSearch** | **161.13 ns** | **0.6697 ns** | **0.5936 ns** |   **9.55** |     **0.08** | **0.0627** |     **264 B** |
 |            CreateDictionary | OptimizedForNonPositionalSearch |  16.88 ns | 0.1433 ns | 0.1270 ns |   1.00 |     0.00 | 0.0190 |      80 B |
 | **CreateDictionaryMultiKeyMap** |    **OptimizedForPositionalSearch** | **204.40 ns** | **1.6882 ns** | **1.4965 ns** |  **11.88** |     **0.13** | **0.0894** |     **376 B** |
 |            CreateDictionary |    OptimizedForPositionalSearch |  17.21 ns | 0.1508 ns | 0.1410 ns |   1.00 |     0.00 | 0.0190 |      80 B |
