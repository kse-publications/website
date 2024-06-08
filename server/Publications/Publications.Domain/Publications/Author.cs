using System.Text.Json.Serialization;
using Publications.Domain.Shared.Serialization;

namespace Publications.Domain.Publications;

/// <summary>
/// Represents an author of a <see cref="Publication"/>.
/// </summary>
public class Author
{
    public int Id { get; init; }
    public string Name { get; init; }
    
    [IgnoreInResponse]
    [JsonIgnore]
    public DateTime LastModifiedAt { get; set; }
    
    [JsonConstructor]
    public Author(int id, string name)
    {
        Id = id;
        Name = name;
    }
}