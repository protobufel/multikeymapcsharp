``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]   : .NET Core 4.6.25211.01, 64bit RyuJIT
  ShortRun : .NET Core 4.6.25211.01, 64bit RyuJIT

Job=ShortRun  LaunchCount=1  TargetCount=3  
WarmupCount=3  

```
 |                                                  Method |                        Strategy | SubKeyEqualityByRef | KeyEqualityByRef |         Mean |         Error |      StdDev | Scaled | ScaledSD |  Gen 0 |  Gen 1 | Allocated |
 |-------------------------------------------------------- |-------------------------------- |-------------------- |----------------- |-------------:|--------------:|------------:|-------:|---------:|-------:|-------:|----------:|
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |               **False** |            **False** | **16,751.92 ns** | **1,859.4996 ns** | **105.0652 ns** |   **9.48** |     **0.05** | **1.5869** | **0.2340** |    **9917 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |            False | 18,092.46 ns |   788.6834 ns |  44.5621 ns |  10.24 |     0.03 | 1.8005 |      - |    7584 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |            False | 23,646.35 ns |   579.9121 ns |  32.7661 ns |  13.39 |     0.03 | 2.2583 |      - |    9560 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |               False |            False |  1,766.48 ns |    82.9683 ns |   4.6879 ns |   1.00 |     0.00 | 0.1984 |      - |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |               **False** |             **True** |  **6,039.06 ns** |   **494.4366 ns** |  **27.9366 ns** | **115.26** |     **0.44** | **0.7706** | **0.1933** |    **4842 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |             True |  5,055.11 ns |   275.4613 ns |  15.5641 ns |  96.48 |     0.25 | 0.4044 |      - |    1704 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |             True |  7,649.12 ns |   922.1837 ns |  52.1051 ns | 145.99 |     0.82 | 0.5951 |      - |    2560 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |               False |             True |     52.39 ns |     0.6682 ns |   0.0378 ns |   1.00 |     0.00 |      - |      - |       0 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |                **True** |            **False** |    **389.32 ns** |    **25.3550 ns** |   **1.4326 ns** |   **0.22** |     **0.00** | **0.0539** | **0.0149** |     **341 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |            False |    128.13 ns |     3.6459 ns |   0.2060 ns |   0.07 |     0.00 | 0.0284 |      - |     120 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |            False |    127.25 ns |    11.2053 ns |   0.6331 ns |   0.07 |     0.00 | 0.0284 |      - |     120 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |                True |            False |  1,775.29 ns |   108.6025 ns |   6.1362 ns |   1.00 |     0.00 | 0.1984 |      - |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |                **True** |             **True** |    **394.40 ns** |    **28.0934 ns** |   **1.5873 ns** |   **7.27** |     **0.02** | **0.0539** | **0.0148** |     **341 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |             True |    125.98 ns |    11.7624 ns |   0.6646 ns |   2.32 |     0.01 | 0.0284 |      - |     120 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |             True |    125.05 ns |     5.3778 ns |   0.3039 ns |   2.30 |     0.01 | 0.0284 |      - |     120 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |                True |             True |     54.27 ns |     1.1661 ns |   0.0659 ns |   1.00 |     0.00 |      - |      - |       0 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |               **False** |            **False** | **21,369.18 ns** | **1,648.2007 ns** |  **93.1264 ns** |  **12.25** |     **0.05** | **2.5940** | **0.3764** |   **16378 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |            False | 12,475.20 ns |   568.9031 ns |  32.1441 ns |   7.15 |     0.02 | 1.7853 |      - |    7544 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |            False |  3,242.42 ns |   138.3780 ns |   7.8186 ns |   1.86 |     0.00 | 0.4158 |      - |    1760 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |               False |            False |  1,744.55 ns |    36.0761 ns |   2.0384 ns |   1.00 |     0.00 | 0.1984 |      - |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |               **False** |             **True** | **22,655.08 ns** | **1,328.2080 ns** |  **75.0462 ns** | **433.74** |     **3.23** | **3.1942** | **0.2441** |   **16137 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |             True | 13,206.98 ns |   559.8954 ns |  31.6351 ns | 252.85 |     1.82 | 1.7853 |      - |    7544 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |             True |  3,256.24 ns |    38.2939 ns |   2.1637 ns |  62.34 |     0.43 | 0.4158 |      - |    1760 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |               False |             True |     52.23 ns |     7.8338 ns |   0.4426 ns |   1.00 |     0.00 |      - |      - |       0 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |                **True** |            **False** |    **467.14 ns** |    **82.3762 ns** |   **4.6544 ns** |   **0.27** |     **0.00** | **0.0668** | **0.0149** |     **421 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |            False |    146.94 ns |     5.9772 ns |   0.3377 ns |   0.08 |     0.00 | 0.0379 |      - |     160 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |            False |    255.02 ns |    22.0710 ns |   1.2471 ns |   0.15 |     0.00 | 0.0510 |      - |     216 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |                True |            False |  1,740.04 ns |    23.4473 ns |   1.3248 ns |   1.00 |     0.00 | 0.1984 |      - |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |                **True** |             **True** |    **472.27 ns** |    **12.8190 ns** |   **0.7243 ns** |   **8.92** |     **0.02** | **0.0668** | **0.0149** |     **421 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |             True |    148.30 ns |     9.2396 ns |   0.5221 ns |   2.80 |     0.01 | 0.0379 |      - |     160 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |             True |    254.76 ns |    29.1318 ns |   1.6460 ns |   4.81 |     0.03 | 0.0510 |      - |     216 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |                True |             True |     52.92 ns |     1.9030 ns |   0.1075 ns |   1.00 |     0.00 |      - |      - |       0 B |
