using System.Text.Json.Serialization;
using Publications.Domain.Shared.Serialization;
using Redis.OM.Modeling;

namespace Publications.Domain.Shared;

public abstract class Entity
{
    [RedisIdField]
    [Indexed(Sortable = true)]
    public int Id { get; init; }
    
    [Indexed(Sortable = true)]
    [JsonInclude]
    public string Slug { get; set; } = string.Empty;
    
    [Indexed]
    [IgnoreInResponse]
    [JsonInclude]
    public DateTime LastSynchronizedAt { get; set; }
    
    [Indexed]
    [IgnoreInResponse]
    [JsonInclude]
    public DateTime LastModifiedAt { get; set; }
}