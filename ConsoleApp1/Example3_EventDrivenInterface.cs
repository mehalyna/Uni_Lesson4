namespace ConsoleApp1;

// Interface with events and delegates
public interface INotificationService
{
    event EventHandler<NotificationEventArgs>? NotificationSent;
    event EventHandler<NotificationErrorEventArgs>? NotificationFailed;

    void SendNotification(string recipient, string message);
    int GetSentCount();
}

public class NotificationEventArgs : EventArgs
{
    public string Recipient { get; }
    public string Message { get; }
    public DateTime Timestamp { get; }

    public NotificationEventArgs(string recipient, string message)
    {
        Recipient = recipient;
        Message = message;
        Timestamp = DateTime.Now;
    }
}

public class NotificationErrorEventArgs : EventArgs
{
    public string Recipient { get; }
    public string Error { get; }
    public DateTime Timestamp { get; }

    public NotificationErrorEventArgs(string recipient, string error)
    {
        Recipient = recipient;
        Error = error;
        Timestamp = DateTime.Now;
    }
}

public class EmailNotificationService : INotificationService
{
    private int _sentCount;
    private readonly List<string> _blacklist = new() { "blocked@example.com" };

    public event EventHandler<NotificationEventArgs>? NotificationSent;
    public event EventHandler<NotificationErrorEventArgs>? NotificationFailed;

    public void SendNotification(string recipient, string message)
    {
        if (string.IsNullOrWhiteSpace(recipient))
            throw new ArgumentException("Recipient cannot be null or empty", nameof(recipient));

        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be null or empty", nameof(message));

        try
        {
            // Validate email format
            if (!recipient.Contains('@'))
            {
                throw new FormatException("Invalid email format");
            }

            // Check blacklist
            if (_blacklist.Contains(recipient))
            {
                throw new InvalidOperationException("Recipient is blacklisted");
            }

            // Simulate sending email
            Console.WriteLine($"  -> Sending email to {recipient}");
            _sentCount++;

            // Raise success event
            NotificationSent?.Invoke(this, new NotificationEventArgs(recipient, message));
        }
        catch (Exception ex)
        {
            // Raise failure event
            NotificationFailed?.Invoke(this, new NotificationErrorEventArgs(recipient, ex.Message));
            throw;
        }
    }

    public int GetSentCount() => _sentCount;
}

public static class Example3
{
    public static void Run()
    {
        Console.WriteLine("=== Example 3: Event-Driven Interface ===");

        try
        {
            INotificationService notificationService = new EmailNotificationService();

            // Subscribe to events
            notificationService.NotificationSent += (sender, args) =>
            {
                Console.WriteLine($"✓ Notification sent to {args.Recipient} at {args.Timestamp:HH:mm:ss}");
            };

            notificationService.NotificationFailed += (sender, args) =>
            {
                Console.WriteLine($"✗ Failed to send to {args.Recipient}: {args.Error}");
            };

            // Send successful notifications
            notificationService.SendNotification("user1@example.com", "Welcome message");
            notificationService.SendNotification("user2@example.com", "Confirmation message");

            // Test error: Invalid email format
            try
            {
                notificationService.SendNotification("invalid-email", "This should fail");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Expected error caught: {ex.Message}");
            }

            // Test error: Blacklisted recipient
            try
            {
                notificationService.SendNotification("blocked@example.com", "This should fail");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Expected error caught: {ex.Message}");
            }

            // Test error: Empty recipient
            try
            {
                notificationService.SendNotification("", "This should fail");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Expected error caught: {ex.Message}");
            }

            Console.WriteLine($"Total successful notifications: {notificationService.GetSentCount()}");
            Console.WriteLine("Example 3 completed successfully!\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected error in Example 3: {ex.Message}\n");
        }
    }
}
