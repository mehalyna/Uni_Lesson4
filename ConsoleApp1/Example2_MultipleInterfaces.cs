namespace ConsoleApp1;

// Multiple interface implementation
public interface ILogger
{
    void Log(string message);
}

public interface IDisposable
{
    void Dispose();
}

public interface IConfigurable
{
    void Configure(Dictionary<string, string> settings);
    string GetConfiguration(string key);
}

public class FileLogger : ILogger, IDisposable, IConfigurable
{
    private StreamWriter? _writer;
    private Dictionary<string, string> _config = new();
    private bool _disposed;

    public void Configure(Dictionary<string, string> settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        _config = settings;

        if (!_config.ContainsKey("FilePath"))
            throw new InvalidOperationException("FilePath configuration is required");
    }

    public string GetConfiguration(string key)
    {
        if (!_config.ContainsKey(key))
            throw new KeyNotFoundException($"Configuration key '{key}' not found");

        return _config[key];
    }

    public void Log(string message)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileLogger));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        try
        {
            if (_writer == null)
            {
                string filePath = GetConfiguration("FilePath");
                _writer = new StreamWriter(filePath, append: true);
            }

            _writer.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
            _writer.Flush();
        }
        catch (IOException ex)
        {
            throw new InvalidOperationException($"Failed to write log: {ex.Message}", ex);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _writer?.Dispose();
            _disposed = true;
        }
    }
}

public static class Example2
{
    public static void Run()
    {
        Console.WriteLine("=== Example 2: Multiple Interface Implementation ===");

        FileLogger? logger = null;

        try
        {
            logger = new FileLogger();

            // Configure the logger
            var config = new Dictionary<string, string>
            {
                { "FilePath", "application.log" },
                { "LogLevel", "Info" }
            };
            logger.Configure(config);
            Console.WriteLine("Logger configured successfully");

            // Get configuration
            string filePath = logger.GetConfiguration("FilePath");
            Console.WriteLine($"Log file path: {filePath}");

            // Log messages
            logger.Log("Application started");
            logger.Log("Processing data");
            logger.Log("Application completed");
            Console.WriteLine("Messages logged successfully");

            // Test error: Get non-existent config
            try
            {
                logger.GetConfiguration("NonExistentKey");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Test error: Log after dispose
            logger.Dispose();
            try
            {
                logger.Log("This should fail");
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"Expected error: {ex.GetType().Name}");
            }

            Console.WriteLine("Example 2 completed successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in Example 2: {ex.Message}\n");
        }
        finally
        {
            logger?.Dispose();
        }
    }
}
