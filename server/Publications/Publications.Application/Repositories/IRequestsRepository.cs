using Publications.Domain.Requests;
using Publications.Domain.Shared;

namespace Publications.Application.Repositories;

public interface IRequestsRepository
{
    Task AddAsync(Request request);
    
    Task<Dictionary<int, int>>  GetResourceViews<TResource>(bool distinct = true) 
        where TResource : Entity<TResource>;
    
    Task<Dictionary<int, int>> GetResourceRecentViews(DateTime periodStart);
    Task<int> GetResourceRecentViewsCount(DateTime periodStart);
}