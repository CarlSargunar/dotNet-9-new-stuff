internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, New Linq Methods!");
        Console.WriteLine();        

        // Make up some students
        Student[] students = [
            new("Alice", "A"),
            new("Bob", "B"),
            new("Charlie", "C"),
            new("David", "B"),
            new("Eve", "A"),
            new("Frank", "F"),
            new("Grace", "A"),
            new("Hank", "B"),
            new("Ivy", "C"),
            new("Jack", "A")
        ];

        // Index
        Console.WriteLine("Index : ");
        
        // Index Old Way - lambda function
        var studentsWithIndexOld = students.Select((student, index) => (index, student));
        foreach (var (index, student) in studentsWithIndexOld)
        {
            Console.WriteLine($"Student Index (Old Way) {index}: {student.Name} - {student.Score}");
        }

        Console.WriteLine();


        // Index New Way
        var studentsWithIndex = students.Index();

        foreach (var (index, student) in studentsWithIndex)
        {
            Console.WriteLine($"Student {index}: {student.Name} - {student.Score}");
        }
        Console.WriteLine();        




        Console.WriteLine();        
        Console.WriteLine("CountBy : ");

        // CountBy
        // Old Way - multiple operations
        var studentsByScoreOld = students.GroupBy(student => student.Score)
            .Select(group => (group.Key, group.Count()));

        // Output the count of students by score    
        foreach (var (score, count) in studentsByScoreOld)
        {
            Console.WriteLine($"Students with score (Old Way) {score}: {count}");
        }        

        Console.WriteLine();        

        // New Way
        var studentsByScore = students.CountBy(student => student.Score);

        // Output the count of students by score
        foreach (var (score, count) in studentsByScore)
        {
            Console.WriteLine($"Students with score {score}: {count}");
        }

        Console.WriteLine();

        //Aggregate By
        Console.WriteLine("AggregateBy : ");
        // Old Way
        var studentsByScoreAggregateOld = students
            .GroupBy(student => student.Score)
            .Select(group => (group.Key, group.Select(student => student.Name).ToList()));

        foreach (var (score, studentGroup) in studentsByScoreAggregateOld)
        {
            Console.WriteLine($"Students with a {score}-score (Old Way): {string.Join(", ", studentGroup)}");
        }        


        Console.WriteLine();
        
        // New Way
        IEnumerable<KeyValuePair<string, List<string>>> studentsByScoreAggregate = students
        .AggregateBy(
            keySelector: student => student.Score,
            seed: new List<string>(),
            func: (group, student) => [..group, student.Name]
        );
 
        foreach (var (score, studentGroup) in studentsByScoreAggregate)
        {
            Console.WriteLine($"Students with a {score}-score: {string.Join(", ", studentGroup)}");
        }
    }
}

record Student(string Name, string Score);
