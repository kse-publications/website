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
    
    public SearchRedisQuery Filter(string type)
    {
        if (type == string.Empty)
            return this;

        And(nameof(Publication.Type).EqualTo(type));
        return this;
    }
    
    public SearchRedisQuery OnlyVisible()
    {
        And($"{nameof(Publication.Visible)}".EqualTo("true"));
        return this;
    }
    
    public RedisQuery Build()
    {
        return Expression;
    }
}