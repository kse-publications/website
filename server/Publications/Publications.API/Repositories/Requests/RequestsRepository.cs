using Microsoft.EntityFrameworkCore;
using Publications.API.Models;

namespace Publications.API.Repositories.Requests;

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
    
    public async Task<Dictionary<int, int>> GetResourceViews<TResource>() 
        where TResource : Entity<TResource>
    {
        string resourceName = GetResourceName<TResource>();
        
        return await _dbContext.Requests
            .Where(r => r.ResourceName == resourceName)
            .GroupBy(r => r.ResourceId)
            .Select(group => new
            {
                ResourceId = group.Key,
                Views = group.Count()
            })
            .ToDictionaryAsync(k => k.ResourceId, v => v.Views);
    }
    
    public async Task<Dictionary<int, int>> GetResourceDistinctViews<TResource>() 
        where TResource : Entity<TResource>
    {
        string resourceName = GetResourceName<TResource>();
        
        return await _dbContext.Requests
            .Where(r => r.ResourceName == resourceName)
            .GroupBy(r => r.ResourceId)
            .Select(group => new
            {
                ResourceId = group.Key, 
                Views = group
                    .Select(r => r.SessionId)
                    .Distinct()
                    .Count()
            })
            .ToDictionaryAsync(k => k.ResourceId, v => v.Views);
    }
    
    private static string GetResourceName<TResource>()
    {
        return typeof(TResource).Name.ToLower() + "s";
    }
}