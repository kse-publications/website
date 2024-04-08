using Microsoft.EntityFrameworkCore;
using Publications.API.Models;

namespace Publications.API.Repositories.Requests;

public class RequestsRepository: IRequestsRepository
{
    private readonly RequestsHistoryDbContext _dbContext;
    private readonly ILogger<RequestsRepository> _logger;

    public RequestsRepository(RequestsHistoryDbContext dbContext, ILogger<RequestsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task AddAsync(Request request)
    {
        _logger.LogInformation("Adding request to the database: {SessionId}, {ResourceName}, {ResourceId}",
            request.SessionId, request.ResourceName, request.ResourceId);
        
        // await _dbContext.Requests.AddAsync(request);
        // await _dbContext.SaveChangesAsync();
    }
    public async Task<Dictionary<int, int>> GetResourceViews(string resourceName)
    {
        return await _dbContext.Requests
            .Where(r => r.ResourceName == resourceName)
            .GroupBy(r => r.ResourceId)
            .Select(group => new { ResourceId = group.Key, Views = group.Count() })
            .ToDictionaryAsync(k => k.ResourceId, v => v.Views);
    }
}