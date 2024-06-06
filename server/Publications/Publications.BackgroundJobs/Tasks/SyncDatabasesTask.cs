using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Application.Statistics;
using Publications.BackgroundJobs.Abstractions;
using Publications.BackgroundJobs.Options;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.BackgroundJobs.Tasks;

public class SyncDatabasesTask(
    ILogger<SyncDatabasesTask> taskLogger,
    IOptionsSnapshot<DbSynchronizationOptions> options,
    ISourceRepository sourceRepository,
    IPublicationsRepository publicationsRepository,
    ICollectionsRepository collectionsRepository,
    IFiltersRepository filtersRepository,
    IFiltersService filtersService,
    IRequestsRepository requestsRepository,
    IStatisticsRepository statisticsRepository)
    : BaseRetriableTask<SyncDatabasesTask>(taskLogger, options.Value.RetryOptions)
{
    private IReadOnlyCollection<Publication>? _sourcePublications;
    private IReadOnlyCollection<FilterGroup>? _sourceFilters;
    private IReadOnlyCollection<Collection>? _sourceCollections;
    private IReadOnlyCollection<Publication>? _localPublications;
    
    protected override async Task DoRetriableTaskAsync()
    {
        _sourceCollections = await sourceRepository.GetCollectionsAsync();
        _sourcePublications = await sourceRepository.GetPublicationsAsync();
        _localPublications = await publicationsRepository.GetAllAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        _sourceFilters = await filtersService.GetFiltersForPublicationsAsync(_sourcePublications!);

        await DeletePublicationsNotInSourceAsync();
        
        List<Publication> newOrUpdatedPublications = FindNewOrUpdatedPublications(
            options.Value.ForceUpdateAll.Enabled);
        
        newOrUpdatedPublications = filtersService
            .AssignFiltersToPublicationsAsync(newOrUpdatedPublications, _sourceFilters.ToList())
            .Select(p => p.Synchronize())
            .ToList();
            
        await publicationsRepository.InsertOrUpdateAsync(newOrUpdatedPublications);
        await UpdatePublicationsViews(newOrUpdatedPublications);
        await collectionsRepository.InsertOrUpdateAsync(_sourceCollections!);
        await filtersRepository.ReplaceWithNewAsync(_sourceFilters);
        await statisticsRepository.SetTotalPublicationsCountAsync(_sourcePublications!.Count);
    }
    
    private async Task DeletePublicationsNotInSourceAsync()
    {
        var sourcePublicationsDict = _sourcePublications!.ToDictionary(p => p.Id);

        List<Publication> publicationsToDelete = _localPublications!
            .Where(p => !sourcePublicationsDict.TryGetValue(p.Id, out _))
            .ToList();

        await publicationsRepository.DeleteAsync(publicationsToDelete);
    }

    private List<Publication> FindNewOrUpdatedPublications(bool forceUpdateAllEnabled)
    {
        if (options.Value.ForceUpdateAll.Enabled)
        { 
            return _sourcePublications!.ToList();
        }
        
        var localIdsHashSet = _localPublications!.Select(p => p.Id).ToHashSet();
        var sourcePublicationsDict = _sourcePublications!.ToDictionary(p => p.Id);

        // updated publications
        List<Publication> newOrUpdatedPublications = _localPublications!
            .Where(p =>
                sourcePublicationsDict.TryGetValue(p.Id, out _) &&
                p.LastSynchronizedAt < sourcePublicationsDict[p.Id].LastModifiedAt)
            .Select(p => sourcePublicationsDict[p.Id])
            .ToList();
            
        // new publications
        newOrUpdatedPublications.AddRange(_sourcePublications!
            .Where(p => !localIdsHashSet.Contains(p.Id)));
        
        return newOrUpdatedPublications;
    }

    private async Task UpdatePublicationsViews(IEnumerable<Publication> publications)
    {
        Dictionary<int, int> views = await requestsRepository.GetResourceViews<Publication>();

        foreach (var publication in publications)
        {
            if (!views.TryGetValue(publication.Id, out var viewsCount))
                continue;
            
            await publicationsRepository.UpdatePropertyValueAsync(
                publication.Id, 
                nameof(Publication.Views),
                newValue: viewsCount.ToString());
        }
    }
}