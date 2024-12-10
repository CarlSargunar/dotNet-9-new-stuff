using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting benchmarks...");
        BenchmarkRunner.Run<CombinedBenchmarks>();
        Console.WriteLine("Benchmarks completed.");
    }
}
