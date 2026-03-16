namespace ConsoleApp1;

// Basic interface demonstrating CRUD operations
public interface IRepository
{
    void Add(string item);
    string Get(int id);
    void Update(int id, string item);
    void Delete(int id);
    int Count();
}

public class InMemoryRepository : IRepository
{
    private readonly Dictionary<int, string> _storage = new();
    private int _currentId = 1;

    public void Add(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Item cannot be null or empty", nameof(item));

        _storage[_currentId++] = item;
    }

    public string Get(int id)
    {
        if (!_storage.ContainsKey(id))
            throw new KeyNotFoundException($"Item with ID {id} not found");

        return _storage[id];
    }

    public void Update(int id, string item)
    {
        if (!_storage.ContainsKey(id))
            throw new KeyNotFoundException($"Item with ID {id} not found");

        if (string.IsNullOrWhiteSpace(item))
            throw new ArgumentException("Item cannot be null or empty", nameof(item));

        _storage[id] = item;
    }

    public void Delete(int id)
    {
        if (!_storage.Remove(id))
            throw new KeyNotFoundException($"Item with ID {id} not found");
    }

    public int Count() => _storage.Count;
}

public static class Example1
{
    public static void Run()
    {
        Console.WriteLine("=== Example 1: Basic Repository Pattern ===");
        IRepository repository = new InMemoryRepository();

        try
        {
            // Add items
            repository.Add("First Item");
            repository.Add("Second Item");
            repository.Add("Third Item");
            Console.WriteLine($"Added 3 items. Total count: {repository.Count()}");

            // Get item
            string item = repository.Get(2);
            Console.WriteLine($"Retrieved item with ID 2: {item}");

            // Update item
            repository.Update(2, "Updated Second Item");
            Console.WriteLine($"Updated item with ID 2: {repository.Get(2)}");

            // Delete item
            repository.Delete(1);
            Console.WriteLine($"Deleted item with ID 1. Remaining count: {repository.Count()}");

            // Intentional error: try to get deleted item
            try
            {
                repository.Get(1);
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Intentional error: try to add null item
            try
            {
                repository.Add("");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            Console.WriteLine("Example 1 completed successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in Example 1: {ex.Message}\n");
        }
    }
}
