using Coravel.Invocable;
using Microsoft.Extensions.Logging;
using Publications.Application.Repositories;
using Publications.Domain.Requests;

namespace Publications.BackgroundJobs.Tasks;

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