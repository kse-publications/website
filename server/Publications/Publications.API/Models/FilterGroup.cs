﻿using Publications.API.Serialization;
using Redis.OM.Modeling;

namespace Publications.API.Models;

[Document(IndexName = "filtergroup-idx", StorageType = StorageType.Json, Prefixes = ["filter-group"])]
public class FilterGroup
{
    [RedisIdField]
    [Indexed]
    public int Id { get; init; }
    
    public string Name { get; init; } = null!;
    
    [IgnoreInResponse]
    public string ResourceName { get; set; } = string.Empty;

    public Filter[] Filters { get; set; } = Array.Empty<Filter>();
}

