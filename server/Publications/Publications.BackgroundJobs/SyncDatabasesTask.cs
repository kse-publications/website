using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.BackgroundJobs.Abstractions;
using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;

namespace Publications.BackgroundJobs;

public class SyncDatabasesTask(
    ILogger<SyncDatabasesTask> taskLogger,
    IOptions<RetriableTaskOptions> options,
    ISourceRepository sourceRepository,
    IPublicationsCommandRepository publicationsCommandRepository,
    ICollectionsRepository collectionsRepository,
    IAuthorsRepository authorsRepository,
    IPublishersRepository publishersRepository,
    IFiltersService filtersService,
    IRequestsRepository requestsRepository)
    : BaseRetriableTask<SyncDatabasesTask>(taskLogger, options.Value)
{
    private readonly DateTime _syncStartDateTime = DateTime.UtcNow;
    
    private IReadOnlyCollection<Publication>? _publications;
    private IReadOnlyCollection<Author>? _authors;
    private IReadOnlyCollection<Publisher>? _publishers;
    private IReadOnlyCollection<FilterGroup>? _filters;
    private IReadOnlyCollection<Collection>? _collections;
    
    protected override async Task DoRetriableTaskAsync()
    {
        _collections = await sourceRepository.GetCollectionsAsync();
        _publications = await sourceRepository.GetPublicationsAsync();
        _authors = await sourceRepository.GetAuthorsAsync();
        _publishers = await sourceRepository.GetPublishersAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        _filters = await filtersService.GetFiltersForPublicationsAsync(_publications!);
        _publications = filtersService.AssignFiltersToPublicationsAsync(
            _publications!.ToList(), _filters.ToList());

        await collectionsRepository.InsertOrUpdateAsync(_collections!);
        await publicationsCommandRepository
            .InsertOrUpdateAsync(await SetPublicationsViews(_publications));
        await authorsRepository.InsertOrUpdateAsync(_authors!);
        await publishersRepository.InsertOrUpdateAsync(_publishers!);
        await publicationsCommandRepository.ReplaceFiltersAsync(_filters);

        await collectionsRepository.SynchronizeAsync(_syncStartDateTime);
        await publicationsCommandRepository.SynchronizeAsync(_syncStartDateTime);
        await authorsRepository.SynchronizeAsync(_syncStartDateTime);
        await publishersRepository.SynchronizeAsync(_syncStartDateTime);
    }

    private async Task<IReadOnlyCollection<Publication>> SetPublicationsViews(
        IEnumerable<Publication> resourceItemsCollection)
    {
        Dictionary<int, int> views = await requestsRepository.GetResourceViews<Publication>();

        return resourceItemsCollection
            .Select(resource => views.TryGetValue(resource.Id, out int resourceViews)
                ? resource.UpdateViews(resourceViews)
                : resource)
            .ToList()
            .AsReadOnly();
    }
}