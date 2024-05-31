namespace Publications.Domain.Publications;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
public class Author
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}