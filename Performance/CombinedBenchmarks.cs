using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Collections.Generic;

// Combined benchmarks class
[MemoryDiagnoser]
public class CombinedBenchmarks
{

    // Constants
    private Random random = new Random(42);
    private const int N = 10000;
    private const int NumberOfIterations = 1000;
    private const string SampleText = "The quick brown fox jumps over the lazy dog.";
    private byte[] data;
    private readonly SHA256 sha256 = SHA256.Create();
    private readonly MD5 md5 = MD5.Create();
    private List<int> numbers;

    [GlobalSetup]
    public void Setup()
    {
        data = new byte[N];
        random.NextBytes(data);
        numbers = Enumerable.Range(1, 1_000).ToList(); 
        Console.WriteLine($"Setup complete. Numbers count: {numbers.Count}");
    }

    // LINQ benchmarks
    [Benchmark]
    public int LinqSum() => numbers.Sum();

    [Benchmark]
    public int LinqOrderBySum() => numbers.OrderBy(x => x).Sum();



    // Hashing benchmarks
    [Benchmark]
    public byte[] ComputeSha256() => sha256.ComputeHash(data);

    [Benchmark]
    public byte[] ComputeMd5() => md5.ComputeHash(data);

    // Math benchmarks
    [Benchmark]
    public int CalculateFibonacci() => Fibonacci(30);

    [Benchmark]
    public int CalculatePrimes() => CountPrimes(10_000);

    private int Fibonacci(int n)
    {
        if (n <= 1) return n;
        return Fibonacci(n - 1) + Fibonacci(n - 2);
    }

    private int CountPrimes(int max)
    {
        int count = 0;
        for (int i = 2; i <= max; i++)
        {
            if (IsPrime(i)) count++;
        }
        return count;
    }

    private bool IsPrime(int number)
    {
        if (number < 2) return false;
        for (int i = 2; i * i <= number; i++)
        {
            if (number % i == 0) return false;
        }
        return true;
    }

    // Text benchmarks
    [Benchmark]
    public string ReverseString()
    {
        char[] charArray = SampleText.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    [Benchmark]
    public string ReverseStringUsingSpan()
    {
        Span<char> charSpan = stackalloc char[SampleText.Length];
        SampleText.AsSpan().CopyTo(charSpan);
        charSpan.Reverse();
        return new string(charSpan);
    }

    [Benchmark]
    public int CountVowels()
    {
        int count = 0;
        foreach (char c in SampleText)
        {
            if ("aeiouAEIOU".Contains(c)) count++;
        }
        return count;
    }

    [Benchmark]
    public int CountVowelsUsingSpan()
    {
        ReadOnlySpan<char> vowels = "aeiouAEIOU".AsSpan();
        int count = 0;
        foreach (char c in SampleText)
        {
            if (vowels.Contains(c)) count++;
        }
        return count;
    }


    // JSON serialization/deserialization
    private readonly string jsonSample = "{\"Name\":\"John\",\"Age\":30}";
    private readonly System.Text.Json.JsonSerializerOptions jsonOptions = new();

    [Benchmark]
    public string JsonSerialization()
    {
        var obj = new { Name = "John", Age = 30 };
        return System.Text.Json.JsonSerializer.Serialize(obj, jsonOptions);
    }

    [Benchmark]
    public object JsonDeserialization()
    {
        return System.Text.Json.JsonSerializer.Deserialize(jsonSample, typeof(object), jsonOptions);
    }

    [Benchmark]
    public void ThrowAndCatchException()
    {
        for (int i = 0; i < NumberOfIterations; i++)
        {
            try
            {
                ThrowException();
            }
            catch
            {
                // Exception caught - the cost of this is what we're measuring
            }
        }
    }

    private void ThrowException()
    {
        throw new System.Exception("This is a test exception.");
    }

    // Garbage Collection
    [Benchmark]
    public void GarbageCollectionTest()
    {
        for (int i = 0; i < N; i++)
        {
            _ = new byte[1024]; // Allocate memory
        }
        GC.Collect(); // Force a GC
    }

    // Multithreaded benchmarks
    private const int TaskCount = 10;
    private const int IterationsPerTask = 100_000;

    [Benchmark]
    public void ParallelFor()
    {
        Parallel.For(0, TaskCount, _ =>
        {
            DoWork(IterationsPerTask);
        });
    }

    [Benchmark]
    public void TasksWithWhenAll()
    {
        var tasks = new Task[TaskCount];
        for (int i = 0; i < TaskCount; i++)
        {
            tasks[i] = Task.Run(() => DoWork(IterationsPerTask));
        }

        Task.WaitAll(tasks);
    }

    [Benchmark]
    public void ThreadPoolBenchmarks()
    {
        using var countdownEvent = new CountdownEvent(TaskCount);
        for (int i = 0; i < TaskCount; i++)
        {
            ThreadPool.QueueUserWorkItem(_ =>
            {
                DoWork(IterationsPerTask);
                countdownEvent.Signal();
            });
        }

        countdownEvent.Wait();
    }

    private void DoWork(int iterations)
    {
        int result = 0;
        for (int i = 0; i < iterations; i++)
        {
            result += (i % 2 == 0) ? 1 : -1; // Simulates computational work
        }
    }
}
