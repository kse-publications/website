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
        Expression.QueryText = RedisQueryExtensions.Either(Expression.QueryText, subquery);
        return this;
    }

    public SearchRedisQuery And(string subquery)
    {
        Expression.QueryText = RedisQueryExtensions.Both(Expression.QueryText, subquery);
        return this;
    }
    
    public RedisQuery Build()
    {
        return Expression;
    }
}