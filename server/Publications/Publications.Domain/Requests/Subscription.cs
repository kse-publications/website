

public class Subscription
{
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }

    // EF Core constructor
    private Subscription()
    {
    }

    public Subscription(string email)
    {
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
}