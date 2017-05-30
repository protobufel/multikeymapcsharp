``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |                                                  Method |                        Strategy |      Mean |     Error |    StdDev |    Median | Scaled | ScaledSD |    Gen 0 | Allocated |
 |-------------------------------------------------------- |-------------------------------- |----------:|----------:|----------:|----------:|-------:|---------:|---------:|----------:|
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** | **204.52 us** | **1.8742 us** | **1.7532 us** | **203.72 us** |  **14.84** |     **0.29** |  **32.9590** | **135.84 KB** |
 |                    MultiKeyMap_TryGetValuesByPartialKey | OptimizedForNonPositionalSearch | 204.74 us | 0.7252 us | 0.6784 us | 204.41 us |  14.86 |     0.26 |  32.9590 | 135.84 KB |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch | 200.91 us | 1.6190 us | 1.4352 us | 200.10 us |  14.58 |     0.27 |  35.8887 | 147.56 KB |
 |    MultiKeyMap_Mixed_PositionalTryGetValuesByPartialKey | OptimizedForNonPositionalSearch | 207.27 us | 4.0599 us | 5.8226 us | 203.71 us |  15.04 |     0.49 |  35.8887 | 147.56 KB |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch | 162.87 us | 0.3314 us | 0.2767 us | 162.84 us |  11.82 |     0.21 |  32.2266 | 132.07 KB |
 |     MultiKeyMap_Only_PositionalTryGetValuesByPartialKey | OptimizedForNonPositionalSearch | 165.07 us | 1.0708 us | 1.0016 us | 164.73 us |  11.98 |     0.22 |  32.2266 | 132.07 KB |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |  13.79 us | 0.2647 us | 0.2476 us |  13.82 us |   1.00 |     0.00 |   1.1444 |   4.75 KB |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** | **730.12 us** | **2.3554 us** | **1.9669 us** | **730.71 us** |  **54.32** |     **0.16** | **161.1328** | **662.45 KB** |
 |                    MultiKeyMap_TryGetValuesByPartialKey |    OptimizedForPositionalSearch | 732.36 us | 2.4165 us | 2.0178 us | 731.92 us |  54.48 |     0.16 | 161.1328 | 662.45 KB |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch | 654.71 us | 2.0868 us | 1.8499 us | 655.24 us |  48.71 |     0.15 | 150.3906 | 616.36 KB |
 |    MultiKeyMap_Mixed_PositionalTryGetValuesByPartialKey |    OptimizedForPositionalSearch | 668.74 us | 6.1855 us | 5.7859 us | 665.91 us |  49.75 |     0.42 | 150.3906 | 616.36 KB |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch | 335.80 us | 4.7577 us | 4.4504 us | 335.31 us |  24.98 |     0.32 |  74.2188 | 305.53 KB |
 |     MultiKeyMap_Only_PositionalTryGetValuesByPartialKey |    OptimizedForPositionalSearch | 338.06 us | 1.0295 us | 0.9127 us | 337.72 us |  25.15 |     0.07 |  74.2188 | 305.53 KB |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |  13.44 us | 0.0222 us | 0.0173 us |  13.45 us |   1.00 |     0.00 |   1.1444 |   4.75 KB |
