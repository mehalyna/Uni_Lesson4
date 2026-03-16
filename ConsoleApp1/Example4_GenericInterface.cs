namespace ConsoleApp1;

// Generic interface demonstrating type-safe operations
public interface ICache<TKey, TValue> where TKey : notnull
{
    void Set(TKey key, TValue value, TimeSpan? expiration = null);
    TValue Get(TKey key);
    bool TryGet(TKey key, out TValue? value);
    void Remove(TKey key);
    void Clear();
    int Count { get; }
}

public class CacheEntry<TValue>
{
    public TValue Value { get; }
    public DateTime? ExpiresAt { get; }

    public CacheEntry(TValue value, DateTime? expiresAt)
    {
        Value = value;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => ExpiresAt.HasValue && DateTime.Now > ExpiresAt.Value;
}

public class MemoryCache<TKey, TValue> : ICache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, CacheEntry<TValue>> _cache = new();
    private readonly object _lock = new();

    public int Count
    {
        get
        {
            lock (_lock)
            {
                CleanExpiredEntries();
                return _cache.Count;
            }
        }
    }

    public void Set(TKey key, TValue value, TimeSpan? expiration = null)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        lock (_lock)
        {
            DateTime? expiresAt = expiration.HasValue
                ? DateTime.Now.Add(expiration.Value)
                : null;

            _cache[key] = new CacheEntry<TValue>(value, expiresAt);
        }
    }

    public TValue Get(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        lock (_lock)
        {
            if (!_cache.TryGetValue(key, out var entry))
                throw new KeyNotFoundException($"Key '{key}' not found in cache");

            if (entry.IsExpired)
            {
                _cache.Remove(key);
                throw new InvalidOperationException($"Cache entry for key '{key}' has expired");
            }

            return entry.Value;
        }
    }

    public bool TryGet(TKey key, out TValue? value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        lock (_lock)
        {
            if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
            {
                value = entry.Value;
                return true;
            }

            if (_cache.ContainsKey(key))
            {
                _cache.Remove(key); // Remove expired entry
            }

            value = default;
            return false;
        }
    }

    public void Remove(TKey key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        lock (_lock)
        {
            if (!_cache.Remove(key))
                throw new KeyNotFoundException($"Key '{key}' not found in cache");
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            _cache.Clear();
        }
    }

    private void CleanExpiredEntries()
    {
        var expiredKeys = _cache
            .Where(kvp => kvp.Value.IsExpired)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            _cache.Remove(key);
        }
    }
}

public static class Example4
{
    public static void Run()
    {
        Console.WriteLine("=== Example 4: Generic Interface (Cache) ===");

        try
        {
            // String cache
            ICache<string, string> stringCache = new MemoryCache<string, string>();
            stringCache.Set("user:1", "John Doe");
            stringCache.Set("user:2", "Jane Smith", TimeSpan.FromSeconds(2));
            Console.WriteLine($"String cache - User 1: {stringCache.Get("user:1")}");
            Console.WriteLine($"String cache count: {stringCache.Count}");

            // Integer cache
            ICache<int, decimal> priceCache = new MemoryCache<int, decimal>();
            priceCache.Set(101, 29.99m);
            priceCache.Set(102, 49.99m);
            priceCache.Set(103, 19.99m);
            Console.WriteLine($"Price cache - Product 102: ${priceCache.Get(102)}");

            // Test TryGet
            if (priceCache.TryGet(101, out var price))
            {
                Console.WriteLine($"Price found: ${price}");
            }

            // Test error: Key not found
            try
            {
                stringCache.Get("user:999");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Test error: Expired entry
            Console.WriteLine("Waiting for cache entry to expire...");
            Thread.Sleep(2100); // Wait for user:2 to expire
            try
            {
                stringCache.Get("user:2");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Test TryGet on expired entry
            if (!stringCache.TryGet("user:2", out var expiredValue))
            {
                Console.WriteLine("Expired entry not found using TryGet (as expected)");
            }

            // Test error: Null key
            try
            {
                stringCache.Set(null!, "value");
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Expected error: {ex.GetType().Name}");
            }

            // Clear cache
            stringCache.Clear();
            Console.WriteLine($"String cache cleared. Count: {stringCache.Count}");

            Console.WriteLine("Example 4 completed successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in Example 4: {ex.Message}\n");
        }
    }
}
