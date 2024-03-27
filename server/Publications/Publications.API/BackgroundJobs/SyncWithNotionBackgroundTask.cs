using Microsoft.Extensions.Options;
using Publications.API.Models;
using Publications.API.Repositories;
using Publications.API.Repositories.Authors;
using Publications.API.Repositories.Publications;
using Publications.API.Repositories.Publishers;
using Publications.API.Repositories.Source;

namespace Publications.API.BackgroundJobs;

public class SyncWithNotionBackgroundTask : BaseRetriableTask<SyncWithNotionBackgroundTask>
{
    private readonly ILogger<SyncWithNotionBackgroundTask> _taskLogger;
    private readonly ISourceRepository _sourceRepository;
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IAuthorsRepository _authorsRepository;
    private readonly IPublishersRepository _publishersRepository;

    public SyncWithNotionBackgroundTask(
        ILogger<SyncWithNotionBackgroundTask> taskLogger,
        ISourceRepository sourceRepository,
        IOptions<RetriableTaskOptions> options, 
        IPublicationsRepository publicationsRepository,
        IAuthorsRepository authorsRepository, 
        IPublishersRepository publishersRepository) : base(taskLogger, options.Value)
    {
        _taskLogger = taskLogger;
        _sourceRepository = sourceRepository;
        _publicationsRepository = publicationsRepository;
        _authorsRepository = authorsRepository;
        _publishersRepository = publishersRepository;
    }

    protected override async Task DoBackgroundRetriableTaskAsync()
    {
        IReadOnlyCollection<Publication> publications = await _sourceRepository.GetPublicationsAsync();
        IReadOnlyCollection<Author> authors = await _sourceRepository.GetAuthorsAsync();
        IReadOnlyCollection<Publisher> publishers = await _sourceRepository.GetPublishersAsync();
        await _publicationsRepository.InsertOrUpdateAsync(publications);
        await _authorsRepository.InsertOrUpdateAsync(authors);
        await _publishersRepository.InsertOrUpdateAsync(publishers);
    }
}