using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.BackgroundJobs.Abstractions;
using Publications.Domain.Authors;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;
using Publications.Domain.Shared;

namespace Publications.BackgroundJobs;

public class SyncDatabasesTask : BaseRetriableTask<SyncDatabasesTask>
{
    private readonly ISourceRepository _sourceRepository;
    private readonly IPublicationsCommandRepository _publicationsCommandRepository;
    private readonly IAuthorsRepository _authorsRepository;
    private readonly IPublishersRepository _publishersRepository;
    private readonly IFiltersService _filtersService;
    private readonly IRequestsRepository _requestsRepository;

    public SyncDatabasesTask(
        ILogger<SyncDatabasesTask> taskLogger,
        ISourceRepository sourceRepository,
        IOptions<RetriableTaskOptions> options, 
        IPublicationsCommandRepository publicationsCommandRepository,
        IAuthorsRepository authorsRepository, 
        IPublishersRepository publishersRepository,
        IFiltersService filtersService,
        IRequestsRepository requestsRepository) : base(taskLogger, options.Value)
    {
        _sourceRepository = sourceRepository;
        _publicationsCommandRepository = publicationsCommandRepository;
        _authorsRepository = authorsRepository;
        _publishersRepository = publishersRepository;
        _filtersService = filtersService;
        _requestsRepository = requestsRepository;
    }
    
    private readonly DateTime _syncStartDateTime = DateTime.UtcNow;
    
    private IReadOnlyCollection<Publication>? _publications;
    private IReadOnlyCollection<Author>? _authors;
    private IReadOnlyCollection<Publisher>? _publishers;
    private IReadOnlyCollection<FilterGroup>? _filters;
    
    protected override async Task DoRetriableTaskAsync()
    {
        _publications = await _sourceRepository.GetPublicationsAsync();
        _authors = await _sourceRepository.GetAuthorsAsync();
        _publishers = await _sourceRepository.GetPublishersAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        _filters = await _filtersService.GetFiltersForPublicationsAsync(_publications!);
        _publications = await _filtersService.AssignFiltersToPublicationsAsync(
            _publications!.ToList(), _filters.ToList());
        
        await _publicationsCommandRepository.InsertOrUpdateAsync(await SetResourceViewsAsync(_publications));
        await _authorsRepository.InsertOrUpdateAsync(await SetResourceViewsAsync(_authors!));
        await _publishersRepository.InsertOrUpdateAsync(await SetResourceViewsAsync(_publishers!));
        await _publicationsCommandRepository.ReplaceFiltersAsync(_filters);
        
        await _publicationsCommandRepository.SynchronizeAsync(_syncStartDateTime);
        await _authorsRepository.SynchronizeAsync(_syncStartDateTime);
        await _publishersRepository.SynchronizeAsync(_syncStartDateTime);
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