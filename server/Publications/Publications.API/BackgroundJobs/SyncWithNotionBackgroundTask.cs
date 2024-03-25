using Microsoft.Extensions.Options;
using Publications.API.Models;
using Publications.API.Repositories;

namespace Publications.API.BackgroundJobs;

public class SyncWithNotionBackgroundTask : BaseRetriableTask<SyncWithNotionBackgroundTask>
{
    private readonly ILogger<SyncWithNotionBackgroundTask> _taskLogger;
    private readonly IPublicationsSourceRepository _sourceRepository;
    private readonly IPublicationsRepository _targetRepository;

    public SyncWithNotionBackgroundTask(
        ILogger<SyncWithNotionBackgroundTask> taskLogger,
        IPublicationsSourceRepository sourceRepository,
        IPublicationsRepository targetRepository,
        IOptions<RetriableTaskOptions> options) : base(taskLogger, options.Value)
    {
        _taskLogger = taskLogger;
        _sourceRepository = sourceRepository;
        _targetRepository = targetRepository;
    }

    protected override async Task DoBackgroundRetriableTaskAsync()
    {
        IReadOnlyCollection<Publication> publications = await _sourceRepository.GetPublicationsAsync();
        await _targetRepository.InsertOrUpdateAsync(publications);
    }
}