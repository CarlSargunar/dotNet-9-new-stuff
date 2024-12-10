record DataRow(Guid Id, string Data);

class Program
{
    static void Main(string[] args)
    {

        // Demo
        Console.WriteLine("Hello, New UUID!");
        Console.WriteLine("Guids look like this : " + Guid.NewGuid());
        Console.WriteLine("UUID7 looks like this : " + Guid.CreateVersion7());


        // Build Data
        Console.WriteLine("Inserting rows...");
        var dataRows = new List<DataRow>();

        for (int i = 0; i < 5; i++)
        {
            var myId = Guid.CreateVersion7(); 
            var data = $"Inserted {i}";
            dataRows.Add(new DataRow(myId, data));

            Console.WriteLine($"Id: {myId}, Data: {data}");
            Thread.Sleep(50); // Simulate slight delay
        }

        Console.WriteLine("\nQuerying rows (Descending Order)...");
        var reverseRows = dataRows.OrderByDescending(row => row.Id);
        foreach (var row in reverseRows)
        {
            Console.WriteLine($"Id: {row.Id}, Data: {row.Data}");
        }
    }
}
