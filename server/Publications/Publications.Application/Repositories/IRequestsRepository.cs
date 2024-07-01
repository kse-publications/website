using Publications.Domain.Requests;
using Publications.Domain.Shared;

namespace Publications.Application.Repositories;

public interface IRequestsRepository
{
    Task AddAsync(Request request);
    
    Task<Dictionary<int, int>> GetRequestsDistributionAsync<TResource>(
        DateTime? after = null,
        DateTime? before = null,
        bool distinct = true) where TResource : Entity;
    
    Task<int> GetRequestsCountAsync<TResource>(
        DateTime? after = null,
        DateTime? before = null,
        bool distinct = true) where TResource : Entity;
}