using System.Text.Json.Serialization;

namespace Publications.Domain.Publications;

/// <summary>
/// Represents a publisher of a <see cref="Publication"/>. 
/// </summary>
public class Publisher
{
    public int Id { get; init; }
    public string Name { get; init; }
    
    [JsonConstructor]
    public Publisher(int id, string name)
    {
        Id = id;
        Name = name;
    }
}