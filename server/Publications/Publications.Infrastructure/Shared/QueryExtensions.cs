using NRedisStack.Search;
using NRedisStack.Search.Aggregation;

namespace Publications.Infrastructure.Shared;

internal static class QueryExtensions
{
    internal static Query Paginate(this Query query,
        int page, int pageSize)
    {
        return query.Limit(
            offset: pageSize * (page - 1),
            count: pageSize);
    }
    
    internal static AggregationRequest Paginate(this AggregationRequest request,
        int page, int pageSize)
    {
        return request.Limit(
            offset: pageSize * (page - 1),
            count: pageSize);
    }
}