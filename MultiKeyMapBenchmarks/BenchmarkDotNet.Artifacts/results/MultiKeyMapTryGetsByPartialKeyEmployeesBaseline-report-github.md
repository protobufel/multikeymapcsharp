``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507522 Hz, Resolution=285.1016 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]   : .NET Core 4.6.25211.01, 64bit RyuJIT
  ShortRun : .NET Core 4.6.25211.01, 64bit RyuJIT

Job=ShortRun  LaunchCount=1  TargetCount=3  
WarmupCount=3  

```
 |                                      Method | RecordCount | SearchCount | KeySize |                        Strategy | SubKeyEqualityByRef | KeyEqualityByRef |      Mean |     Error |    StdDev | Scaled |  Gen 0 | Allocated |
 |-------------------------------------------- |------------ |------------ |-------- |-------------------------------- |-------------------- |----------------- |----------:|----------:|----------:|-------:|-------:|----------:|
 |                  **TryGetFullKeysByPartialKey** |        **1000** |           **1** |      **10** | **OptimizedForNonPositionalSearch** |               **False** |            **False** |  **2.136 us** | **0.0629 us** | **0.0036 us** |   **1.00** | **0.2213** |     **936 B** |
 | Mixed_Positional_TryGetFullKeysByPartialKey |        1000 |           1 |      10 | OptimizedForNonPositionalSearch |               False |            False |  3.237 us | 0.1225 us | 0.0069 us |   1.52 | 0.4005 |    1688 B |
 |  Only_Positional_TryGetFullKeysByPartialKey |        1000 |           1 |      10 | OptimizedForNonPositionalSearch |               False |            False |  3.820 us | 0.2312 us | 0.0131 us |   1.79 | 0.4921 |    2072 B |
 |                  **TryGetFullKeysByPartialKey** |        **1000** |           **1** |      **10** |    **OptimizedForPositionalSearch** |               **False** |            **False** | **21.471 us** | **1.4227 us** | **0.0804 us** |   **1.00** | **3.2349** |   **13632 B** |
 | Mixed_Positional_TryGetFullKeysByPartialKey |        1000 |           1 |      10 |    OptimizedForPositionalSearch |               False |            False | 13.687 us | 0.2338 us | 0.0132 us |   0.64 | 1.8921 |    7992 B |
 |  Only_Positional_TryGetFullKeysByPartialKey |        1000 |           1 |      10 |    OptimizedForPositionalSearch |               False |            False |  2.835 us | 0.0802 us | 0.0045 us |   0.13 | 0.4082 |    1728 B |
