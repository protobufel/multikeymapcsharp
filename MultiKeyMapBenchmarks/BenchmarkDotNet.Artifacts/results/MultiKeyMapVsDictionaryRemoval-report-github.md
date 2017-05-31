``` ini

BenchmarkDotNet=v0.10.6, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=Intel Core i7-4790 CPU 3.60GHz (Haswell), ProcessorCount=8
Frequency=3507520 Hz, Resolution=285.1017 ns, Timer=TSC
dotnet cli version=1.0.4
  [Host]     : .NET Core 4.6.25211.01, 64bit RyuJIT
  DefaultJob : .NET Core 4.6.25211.01, 64bit RyuJIT


```
 |             Method |                        Strategy |     Mean |     Error |    StdDev | Scaled | ScaledSD |    Gen 0 |   Gen 1 | Allocated |
 |------------------- |-------------------------------- |---------:|----------:|----------:|-------:|---------:|---------:|--------:|----------:|
 | **MultiKeyMap_Remove** | **OptimizedForNonPositionalSearch** | **628.8 us** | **4.9192 us** | **4.3607 us** |   **2.80** |     **0.02** |  **68.3594** |       **-** | **283.21 KB** |
 |  Dictionary_Remove | OptimizedForNonPositionalSearch | 224.7 us | 0.9011 us | 0.7988 us |   1.00 |     0.00 |  36.6211 |       - |  150.4 KB |
 | **MultiKeyMap_Remove** |    **OptimizedForPositionalSearch** | **839.4 us** | **4.4798 us** | **4.1904 us** |   **2.10** |     **0.01** | **126.6927** | **31.7708** | **578.41 KB** |
 |  Dictionary_Remove |    OptimizedForPositionalSearch | 400.2 us | 2.0390 us | 1.9073 us |   1.00 |     0.00 |  83.0078 | 27.3438 | 398.72 KB |
