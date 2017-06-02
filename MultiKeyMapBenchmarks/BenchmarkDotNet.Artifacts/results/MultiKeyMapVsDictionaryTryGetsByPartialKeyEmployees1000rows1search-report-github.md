``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507521 Hz, Resolution=285.1016 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]   : .NET Core 4.6.25211.01, 64bit RyuJIT
  ShortRun : .NET Core 4.6.25211.01, 64bit RyuJIT

Job=ShortRun  LaunchCount=1  TargetCount=3  
WarmupCount=3  

```
 |                                                  Method |                        Strategy | SubKeyEqualityByRef | KeyEqualityByRef |         Mean |         Error |      StdDev | Scaled | ScaledSD |  Gen 0 | Allocated |
 |-------------------------------------------------------- |-------------------------------- |-------------------- |----------------- |-------------:|--------------:|------------:|-------:|---------:|-------:|----------:|
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |               **False** |            **False** | **12,990.40 ns** |   **651.9491 ns** |  **36.8363 ns** |   **7.24** |     **0.02** | **1.4038** |    **5936 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |            False | 17,909.72 ns |   725.1369 ns |  40.9716 ns |   9.98 |     0.02 | 2.3193 |    9824 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |            False | 24,546.12 ns |   601.9293 ns |  34.0101 ns |  13.68 |     0.02 | 2.2583 |    9560 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |               False |            False |  1,794.93 ns |    24.6116 ns |   1.3906 ns |   1.00 |     0.00 | 0.1984 |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |               **False** |             **True** |  **2,351.48 ns** |    **69.4887 ns** |   **3.9262 ns** | **116.77** |     **0.38** | **0.2098** |     **896 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |             True |  4,986.97 ns |   110.8747 ns |   6.2646 ns | 247.65 |     0.78 | 0.4044 |    1704 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |               False |             True |  7,710.21 ns |   224.9538 ns |  12.7103 ns | 382.89 |     1.25 | 0.5951 |    2560 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |               False |             True |     20.14 ns |     1.3028 ns |   0.0736 ns |   1.00 |     0.00 |      - |       0 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |                **True** |            **False** |     **55.41 ns** |     **3.8541 ns** |   **0.2178 ns** |   **0.03** |     **0.00** | **0.0190** |      **80 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |            False |     83.88 ns |     3.0662 ns |   0.1732 ns |   0.05 |     0.00 | 0.0285 |     120 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |            False |     82.62 ns |     1.5100 ns |   0.0853 ns |   0.05 |     0.00 | 0.0285 |     120 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |                True |            False |  1,766.25 ns |   128.6370 ns |   7.2682 ns |   1.00 |     0.00 | 0.1984 |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** | **OptimizedForNonPositionalSearch** |                **True** |             **True** |     **55.94 ns** |     **2.3280 ns** |   **0.1315 ns** |   **2.47** |     **0.03** | **0.0190** |      **80 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |             True |     87.33 ns |    11.7017 ns |   0.6612 ns |   3.85 |     0.06 | 0.0285 |     120 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey | OptimizedForNonPositionalSearch |                True |             True |     88.05 ns |     9.2020 ns |   0.5199 ns |   3.89 |     0.06 | 0.0285 |     120 B |
 |                                  Dictionary_TryGetValue | OptimizedForNonPositionalSearch |                True |             True |     22.66 ns |     6.5914 ns |   0.3724 ns |   1.00 |     0.00 |      - |       0 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |               **False** |            **False** | **20,138.68 ns** | **1,939.4051 ns** | **109.5800 ns** |  **11.51** |     **0.06** | **3.2349** |   **13592 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |            False | 13,615.85 ns |   371.8788 ns |  21.0118 ns |   7.78 |     0.02 | 2.2125 |    9288 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |            False |  3,521.08 ns |   370.1382 ns |  20.9135 ns |   2.01 |     0.01 | 0.5226 |    2208 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |               False |            False |  1,749.57 ns |    71.7361 ns |   4.0532 ns |   1.00 |     0.00 | 0.1984 |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |               **False** |             **True** | **17,336.27 ns** |   **598.2607 ns** |  **33.8028 ns** | **873.72** |     **4.63** | **3.2654** |   **13824 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |             True | 14,195.43 ns |   760.6634 ns |  42.9789 ns | 715.43 |     4.02 | 2.1515 |    9056 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |               False |             True |  3,516.90 ns |    59.5553 ns |   3.3650 ns | 177.25 |     0.91 | 0.5226 |    2208 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |               False |             True |     19.84 ns |     2.1786 ns |   0.1231 ns |   1.00 |     0.00 |      - |       0 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |                **True** |            **False** |    **127.92 ns** |    **23.4697 ns** |   **1.3261 ns** |   **0.07** |     **0.00** | **0.0379** |     **160 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |            False |    118.80 ns |    42.8401 ns |   2.4205 ns |   0.07 |     0.00 | 0.0380 |     160 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |            False |    165.07 ns |    30.5713 ns |   1.7273 ns |   0.09 |     0.00 | 0.0608 |     256 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |                True |            False |  1,762.58 ns |   143.6602 ns |   8.1171 ns |   1.00 |     0.00 | 0.1984 |     840 B |
 |                  **MultiKeyMap_TryGetFullKeysByPartialKey** |    **OptimizedForPositionalSearch** |                **True** |             **True** |    **136.27 ns** |     **3.6177 ns** |   **0.2044 ns** |   **6.48** |     **0.01** | **0.0379** |     **160 B** |
 | MultiKeyMap_Mixed_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |             True |    103.36 ns |    12.7205 ns |   0.7187 ns |   4.92 |     0.03 | 0.0380 |     160 B |
 |  MultiKeyMap_Only_Positional_TryGetFullKeysByPartialKey |    OptimizedForPositionalSearch |                True |             True |    158.05 ns |     5.3473 ns |   0.3021 ns |   7.52 |     0.01 | 0.0608 |     256 B |
 |                                  Dictionary_TryGetValue |    OptimizedForPositionalSearch |                True |             True |     21.02 ns |     0.3111 ns |   0.0176 ns |   1.00 |     0.00 |      - |       0 B |
