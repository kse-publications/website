using System.Text.Json.Serialization;
using Redis.OM.Modeling;

namespace Publications.API.Models;

public abstract class Entity<T> where T: Entity<T>
{
    [JsonIgnore]
    [RedisIdField]
    [Indexed]
    public int Id { get; set; }
    
    [JsonIgnore]
    public Guid NotionId { get; set; }
    
    public string Slug { get; set; } = string.Empty;
    
    public abstract T UpdateSlug();
}