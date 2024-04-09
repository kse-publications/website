using Coravel.Invocable;
using Publications.API.Models;
using Publications.API.Repositories.Requests;

namespace Publications.API.BackgroundJobs;

public class StoreRequestAnalyticsTask: IInvocable, IInvocableWithPayload<Request>
{
    private readonly IRequestsRepository _requestsRepository;
    private readonly ILogger<StoreRequestAnalyticsTask> _logger;
    
    public Request Payload { get; set; } = null!;

    public StoreRequestAnalyticsTask(
        IRequestsRepository requestsRepository, 
        ILogger<StoreRequestAnalyticsTask> logger)
    {
        _requestsRepository = requestsRepository;
        _logger = logger;
    }
    
    public async Task Invoke()
    {
        try
        {
            await _requestsRepository.AddAsync(Payload);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to store request analytics.");
            throw;
        }
    }
}