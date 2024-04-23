using Publications.API.Serialization;
using Redis.OM.Modeling;

namespace Publications.API.Models;

[Document(IndexName = "filtergroup-idx", StorageType = StorageType.Json, Prefixes = ["filter-group"])]
public class FilterGroup
{
    [RedisIdField]
    [Indexed]
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    [IgnoreInResponse]
    public string ResourceName { get; set; } = string.Empty;

    public Filter[] Filters { get; set; } = Array.Empty<Filter>();
}

