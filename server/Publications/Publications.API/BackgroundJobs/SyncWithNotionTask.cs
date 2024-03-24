using Publications.API.Repositories;

namespace Publications.API.BackgroundJobs;

public class SyncWithNotionTask: BaseRetriableTask<SyncWithNotionTask>
{
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IPublicationsSourceRepository _sourceRepository;

    public SyncWithNotionTask(
        IPublicationsRepository publicationsRepository, 
        IPublicationsSourceRepository sourceRepository,
        ILogger<SyncWithNotionTask> taskLogger, 
        int maxRetries = 3) 
        : base(taskLogger, maxRetries)
    {
        _publicationsRepository = publicationsRepository;
        _sourceRepository = sourceRepository;
    }

    protected override async Task ExecuteTask()
    {
        var publications = await _sourceRepository.GetPublicationsAsync();
        await _publicationsRepository.InsertOrUpdateAsync(publications);
    }
}