using Publications.API.Models;

namespace Publications.API.Repositories.Requests;

public interface IRequestsRepository
{
    Task AddAsync(Request request);
    
    Task<Dictionary<int, int>>  GetResourceViews<TResource>(bool distinct = true) 
        where TResource : Entity<TResource>;
}