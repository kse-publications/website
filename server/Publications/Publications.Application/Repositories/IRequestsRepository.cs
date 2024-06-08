using Publications.Domain.Requests;
using Publications.Domain.Shared;

namespace Publications.Application.Repositories;

public interface IRequestsRepository
{
    Task AddAsync(Request request);
    
    Task<Dictionary<int, int>>  GetResourceViews<TResource>(bool distinct = true) 
        where TResource : Entity;
}