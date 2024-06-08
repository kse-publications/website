using System.Text.Json.Serialization;
using Publications.Domain.Shared.Serialization;
using Publications.Domain.Shared.Slugs;
using Redis.OM.Modeling;

namespace Publications.Domain.Shared;

public abstract class Entity<T> where T: Entity<T>
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
    [JsonConverter(typeof(UnixTimestampJsonConverter))]
    public DateTime LastSynchronizedAt { get; set; }
    
    [Indexed]
    [IgnoreInResponse]
    [JsonConverter(typeof(UnixTimestampJsonConverter))]
    public DateTime LastModifiedAt { get; set; }
    
    public static string IndexName => $"{typeof(T).Name.ToLower()}-idx";
}