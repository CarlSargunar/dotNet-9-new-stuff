# What's new in .NET 9 and C# 13

This repo has a bunch of samples that show off new features in .NET 9.

## .NET Aspire

**_NOTE: You're able to use .NET Aspire 9.0 with either .NET 8 or .NET 9!_**

- No longer needs the workload, Aspire is part of the SDK
- .NET Templates used to install project types

```bash
    dotnet new install Aspire.ProjectTemplates::9.0.0 --force
```

- New dashboard features
  - Manage resource lifecycles - stop, start, restart individual services
  - Combine telemetry from multiple sources (e.g. when a service has multiple instances)
  - Browser telemetry support (JavaScript OpenTelemetry SDK)
- Orchestration improvements
  - WaitFor(XYZ) : Wait for service XYZ to be ready before starting
  - WaitForCompletion(XYZ) : Wait for XYZ to complete before starting
- Resource health checks : .WithHttpHealthCheck("/health");
  - Will poll HealthCheck endpoint until it returns 200 OK
- Persistent Containers : .WithLifetime(ContainerLifetime.Persistent);
  - Will keep the container running during rebuilds of the app
- Container Networking : All services added to **'default-aspire-network'** network
- Eventing Model : Hook into application lifecycle events
- Support for Azure Functions (preview)

### Aspire Demo Notes

- Install the Aspire SDK
  - dotnet new install Aspire.ProjectTemplates::9.0.0 --force
- Create a new Aspire project
  - dotnet new aspire-starter --use-redis-cache --output Aspire
- Trust dev Certs
  - dotnet dev-certs https --trust
- Run the project
  dotnet run --project Aspire/Aspire.AppHost

## Params Collections

Params now supports various collections beyond arrays, like `Span<T>` and `ReadOnlySpan<T>`.

So instead of this:

```csharp

 // Method without using params
static void PrintNumbersWithSpan(ReadOnlySpan<int> numbers)
{
    Console.WriteLine("Using ReadOnlySpan<int> without params:");
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
}
PrintNumbersWithSpan(new ReadOnlySpan<int>(new[] { 6, 7, 8, 9, 10 }));
```

We can do this:

```csharp
public void PrintNumbersWithSpan(params ReadOnlySpan<int> numbers)
{
    foreach (var number in numbers)
    {
        Console.WriteLine(number);
    }
}

PrintNumbersWithSpan(1, 2, 3, 4, 5);
```

To run the project

```bash
dotnet run --project ParamsDemo
```


## Lock Object

Previously you could lock with an Object, but with .NET 9 there's a new Lock Object.

- Uses a new mechanism - Previously a Monitor class was used, but now uses the Dispose pattern.
- Asynchronous Limitations - You can't use the lock object with async methods, use the SemaphoreSlim class instead.

```csharp
public class LockExample
{
    private readonly Lock _lock = new();

    public void DoStuff()
    {
        lock (_lock)
        {
            Console.WriteLine("We're inside .NET 9 lock");
        }
    }
}
```

To Run the project

```bash
dotnet run --project LockObj
```

## Hybrid Cache

A new caching mechanism that combines in-memory (in-process) and distributed (out-of-process) caching. Designed to avoid problems like cache stampedes. It's still in preview, and will be relased in a future release of .NET 9.

Supports older versions, down to Framework v4.7.2, and Standard 2.0

To install; the Hybrid Cache package, run the following command.

```bash
    dotnet add package Microsoft.Extensions.Caching.Hybrid --version "9.0.0-preview.7.24406.2"
```

Stampede : When a cache expires, and multiple requests come in at the same time, they all hit the database at the same time.

For more info see : https://www.youtube.com/watch?v=PDIKTbbkmCk

## New LINQ Methods : CountBy, AggregateBy, and Index

New Methods : Index, CountBy, AggregateBy

- Index - return the index of the element in the collection
- CountBy - groups the elements in the collection by a key and returns the count of elements in each group.
- AggregateBy - groups the elements in the collection by a key and returns the aggregate value of elements in each group.

To run the sample, run the following

```bash
dotnet run --project NewLinq
```

## Built-in UUID v7 Generation

Previously .NET used the UUID v4 generation, but now you can use the UUID v7 generation, which keeps a timestamp in the UUID.

Importantly, these can be stored in a database, compatible with UniqueIdentifier

| Timestamp        | Random        | More Random   |
| ---------------- | ------------- | ------------- |
| 48-bit timestamp | 12-bit random | 62-bit random |

    Guids (UUID4) look like this : 833dd2cd-f8d6-4688-8a43-769f06200874
    UUID7 looks like this        : 0193b13d-4517-72f7-9866-20f4404cc050

To run the project run :

```bash
dotnet run --project NewUUID
```

## Task.WhenEach()

If you had a list of Tasks and wanted to process each as soon as it completes, Task.WhenEach() is the method for you.

```csharp
// List of 5 tasks that finish at random intervals
var tasks = Enumerable.Range(1, 5)
   .Select(async i =>
   {
     await Task.Delay(new Random().Next(1000, 5000));
     return $"Task {i} done";
   })
   .ToList();

// Before
while(tasks.Count > 0)
{
   var completedTask = await Task.WhenAny(tasks);
   tasks.Remove(completedTask);
   Console.WriteLine(await completedTask);
}

// .NET 9
await foreach (var completedTask in Task.WhenEach(tasks))
   Console.WriteLine(await completedTask);
```

To Run the project

```bash
dotnet run --project WhenEach
```

## Performance Improvements

If you really want to blow your mind, go check out the performance blog release for .NET 9 at [https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/](https://devblogs.microsoft.com/dotnet/performance-improvements-in-net-9/).

This project runs a few benchmarks, incluing hashing, math functions and text manipulation across .Net 6. 8 and 9. It uses the BenchmarkDotNet library to run the benchmarks. To run the code run the following command.

```bash
dotnet run --project Performance -c Release --framework net6.0
dotnet run --project Performance -c Release --framework net8.0
dotnet run --project Performance -c Release --framework net9.0
```

Here are some of the results from the benchmarks.

### .NET 6

---

| Method                 |            Mean |         Error |        StdDev |          Median |      Gen0 |      Gen1 |      Gen2 |  Allocated |
| ---------------------- | --------------: | ------------: | ------------: | --------------: | --------: | --------: | --------: | ---------: |
| LinqSum                |     7,614.06 ns |    146.505 ns |    425.037 ns |     7,371.89 ns |    0.0153 |         - |         - |       40 B |
| LinqOrderBySum         |    58,338.43 ns |    228.745 ns |    202.776 ns |    58,265.73 ns |    7.8125 |         - |         - |    12312 B |
| ComputeSha256          |    50,293.61 ns |    125.765 ns |    117.641 ns |    50,321.76 ns |    0.0610 |         - |         - |      112 B |
| ComputeMd5             |    22,755.18 ns |     25.114 ns |     23.492 ns |    22,752.32 ns |    0.0305 |         - |         - |       80 B |
| CalculateFibonacci     | 6,481,466.82 ns | 20,222.151 ns | 18,915.813 ns | 6,482,283.59 ns |         - |         - |         - |        7 B |
| CalculatePrimes        |   391,165.61 ns |    488.771 ns |    381.600 ns |   391,210.25 ns |         - |         - |         - |          - |
| ReverseString          |        46.69 ns |      0.314 ns |      0.294 ns |        46.63 ns |    0.1428 |         - |         - |      224 B |
| ReverseStringUsingSpan |        39.95 ns |      0.128 ns |      0.100 ns |        39.97 ns |    0.0714 |         - |         - |      112 B |
| CountVowels            |       283.69 ns |      2.725 ns |      2.275 ns |       282.95 ns |         - |         - |         - |          - |
| CountVowelsUsingSpan   |       282.51 ns |      0.991 ns |      0.878 ns |       282.42 ns |         - |         - |         - |          - |
| JsonSerialization      |       313.53 ns |      6.330 ns |     11.086 ns |       312.23 ns |    0.1631 |         - |         - |      256 B |
| JsonDeserialization    |       797.71 ns |      5.227 ns |      4.890 ns |       797.72 ns |    0.1574 |         - |         - |      248 B |
| ThrowAndCatchException | 7,698,602.08 ns | 83,460.120 ns | 78,068.648 ns | 7,715,179.69 ns |  218.7500 |         - |         - |   344008 B |
| GarbageCollectionTest  |   684,728.97 ns |  3,663.886 ns |  3,427.201 ns |   684,608.84 ns | 7000.0000 | 1000.0000 | 1000.0000 | 10480337 B |
| ParallelFor            |   543,406.26 ns |  3,423.335 ns |  3,202.190 ns |   545,205.37 ns |    0.9766 |         - |         - |     2438 B |
| TasksWithWhenAll       |   414,905.98 ns |  2,185.347 ns |  2,044.175 ns |   414,877.29 ns |    0.9766 |         - |         - |     1584 B |
| ThreadPoolBenchmarks   |   502,558.52 ns |  4,034.446 ns |  3,368.946 ns |   501,736.82 ns |         - |         - |         - |      529 B |

### .NET 8

---

| Method                 |            Mean |          Error |         StdDev |      Gen0 |      Gen1 |      Gen2 | Allocated |
| ---------------------- | --------------: | -------------: | -------------: | --------: | --------: | --------: | --------: |
| LinqSum                |       109.43 ns |       0.390 ns |       0.345 ns |         - |         - |         - |         - |
| LinqOrderBySum         |    25,684.01 ns |     147.426 ns |     130.689 ns |    7.8430 |         - |         - |   12312 B |
| ComputeSha256          |    50,412.54 ns |     235.956 ns |     209.169 ns |    0.0610 |         - |         - |     112 B |
| ComputeMd5             |    22,736.64 ns |      18.861 ns |      14.725 ns |    0.0305 |         - |         - |      80 B |
| CalculateFibonacci     | 3,386,988.50 ns |  14,829.071 ns |  13,145.585 ns |         - |         - |         - |       2 B |
| CalculatePrimes        |   371,194.93 ns |     999.316 ns |     885.867 ns |         - |         - |         - |         - |
| ReverseString          |        35.79 ns |       0.258 ns |       0.229 ns |    0.1428 |         - |         - |     224 B |
| ReverseStringUsingSpan |        27.13 ns |       0.218 ns |       0.204 ns |    0.0714 |         - |         - |     112 B |
| CountVowels            |       164.22 ns |       1.428 ns |       1.266 ns |         - |         - |         - |         - |
| CountVowelsUsingSpan   |       162.22 ns |       1.054 ns |       0.986 ns |         - |         - |         - |         - |
| JsonSerialization      |       270.56 ns |       5.338 ns |       7.825 ns |    0.0663 |         - |         - |     104 B |
| JsonDeserialization    |       674.63 ns |       7.848 ns |       6.127 ns |    0.1574 |         - |         - |     248 B |
| ThrowAndCatchException | 7,820,959.01 ns | 134,955.796 ns | 126,237.735 ns |  218.7500 |         - |         - |  344003 B |
| GarbageCollectionTest  |   113,350.04 ns |   2,211.646 ns |   2,172.132 ns | 1000.0000 | 1000.0000 | 1000.0000 |     200 B |
| ParallelFor            |   537,500.52 ns |     796.256 ns |     705.860 ns |    0.9766 |         - |         - |    2272 B |
| TasksWithWhenAll       |   509,305.98 ns |   6,182.510 ns |   5,783.124 ns |    0.9766 |         - |         - |    1585 B |
| ThreadPoolBenchmarks   |   491,315.57 ns |   5,652.214 ns |   5,287.085 ns |         - |         - |         - |     528 B |

### .NET 9

---

| Method                 |            Mean |         Error |        StdDev |      Gen0 |      Gen1 |      Gen2 | Allocated |
| ---------------------- | --------------: | ------------: | ------------: | --------: | --------: | --------: | --------: |
| LinqSum                |       107.89 ns |      0.200 ns |      0.187 ns |         - |         - |         - |         - |
| LinqOrderBySum         |    22,477.61 ns |     97.449 ns |     91.154 ns |    7.8125 |         - |         - |   12280 B |
| ComputeSha256          |    50,380.23 ns |    179.963 ns |    159.532 ns |    0.0610 |         - |         - |     112 B |
| ComputeMd5             |    22,759.29 ns |     27.064 ns |     25.315 ns |    0.0305 |         - |         - |      80 B |
| CalculateFibonacci     | 3,603,892.29 ns | 18,122.134 ns | 16,951.455 ns |         - |         - |         - |       3 B |
| CalculatePrimes        |   365,074.52 ns |  1,145.372 ns |  1,071.382 ns |         - |         - |         - |         - |
| ReverseString          |        35.49 ns |      0.218 ns |      0.193 ns |    0.1428 |         - |         - |     224 B |
| ReverseStringUsingSpan |        27.52 ns |      0.301 ns |      0.281 ns |    0.0714 |         - |         - |     112 B |
| CountVowels            |       161.98 ns |      1.535 ns |      1.361 ns |         - |         - |         - |         - |
| CountVowelsUsingSpan   |       146.90 ns |      1.170 ns |      1.037 ns |         - |         - |         - |         - |
| JsonSerialization      |       248.15 ns |      5.035 ns |      5.597 ns |    0.0663 |         - |         - |     104 B |
| JsonDeserialization    |       506.49 ns |      1.792 ns |      1.496 ns |    0.1574 |         - |         - |     248 B |
| ThrowAndCatchException | 3,366,289.21 ns | 66,375.332 ns | 79,015.118 ns |  203.1250 |         - |         - |  320000 B |
| GarbageCollectionTest  |   107,114.73 ns |  2,061.475 ns |  2,531.677 ns | 1000.0000 | 1000.0000 | 1000.0000 |     171 B |
| ParallelFor            |   266,089.00 ns |    681.617 ns |    604.235 ns |    0.9766 |         - |         - |    2264 B |
| TasksWithWhenAll       |   216,796.76 ns |  2,697.630 ns |  2,391.379 ns |    0.9766 |         - |         - |    1577 B |
| ThreadPoolBenchmarks   |   418,913.97 ns |  1,764.390 ns |  1,473.346 ns |         - |         - |         - |     528 B |

### Aggregated Mean time (Smaller is better)

---

| Test                   | .NET 6         | .NET 8         | .NET 9       |
| ---------------------- | -------------- | -------------- | ------------ |
| LinqSum                | 7,525.14       | 109.33         | 107.95       |
| LinqOrderBySum         | 57,681.18      | 23,880.68      | 22,798.88    |
| ComputeSha256          | 50,267.66      | 50,304.02      | 50,927.69    |
| ComputeMd5             | 22,743.14      | 22,733.03      | 22,762.04    |
| CalculateFibonacci     | 6,380,681.08   | 3,401,143.61   | 3,602,688.88 |
| CalculatePrimes        | 181,976,522.22 | 178,487,111.11 | 365,074.52   |
| ReverseString          | 46.22          | 36.79          | 35.69        |
| ReverseStringUsingSpan | 39.7           | 28.06          | 27.44        |
| CountVowels            | 288.64         | 164.62         | 168.21       |
| CountVowelsUsingSpan   | 281.63         | 161.65         | 146.99       |
| JsonSerialization      | 314.87         | 260.34         | 244.58       |
| JsonDeserialization    | 785.04         | 715.33         | 504          |
| ThrowAndCatchException | 7,718,691.85   | 7,794,008.07   | 3,329,348.68 |
| GarbageCollectionTest  | 664,636.39     | 114,326.03     | 105,293.09   |
| ParallelFor            | 547,751.42     | 537,407.45     | 265,667.61   |
| TasksWithWhenAll       | 418,350.52     | 506,076.28     | 214,791.19   |
| ThreadPoolBenchmarks   | 503,761.47     | 414,715.03     | 215,313.97   |

## Questions

- How many of you in the audience are using .NET 9?
- Do you go LTS to LTS or do you upgrade Release?
- Upgrade experience is very good!
  - Most of the time, just the CsProj + NuGet packages
  - Performance testing code above all uses exactly the same code
- The quality of the library with LTS and STS releases is the same, it's down to internal factors

## References

- https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13
- https://learn.microsoft.com/en-us/dotnet/aspire/whats-new/dotnet-aspire-9?tabs=windows
- https://medium.com/@dkorolov1/must-know-new-c-13-and-net-9-features-a42e5592e65b
