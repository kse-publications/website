using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Domain.Shared.Slugs;
using Publications.Domain.Shared.ValueObjects;
using Redis.OM.Modeling;

namespace Publications.Domain.Publishers;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
public class Publisher
{
    public int Id { get; init; }
    public string Name { get; init; } = null!;
}