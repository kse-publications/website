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

    public async Task<Dictionary<int, int>> GetRequestsDistributionAsync<TResource>(
        DateTime? after = null,
        DateTime? before = null,
        bool distinct = true) where TResource : Entity
    {
        IQueryable<Request> query = ApplyResourceNameFilter<TResource>(_dbContext.Requests);
        query = ApplyDateRangeFilter(query, after, before);

        return await query
            .GroupBy(r => r.ResourceId)
            .Select(group => new
            {
                ResourceId = group.Key,
                Requests = distinct 
                    ? group.Select(r => r.SessionId).Distinct().Count() 
                    : group.Count()
            })
            .ToDictionaryAsync(k => k.ResourceId, v => v.Requests);
    }
    
    public async Task<int> GetRequestsCountAsync<TResource>(
        DateTime? after = null, 
        DateTime? before = null, 
        bool distinct = true) where TResource : Entity
    {
        IQueryable<Request> query = ApplyResourceNameFilter<TResource>(_dbContext.Requests);
        query = ApplyDateRangeFilter(query, after, before);

        return distinct 
            ? await query.Select(r => r.SessionId).Distinct().CountAsync() 
            : await query.CountAsync();
    }
    
    private IQueryable<Request> ApplyResourceNameFilter<TResource>(IQueryable<Request> query)
    {
        string resourceName = ResourceHelper.GetResourceName<TResource>();
        return query.Where(r => r.ResourceName == resourceName);
    }
    
    private IQueryable<Request> ApplyDateRangeFilter(IQueryable<Request> query, DateTime? after, DateTime? before)
    {
        if (after.HasValue && before.HasValue && after.Value > before.Value)
        {
            throw new ArgumentException("After date cannot be greater than before date.");
        }

        if (after.HasValue)
        {
            query = query.Where(r => r.RequestedAt >= after.Value);
        }

        if (before.HasValue)
        {
            query = query.Where(r => r.RequestedAt <= before.Value);
        }
        
        return query;
    }
}