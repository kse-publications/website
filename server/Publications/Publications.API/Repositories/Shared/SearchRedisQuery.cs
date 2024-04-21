using Publications.API.DTOs;
using Publications.API.Models;
using Redis.OM.Searching.Query;

namespace Publications.API.Repositories.Shared;

public class SearchRedisQuery
{
    public RedisQuery Expression { get; set; }
    
    public SearchRedisQuery(RedisQuery query)
    {
        Expression = query;
    }
    
    public SearchRedisQuery Or(string subquery)
    {
        Expression.QueryText = $"({Expression.QueryText} | {subquery})";
        return this;
    }

    public SearchRedisQuery And(string subquery)
    {
        Expression.QueryText = $"{Expression.QueryText} {subquery}";
        return this;
    }
    
    public SearchRedisQuery Filter(PublicationsPaginationFilterDTO filterDTO)
    {
        if (filterDTO.Year != default)
            And(nameof(Publication.Year).EqualTo(filterDTO.Year.ToString()));
        
        if (!string.IsNullOrWhiteSpace(filterDTO.Type))
            And(nameof(Publication.Type).EqualTo(filterDTO.Type));
        
        if (!string.IsNullOrWhiteSpace(filterDTO.Language))
            And(nameof(Publication.Language).EqualTo(filterDTO.Language));
        
        return this;
    }
    
    public RedisQuery Build()
    {
        return Expression;
    }
}