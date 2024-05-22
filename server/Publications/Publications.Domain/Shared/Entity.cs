using System.Text.Json.Serialization;
using Publications.Domain.Shared.Attributes;
using Redis.OM.Modeling;

namespace Publications.Domain.Shared;

public abstract class Entity<T> where T: Entity<T>
{
    [RedisIdField]
    [Indexed(Sortable = true)]
    public int Id { get; init; }

    [Indexed(Sortable = true)]
    public string Slug { get; set; } = string.Empty;
    
    [Indexed]
    [IgnoreInResponse]
    [JsonConverter(typeof(UnixTimestampJsonConverter))]
    public DateTime SynchronizedAt { get; set; }

    public T Synchronize()
    {
        SynchronizedAt = DateTime.UtcNow;
        return (T)this;
    }
    
    public static string IndexName => $"{typeof(T).Name.ToLower()}-idx";
}