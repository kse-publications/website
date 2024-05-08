using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.BackgroundJobs.Abstractions;
using Publications.Domain.Shared;

namespace Publications.BackgroundJobs;

public class SyncDatabasesTask : BaseRetriableTask<SyncDatabasesTask>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly IAuthorsRepository _authorsRepository;
    private readonly IPublishersRepository _publishersRepository;
    private readonly IFiltersService _filtersService;
    private readonly IFiltersRepository _filtersRepository;
    private readonly IRequestsRepository _requestsRepository;

    public SyncDatabasesTask(
        ILogger<SyncDatabasesTask> taskLogger,
        ISourceRepository sourceRepository,
        IOptions<RetriableTaskOptions> options, 
        IPublicationsRepository publicationsRepository,
        IAuthorsRepository authorsRepository, 
        IPublishersRepository publishersRepository,
        IFiltersRepository filtersRepository, 
        IFiltersService filtersService,
        IRequestsRepository requestsRepository) : base(taskLogger, options.Value)
    {
        _sourceRepository = sourceRepository;
        _publicationsRepository = publicationsRepository;
        _authorsRepository = authorsRepository;
        _publishersRepository = publishersRepository;
        _filtersRepository = filtersRepository;
        _filtersService = filtersService;
        _requestsRepository = requestsRepository;
    }

    protected override async Task DoRetriableTaskAsync()
    {
        var publications = await _sourceRepository.GetPublicationsAsync();
        var authors = await _sourceRepository.GetAuthorsAsync();
        var publishers = await _sourceRepository.GetPublishersAsync();
        var filters = await _filtersService.GetFiltersForPublicationsAsync(publications);

        publications = await _filtersService.AssignFiltersToPublicationsAsync(
            publications.ToList(), filters.ToList());
        await _publicationsRepository.InsertOrUpdateAsync(await SetResourceViewsAsync(publications));
        await _authorsRepository.InsertOrUpdateAsync(await SetResourceViewsAsync(authors));
        await _publishersRepository.InsertOrUpdateAsync(await SetResourceViewsAsync(publishers));
        await _filtersRepository.InsertOrUpdateAsync(filters);
    }

    private async Task<IReadOnlyCollection<TResource>> SetResourceViewsAsync<TResource>(
        IEnumerable<TResource> resourceItemsCollection)
        where TResource : Entity<TResource>
    {
        Dictionary<int, int> views = await _requestsRepository.GetResourceViews<TResource>();

        return resourceItemsCollection
            .Select(resource => views.TryGetValue(resource.Id, out int resourceViews)
                ? resource.UpdateViews(resourceViews)
                : resource)
            .ToList()
            .AsReadOnly();
    }
}