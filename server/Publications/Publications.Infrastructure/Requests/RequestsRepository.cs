using Microsoft.EntityFrameworkCore;
using Publications.Application.Repositories;
using Publications.Domain.Requests;
using Publications.Domain.Shared;

namespace Publications.Infrastructure.Requests;

public class RequestsRepository: IRequestsRepository
{
    private readonly RequestsHistoryDbContext _dbContext;

    public RequestsRepository(RequestsHistoryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Request request)
    {
        await _dbContext.Requests.AddAsync(request);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task<Dictionary<int, int>> GetResourceViews<TResource>(
        DateTime? after = null,
        DateTime? before = null,
        bool distinct = true) 
        where TResource : Entity<TResource>
    {
        string resourceName = ResourceHelper.GetResourceName<TResource>();
        
        
        return await _dbContext.Requests
            .Where(r => r.ResourceName == resourceName)
            .GroupBy(r => r.ResourceId)
            .Select(group => new
            {
                ResourceId = group.Key,
                Views = distinct 
                    ? group.Select(r => r.SessionId).Distinct().Count() 
                    : group.Count()
            })
            .ToDictionaryAsync(k => k.ResourceId, v => v.Views);
    }
    public async Task<int> GetResourceRecentViewsCount(DateTime periodStart)
    {
        return await _dbContext.Requests
            .Where(r => r.RequestedAt >= periodStart)
            .CountAsync();
    }
}