using Publications.API.Models;

namespace Publications.API.Repositories.Requests;

public interface IRequestsRepository
{
    Task AddAsync(Request request);
    
    Task<Dictionary<int, int>> GetResourceViews<TResource>() 
        where TResource : Entity<TResource>;
    
    Task<Dictionary<int, int>> GetResourceDistinctViews<TResource>() 
        where TResource : Entity<TResource>;
}