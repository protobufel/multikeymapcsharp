``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                                                  Method |                        Strategy |         Mean |      Error |      StdDev | Scaled | ScaledSD |    Gen 0 |  Gen 1 | Allocated |
 |-------------------------------------------------------- |-------------------------------- |-------------:|-----------:|------------:|-------:|---------:|---------:|-------:|----------:|
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** | **3,993.835 us** | **76.0634 us** |  **71.1497 us** | **774.55** |    **13.91** | **136.7188** | **7.8125** |  **587864 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch | 2,087.965 us | 19.9784 us |  17.7103 us | 404.93 |     3.90 |  85.9375 |      - |  360464 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch | 1,383.378 us |  6.2410 us |   5.5325 us | 268.29 |     1.72 |  64.4531 |      - |  272904 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |     5.156 us |  0.0291 us |   0.0273 us |   1.00 |     0.00 |   0.2136 |      - |     904 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** | **5,061.886 us** | **98.8395 us** | **138.5592 us** | **955.39** |    **28.78** | **335.9375** |      **-** | **1427440 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch | 2,726.744 us |  7.7777 us |   5.6238 us | 514.65 |     7.08 | 187.5000 |      - |  786576 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |   535.473 us |  2.0084 us |   1.8787 us | 101.07 |     1.42 |  33.2031 |      - |  142424 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |     5.299 us |  0.0863 us |   0.0765 us |   1.00 |     0.00 |   0.2136 |      - |     904 B |
