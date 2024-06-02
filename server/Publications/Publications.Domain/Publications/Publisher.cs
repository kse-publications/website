namespace Publications.Domain.Publications;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
public class Publisher
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}