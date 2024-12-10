using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("Hello, Params Demo!");
        // Using params with an array - this worked for Arrays already
        PrintNumbers(1, 2, 3, 4, 5);

        // Using params with ReadOnlySpan<T> - now works with ReadOnlySpan<T>
        PrintNumbersWithSpan(6, 7, 8, 9, 10);

        // Using params with IEnumerable<T>
        PrintNumbersWithIEnumerable(11, 12, 13, 14, 15);
    }

    // Method using params with an array
    static void PrintNumbers(params int[] numbers)
    {
        Console.WriteLine("Using params with array:");
        foreach (var number in numbers)
        {
            Console.WriteLine(number);
        }
    }

    // Method using params with ReadOnlySpan<T>
    static void PrintNumbersWithSpan(params ReadOnlySpan<int> numbers)
    {
        Console.WriteLine("\nUsing params with ReadOnlySpan<T>:");
        foreach (var number in numbers)
        {
            Console.WriteLine(number);
        }
    }

    // Method using params with IEnumerable<T>
    static void PrintNumbersWithIEnumerable(params IEnumerable<int> numbers)
    {
        Console.WriteLine("\nUsing params with IEnumerable<T>:");
        foreach (var number in numbers)
        {
            Console.WriteLine(number);
        }
    }
}
