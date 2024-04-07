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
}