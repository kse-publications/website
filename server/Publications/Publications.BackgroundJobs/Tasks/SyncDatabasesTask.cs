using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.BackgroundJobs.Abstractions;
using Publications.BackgroundJobs.Options;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Shared;
using Publications.Domain.Shared.Slugs;

namespace Publications.BackgroundJobs.Tasks;

public class SyncDatabasesTask(
    ILogger<SyncDatabasesTask> taskLogger,
    IOptionsSnapshot<DbSyncOptions> options,
    VectorizePublicationsTask vectorizePublicationsTask,
    ISourceRepository sourceRepository,
    IPublicationsRepository publicationsRepository,
    ICollectionsRepository collectionsRepository,
    IWordsService wordsService,
    IFiltersRepository filtersRepository,
    IFiltersService filtersService,
    IRequestsRepository requestsRepository,
    IStatisticsRepository statisticsRepository)
    : BaseRetriableTask<SyncDatabasesTask>(taskLogger, options.Value.RetryOptions)
{
    private readonly ILogger<SyncDatabasesTask> _taskLogger = taskLogger;
    private IReadOnlyCollection<Publication>? _sourcePublications;
    private IReadOnlyCollection<FilterGroup>? _sourceFilters;
    private IReadOnlyCollection<Collection>? _sourceCollections;
    
    private HashSet<int>? _sourcePublicationIds;
    private HashSet<int>? _sourceCollectionIds;
    
    private IReadOnlyCollection<SyncEntityMetadata>? _localPublicationsMetadata;
    private IReadOnlyCollection<SyncCollectionMetadata>? _localCollectionsMetadata;
    
    private Dictionary<int, SyncEntityMetadata>? _localPublicationsMetadataDict;
    private Dictionary<int, SyncCollectionMetadata>? _localCollectionsMetadataDict;

    protected override async Task DoRetriableTaskAsync()
    {
        (_sourcePublications, _sourceCollections) = await sourceRepository.GetPublicationsAndCollectionsAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        await SetLocalEntitiesMetadataAsync();
        InitializeDataStructures();

        int[] deletedPublicationIds = await DeletePublicationsNotInSourceAsync();
        int[] deletedCollectionIds = await DeleteCollectionsNotInSourceAsync();
        
        IList<Collection> newCollections = FindNewEntities(_localCollectionsMetadataDict!, _sourceCollections!);
        IList<Collection> updatedCollections = GetUpdatedCollections();
        
        newCollections = HydrateCollections(newCollections);
        updatedCollections = HydrateCollections(updatedCollections);
        
        _sourceFilters = await filtersService.GetFiltersForPublicationsAsync(_sourcePublications!);
        
        IList<Publication> newPublications = FindNewEntities(_localPublicationsMetadataDict!, _sourcePublications!);
        HashSet<int> newPublicationIds = newPublications.Select(p => p.Id).ToHashSet();
        IList<Publication> updatedPublications = 
            GetUpdatedPublications(updatedCollections, newCollections, deletedCollectionIds)
                .Where(p => !newPublicationIds.Contains(p.Id)).ToList();
        
        newPublications = HydratePublications(newPublications);
        updatedPublications = HydratePublications(updatedPublications);
        
        await publicationsRepository.UpdateAsync(updatedPublications);
        await publicationsRepository.InsertAsync(newPublications);
        
        await collectionsRepository.UpdateAsync(updatedCollections);
        await collectionsRepository.InsertAsync(newCollections);
        
        await filtersRepository.ReplaceWithNewAsync(_sourceFilters);
        
        await UpdatePublicationsViews(_sourcePublications!);
        await UpdateViewsCountStats();
        PublicationSummary[] topRecentlyViewedPublications = await publicationsRepository
            .GetTopPublicationsByRecentViews();
        
        await statisticsRepository.SetTopRecentlyViewedPublicationsAsync(topRecentlyViewedPublications);
        await statisticsRepository.SetTotalPublicationsCountAsync(_sourcePublications!.Count);
        
        _taskLogger.LogInformation(
            "Databases synchronized successfully. " +
            "Publications: {0} added, {1} updated, {2} deleted. " +
            "Collections: {3} added, {4} updated, {5} deleted.",
            newPublications.Count, updatedPublications.Count, deletedPublicationIds.Length,
            newCollections.Count, updatedCollections.Count, deletedCollectionIds.Length);
        
        _ = Task.Run(async () => await vectorizePublicationsTask.Invoke()); 
    }
     
    private void InitializeDataStructures()
    {
        _sourcePublicationIds = _sourcePublications!.Select(p => p.Id).ToHashSet();
        _sourceCollectionIds = _sourceCollections!.Select(c => c.Id).ToHashSet();
        _localPublicationsMetadataDict = _localPublicationsMetadata!.ToDictionary(p => p.Id);
        _localCollectionsMetadataDict = _localCollectionsMetadata!.ToDictionary(c => c.Id);
    }
    
    private async Task SetLocalEntitiesMetadataAsync()
    {
        _localPublicationsMetadata = await publicationsRepository.GetAllSyncMetadataAsync();
        _localCollectionsMetadata = await collectionsRepository.GetAllSyncMetadataAsync();
    }

    private IList<Publication> GetUpdatedPublications(
        IList<Collection> updatedCollections,
        IList<Collection> newCollections,
        IEnumerable<int> deletedCollectionIds)
    {
        var updatedPublications = FindUpdatedEntities(
            _localPublicationsMetadataDict!, _sourcePublications!);
        
        var publicationsInUpdatedCollections = FindPublicationsInChangedCollections(
            updatedCollections, newCollections, deletedCollectionIds);
        
        var publicationsWithUpdatedAuthors = FindPublicationsWithUpdatedAuthors();
        var publicationsWithUpdatedPublishers = FindPublicationsWithUpdatedPublishers();
        
        return updatedPublications
            .Concat(publicationsInUpdatedCollections)
            .Concat(publicationsWithUpdatedAuthors)
            .Concat(publicationsWithUpdatedPublishers)
            .DistinctBy(p => p.Id)
            .ToList();
    }
    
    private IList<Collection> GetUpdatedCollections()
    {
        return FindUpdatedEntities(_localCollectionsMetadataDict!, _sourceCollections!);
    }
    
    private List<Publication> FindPublicationsInChangedCollections(
        IList<Collection> updatedCollections,
        IList<Collection> newCollections,
        IEnumerable<int> deletedCollectionIds)
    {
        HashSet<int> publicationIdsInChangedCollections = updatedCollections
            .Concat(newCollections)
            .SelectMany(c => c.GetPublicationIds())
            .Concat(updatedCollections
                .SelectMany(c => _localCollectionsMetadataDict![c.Id].PublicationsIds))
            .ToHashSet();
        
        return _sourcePublications!
            .Where(p => 
                publicationIdsInChangedCollections.Contains(p.Id) || 
                p.Collections.Any(c => deletedCollectionIds.Contains(c.Id)))
            .ToList();
    }
    
    private IList<Publication> FindPublicationsWithUpdatedAuthors()
    {
        return _sourcePublications!
            .Where(p => 
                _localPublicationsMetadataDict!.TryGetValue(p.Id, out var localEntityMeta) && 
                p.Authors.Any(author => WasModifiedAfterLastSync(localEntityMeta, author.LastModifiedAt)))
            .ToList();
    }
    
    private IList<Publication> FindPublicationsWithUpdatedPublishers()
    {
        return _sourcePublications!
            .Where(p => 
                p.Publisher is not null && 
                _localPublicationsMetadataDict!.TryGetValue(p.Id, out var localEntityMeta) &&
                WasModifiedAfterLastSync(localEntityMeta, p.Publisher.LastModifiedAt))
            .ToList();
    }
    
    private static IList<TEntity> FindUpdatedEntities<TEntity, TMetadata>(
        Dictionary<int, TMetadata> localEntitiesMetadataDict,
        IReadOnlyCollection<TEntity> sourceEntities) 
        where TEntity : Entity
        where TMetadata : SyncEntityMetadata
    {
        List<TEntity> updatedEntities = sourceEntities
            .Where(sourceEntity =>
                localEntitiesMetadataDict.TryGetValue(sourceEntity.Id, out var localEntityMeta) &&
                WasModifiedAfterLastSync(localEntityMeta, sourceEntity.LastModifiedAt))
            .ToList();

        return updatedEntities;
    }

    private static IList<TEntity> FindNewEntities<TEntity, TMetadata>(
        Dictionary<int, TMetadata> localEntitiesMetadataDict,
        IReadOnlyCollection<TEntity> sourceEntities) 
        where TEntity : Entity 
        where TMetadata : SyncEntityMetadata
    {
        return sourceEntities
            .Where(e => !localEntitiesMetadataDict.ContainsKey(e.Id))
            .ToList();
    }
    
    private async Task<int[]> DeletePublicationsNotInSourceAsync()
    {
        int[] deletedIds = _localPublicationsMetadata!
            .Where(meta => !_sourcePublicationIds!.Contains(meta.Id))
            .Select(meta => meta.Id)
            .ToArray();

        await publicationsRepository.DeleteAsync(deletedIds);
        
        _localPublicationsMetadata = _localPublicationsMetadata!
            .Where(p => !deletedIds.Contains(p.Id))
            .ToList();
        
        return deletedIds.ToArray();
    }
    
    private async Task<int[]> DeleteCollectionsNotInSourceAsync()
    {
        int[] deletedIds = _localCollectionsMetadata!
            .Where(meta => !_sourceCollectionIds!.Contains(meta.Id))
            .Select(meta => meta.Id)
            .ToArray();

        await collectionsRepository.DeleteAsync(deletedIds);
        
        _localCollectionsMetadata = _localCollectionsMetadata!
            .Where(c => !deletedIds.Contains(c.Id))
            .ToList();

        return deletedIds.ToArray();
    }

    private async Task UpdatePublicationsViews(IEnumerable<Publication> publications)
    {
        DateTime lastMonth = DateTime.Today - TimeSpan.FromDays(30);
        
        Dictionary<int, int> totalViewsDistribution = await requestsRepository
            .GetRequestsDistributionAsync<Publication>();
        Dictionary<int, int> recentViewsDistribution = await requestsRepository
            .GetRequestsDistributionAsync<Publication>(after: lastMonth);

        List<Task> updateViewsTasks = new(totalViewsDistribution.Count + recentViewsDistribution.Count);
            
        foreach (var publication in publications)
        {
            if (totalViewsDistribution.TryGetValue(publication.Id, out var viewsCount))
            {
                updateViewsTasks.Add(publicationsRepository.UpdateAsync(
                    publication.Id,
                    propertyName: nameof(Publication.Views),
                    newValue: viewsCount.ToString()));
            }
                
            if (recentViewsDistribution.TryGetValue(publication.Id, out var recentViews))
            {
                updateViewsTasks.Add(publicationsRepository.UpdateAsync(
                    publication.Id,
                    propertyName: nameof(Publication.RecentViews),
                    newValue: recentViews.ToString()));
            }
        }
            
        await Task.WhenAll(updateViewsTasks);
    }

    private async Task UpdateViewsCountStats()
    {
        DateTime lastMonth = DateTime.Today - TimeSpan.FromDays(30);
        
        int totalViewsCount = await requestsRepository
            .GetRequestsCountAsync<Publication>(distinct: false);
        int recentViewsCount = await requestsRepository
            .GetRequestsCountAsync<Publication>(after: lastMonth, distinct: false);
        
        await statisticsRepository.SetTotalViewsCountAsync(totalViewsCount);
        
        await statisticsRepository.SetRecentViewsCountAsync(recentViewsCount);
    }

    private IList<Publication> HydratePublications(IList<Publication> publications)
    {
        return filtersService.AssignFiltersToPublications(publications, _sourceFilters!.ToList())
            .Select(p => p.HydrateSlug(wordsService))
            .ToList();
    }
    
    private IList<Collection> HydrateCollections(IList<Collection> collections)
    {
        return collections.Select(c => c.HydrateSlug(wordsService)).ToList();
    }
    
    private static bool WasModifiedAfterLastSync(
        SyncEntityMetadata localMetadata, DateTime lastEditedTime)
    {
        return localMetadata.LastSynchronizedAt.ToUniversalTime() < lastEditedTime.ToUniversalTime();
    }
}