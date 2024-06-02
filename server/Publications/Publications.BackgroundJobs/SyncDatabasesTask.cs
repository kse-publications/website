using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Application.Statistics;
using Publications.BackgroundJobs.Abstractions;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.BackgroundJobs;

public class SyncDatabasesTask(
    ILogger<SyncDatabasesTask> taskLogger,
    IOptions<RetriableTaskOptions> options,
    ISourceRepository sourceRepository,
    IPublicationsCommandRepository publicationsCommandRepository,
    ICollectionsRepository collectionsRepository,
    IFiltersService filtersService,
    IRequestsRepository requestsRepository,
    IStatisticsRepository statisticsRepository)
    : BaseRetriableTask<SyncDatabasesTask>(taskLogger, options.Value)
{
    private readonly DateTime _syncStartDateTime = DateTime.UtcNow;
    
    private IReadOnlyCollection<Publication>? _publications;
    private IReadOnlyCollection<FilterGroup>? _filters;
    private IReadOnlyCollection<Collection>? _collections;
    
    protected override async Task DoRetriableTaskAsync()
    {
        _collections = await sourceRepository.GetCollectionsAsync();
        _publications = await sourceRepository.GetPublicationsAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        _filters = await filtersService.GetFiltersForPublicationsAsync(_publications!);
        _publications = filtersService.AssignFiltersToPublicationsAsync(
            _publications!.ToList(), _filters.ToList());

        await collectionsRepository.InsertOrUpdateAsync(_collections!);
        await publicationsCommandRepository
            .InsertOrUpdateAsync(await SetPublicationsViews(_publications));
        await publicationsCommandRepository.ReplaceFiltersAsync(_filters);

        await collectionsRepository.SynchronizeAsync(_syncStartDateTime);
        await publicationsCommandRepository.SynchronizeAsync(_syncStartDateTime);
        await statisticsRepository.SetTotalPublicationsCountAsync(_publications.Count);
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