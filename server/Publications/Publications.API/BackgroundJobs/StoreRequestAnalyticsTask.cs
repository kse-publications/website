using Coravel.Invocable;
using Publications.API.Models;
using Publications.API.Repositories.Requests;

namespace Publications.API.BackgroundJobs;

public class StoreRequestAnalyticsTask: IInvocable, IInvocableWithPayload<Request>
{
    private readonly IRequestsRepository _requestsRepository;
    
    public Request Payload { get; set; } = null!;

    public StoreRequestAnalyticsTask(IRequestsRepository requestsRepository)
    {
        _requestsRepository = requestsRepository;
    }
    
    public async Task Invoke()
    {
        await _requestsRepository.AddAsync(Payload);
    }
}