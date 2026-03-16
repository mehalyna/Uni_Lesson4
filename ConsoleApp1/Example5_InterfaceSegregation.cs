namespace ConsoleApp1;

// Interface Segregation Principle (ISP) demonstration
// Clients should not be forced to depend on interfaces they don't use

public interface IReadable
{
    string Read(string identifier);
}

public interface IWritable
{
    void Write(string identifier, string content);
}

public interface IDeletable
{
    void Delete(string identifier);
}

public interface ISearchable
{
    List<string> Search(string query);
}

// Full-featured document manager implementing all interfaces
public class DocumentManager : IReadable, IWritable, IDeletable, ISearchable
{
    private readonly Dictionary<string, string> _documents = new();

    public string Read(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));

        if (!_documents.ContainsKey(identifier))
            throw new FileNotFoundException($"Document '{identifier}' not found");

        return _documents[identifier];
    }

    public void Write(string identifier, string content)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));

        if (content == null)
            throw new ArgumentNullException(nameof(content));

        _documents[identifier] = content;
    }

    public void Delete(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ArgumentException("Identifier cannot be null or empty", nameof(identifier));

        if (!_documents.Remove(identifier))
            throw new FileNotFoundException($"Document '{identifier}' not found");
    }

    public List<string> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            throw new ArgumentException("Query cannot be null or empty", nameof(query));

        return _documents
            .Where(kvp => kvp.Key.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                         kvp.Value.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(kvp => kvp.Key)
            .ToList();
    }
}

// Read-only document viewer implementing only IReadable
public class DocumentViewer : IReadable
{
    private readonly IReadable _documentSource;

    public DocumentViewer(IReadable documentSource)
    {
        _documentSource = documentSource ?? throw new ArgumentNullException(nameof(documentSource));
    }

    public string Read(string identifier)
    {
        try
        {
            return _documentSource.Read(identifier);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to read document: {ex.Message}", ex);
        }
    }
}

// Document editor implementing only IReadable and IWritable
public class DocumentEditor : IReadable, IWritable
{
    private readonly IReadable _reader;
    private readonly IWritable _writer;

    public DocumentEditor(IReadable reader, IWritable writer)
    {
        _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        _writer = writer ?? throw new ArgumentNullException(nameof(writer));
    }

    public string Read(string identifier) => _reader.Read(identifier);

    public void Write(string identifier, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Cannot write empty content", nameof(content));

        _writer.Write(identifier, content);
    }

    public void Append(string identifier, string additionalContent)
    {
        if (string.IsNullOrWhiteSpace(additionalContent))
            throw new ArgumentException("Cannot append empty content", nameof(additionalContent));

        try
        {
            string existingContent = Read(identifier);
            Write(identifier, existingContent + additionalContent);
        }
        catch (FileNotFoundException)
        {
            Write(identifier, additionalContent);
        }
    }
}

public static class Example5
{
    public static void Run()
    {
        Console.WriteLine("=== Example 5: Interface Segregation Principle ===");

        try
        {
            // Full-featured document manager
            var documentManager = new DocumentManager();

            // Add documents
            documentManager.Write("doc1.txt", "This is the first document");
            documentManager.Write("doc2.txt", "This is the second document");
            documentManager.Write("report.txt", "Annual report content");
            Console.WriteLine("Documents created successfully");

            // Read-only viewer (only needs IReadable)
            IReadable viewer = new DocumentViewer(documentManager);
            string content = viewer.Read("doc1.txt");
            Console.WriteLine($"Viewer read: {content}");

            // Document editor (needs IReadable and IWritable)
            var editor = new DocumentEditor(documentManager, documentManager);
            editor.Append("doc1.txt", " - Appended text");
            Console.WriteLine($"Editor appended text: {documentManager.Read("doc1.txt")}");

            // Search functionality
            ISearchable searcher = documentManager;
            List<string> results = searcher.Search("report");
            Console.WriteLine($"Search results for 'report': {string.Join(", ", results)}");

            // Delete functionality
            IDeletable deleter = documentManager;
            deleter.Delete("doc2.txt");
            Console.WriteLine("Document deleted successfully");

            // Test error: Read non-existent document
            try
            {
                viewer.Read("nonexistent.txt");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Test error: Delete non-existent document
            try
            {
                deleter.Delete("nonexistent.txt");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Test error: Empty search query
            try
            {
                searcher.Search("");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            // Test error: Append empty content
            try
            {
                editor.Append("doc1.txt", "");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Expected error: {ex.Message}");
            }

            Console.WriteLine("Example 5 completed successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in Example 5: {ex.Message}\n");
        }
    }
}
