using Microsoft.Extensions.Options;
using Publications.API.BackgroundJobs.Abstractions;
using Publications.API.Repositories.Authors;
using Publications.API.Repositories.Filters;
using Publications.API.Repositories.Publications;
using Publications.API.Repositories.Publishers;
using Publications.API.Repositories.Source;
using Publications.API.Services;

namespace Publications.API.BackgroundJobs;

public class SyncWithNotionBackgroundTask : BaseRetriableTask<SyncWithNotionBackgroundTask>
{
    private readonly ILogger<SyncWithNotionBackgroundTask> _taskLogger;
    private readonly ISourceRepository _sourceRepository;
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IAuthorsRepository _authorsRepository;
    private readonly IPublishersRepository _publishersRepository;
    private readonly IFiltersService _filtersService;
    private readonly IFiltersRepository _filtersRepository;

    public SyncWithNotionBackgroundTask(
        ILogger<SyncWithNotionBackgroundTask> taskLogger,
        ISourceRepository sourceRepository,
        IOptions<RetriableTaskOptions> options, 
        IPublicationsRepository publicationsRepository,
        IAuthorsRepository authorsRepository, 
        IPublishersRepository publishersRepository,
        IFiltersRepository filtersRepository, 
        IFiltersService filtersService) : base(taskLogger, options.Value)
    {
        _taskLogger = taskLogger;
        _sourceRepository = sourceRepository;
        _publicationsRepository = publicationsRepository;
        _authorsRepository = authorsRepository;
        _publishersRepository = publishersRepository;
        _filtersRepository = filtersRepository;
        _filtersService = filtersService;
    }

    protected override async Task DoRetriableTaskAsync()
    {
        var publications = await _sourceRepository.GetPublicationsAsync();
        var authors = await _sourceRepository.GetAuthorsAsync();
        var publishers = await _sourceRepository.GetPublishersAsync();
        var filters = await _filtersService.GetFiltersForPublicationsAsync(publications);
        
        await _publicationsRepository.InsertOrUpdateAsync(await _filtersService
            .AssignFiltersToPublicationsAsync(publications.ToList(), filters));
        await _authorsRepository.InsertOrUpdateAsync(authors);
        await _publishersRepository.InsertOrUpdateAsync(publishers);
        await _filtersRepository.InsertOrUpdateAsync(filters);
    }
}