namespace ConsoleApp1;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║   Interface Usage Demo - .NET 8.0 Console Application         ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.WriteLine();

        try
        {
            // Example 1: Basic Repository Pattern
            Example1.Run();

            // Example 2: Multiple Interface Implementation
            Example2.Run();

            // Example 3: Event-Driven Interface
            Example3.Run();

            // Example 4: Generic Interface (Cache)
            Example4.Run();

            // Example 5: Interface Segregation Principle
            Example5.Run();

            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   All Examples Completed Successfully!                         ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║   CRITICAL ERROR                                               ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.WriteLine($"Error Type: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
            Environment.Exit(1);
        }
        finally
        {
            try
            {
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (InvalidOperationException)
            {
                // Console input is redirected, skip ReadKey
            }
        }
    }
}
