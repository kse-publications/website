using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Publications.Application.DTOs.Response;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Application.Statistics;
using Publications.BackgroundJobs.Abstractions;
using Publications.BackgroundJobs.Options;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Domain.Shared;

namespace Publications.BackgroundJobs.Tasks;

public class SyncDatabasesTask(
    ILogger<SyncDatabasesTask> taskLogger,
    IOptionsSnapshot<DbSyncOptions> options,
    IDbVersionService dbVersionService,
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
    
    private HashSet<int>? _sourcePublicationIds;
    private HashSet<int>? _sourceCollectionIds;
    
    private IReadOnlyCollection<SyncEntityMetadata>? _localPublicationsMetadata;
    private IReadOnlyCollection<SyncEntityMetadata>? _localCollectionsMetadata;
    
    private Dictionary<int, SyncEntityMetadata>? _localPublicationsMetadataDict;
    private Dictionary<int, SyncEntityMetadata>? _localCollectionsMetadataDict;
    
    protected override async Task DoRetriableTaskAsync()
    {
        _sourcePublications = await sourceRepository.GetPublicationsAsync();
        _sourceCollections = await sourceRepository.GetCollectionsAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        _localPublicationsMetadata = await publicationsRepository.GetAllSyncMetadataAsync();
        _localCollectionsMetadata = await collectionsRepository.GetAllSyncMetadataAsync();
        
        InitializeDataStructures();
        
        _sourceFilters = await filtersService.GetFiltersForPublicationsAsync(_sourcePublications!);

        int[] deletedPublicationIds = await DeletePublicationsNotInSourceAsync();
        int[] deletedCollectionIds = await DeleteCollectionsNotInSourceAsync();
        
        bool forceUpdateAllCollections = !await dbVersionService.IsMajorVersionUpToDateAsync(typeof(Collection));
        bool forceUpdateAllPublications = !await dbVersionService.IsMajorVersionUpToDateAsync(typeof(Publication));
        
        List<Collection> newOrUpdatedCollections = FindNewOrUpdatedCollections(forceUpdateAllCollections);
        List<Publication> newOrUpdatedPublications = FindNewOrUpdatedPublications(
            newOrUpdatedCollections, deletedCollectionIds, forceUpdateAllPublications);

        if (newOrUpdatedPublications.Count != 0)
        {
            var updatedPublicationsWithFilters = filtersService
                .AssignFiltersToPublicationsAsync(newOrUpdatedPublications, _sourceFilters.ToList());
            
            await publicationsRepository.InsertOrUpdateAsync(updatedPublicationsWithFilters);
        }

        if (newOrUpdatedCollections.Count != 0)
        {
            await collectionsRepository.InsertOrUpdateAsync(newOrUpdatedCollections);
        }
        
        await UpdatePublicationsViews(_sourcePublications!);
        await filtersRepository.ReplaceWithNewAsync(_sourceFilters);
        await statisticsRepository.SetTotalPublicationsCountAsync(_sourcePublications!.Count);
        
        taskLogger.LogInformation(
            "Databases synchronized successfully. " +
            "Publications: {0} updated, {1} deleted. " +
            "Collections: {2} updated, {3} deleted.",
            newOrUpdatedPublications.Count, deletedPublicationIds.Length,
            newOrUpdatedCollections.Count, deletedCollectionIds.Length);
    }

    private void InitializeDataStructures()
    {
        _sourcePublicationIds = _sourcePublications!.Select(p => p.Id).ToHashSet();
        _sourceCollectionIds = _sourceCollections!.Select(c => c.Id).ToHashSet();
        _localPublicationsMetadataDict = _localPublicationsMetadata!.ToDictionary(p => p.Id);
        _localCollectionsMetadataDict = _localCollectionsMetadata!.ToDictionary(c => c.Id);
    }

    private List<Publication> FindNewOrUpdatedPublications(
        IEnumerable<Collection> newOrUpdatedCollections,
        IEnumerable<int> deletedCollectionIds,
        bool forceUpdateAllPublication)
    {
        var updatedPublications = FindUpdatedEntities(
            _localPublicationsMetadataDict!, _sourcePublications!, forceUpdateAllPublication);
        
        if (forceUpdateAllPublication)
            return updatedPublications;

        var newPublications = FindNewEntities(
            _localPublicationsMetadataDict!, _sourcePublications!);
        
        var publicationsInUpdatedCollections = FindPublicationsInChangedCollections(
            newOrUpdatedCollections, deletedCollectionIds);
        
        var publicationsWithUpdatedAuthors = FindPublicationsWithUpdatedAuthors();
        var publicationsWithUpdatedPublishers = FindPublicationsWithUpdatedPublishers();
        
        return updatedPublications
            .Concat(newPublications)
            .Concat(publicationsInUpdatedCollections)
            .Concat(publicationsWithUpdatedAuthors)
            .Concat(publicationsWithUpdatedPublishers)
            .DistinctBy(p => p.Id)
            .ToList();
    }
    
    private List<Collection> FindNewOrUpdatedCollections(bool forceUpdateAllCollections)
    {
        var updatedCollections = FindUpdatedEntities(
            _localCollectionsMetadataDict!, _sourceCollections!, forceUpdateAllCollections);
        
        if (forceUpdateAllCollections)
            return updatedCollections;
        
        var newCollections = FindNewEntities(
            _localCollectionsMetadataDict!, _sourceCollections!);
        
        return updatedCollections
            .Concat(newCollections)
            .ToList();
    }
    
    private List<Publication> FindPublicationsInChangedCollections(
        IEnumerable<Collection> newOrUpdatedCollections,
        IEnumerable<int> deletedCollectionIds)
    {
        var publicationIdsInUpdatedCollections = newOrUpdatedCollections
            .SelectMany(c => c.PublicationsIds)
            .ToHashSet();
        
        return _sourcePublications!
            .Where(p => 
                publicationIdsInUpdatedCollections.Contains(p.Id) || 
                p.Collections.Any(c => deletedCollectionIds.Contains(c.Id)))
            .ToList();
    }
    
    private List<Publication> FindPublicationsWithUpdatedAuthors()
    {
        return _sourcePublications!
            .Where(p => 
                _localPublicationsMetadataDict!.TryGetValue(p.Id, out var localEntityMeta) && 
                p.Authors.Any(author => WasModifiedAfterLastSync(localEntityMeta, author.LastModifiedAt)))
            .ToList();
    }
    
    private List<Publication> FindPublicationsWithUpdatedPublishers()
    {
        return _sourcePublications!
            .Where(p => 
                p.Publisher is not null && 
                _localPublicationsMetadataDict!.TryGetValue(p.Id, out var localEntityMeta) &&
                WasModifiedAfterLastSync(localEntityMeta, p.Publisher.LastModifiedAt))
            .ToList();
    }
    
    private static List<TEntity> FindUpdatedEntities<TEntity>(
        Dictionary<int, SyncEntityMetadata> localEntitiesMetadataDict,
        IReadOnlyCollection<TEntity> sourceEntities,
        bool forceUpdateAll) where TEntity : Entity
    {
        if (forceUpdateAll)
        {
            return sourceEntities.ToList();
        }
        
        List<TEntity> updatedEntities = sourceEntities
            .Where(sourceEntity =>
                localEntitiesMetadataDict.TryGetValue(sourceEntity.Id, out var localEntityMeta) &&
                WasModifiedAfterLastSync(localEntityMeta, sourceEntity.LastModifiedAt))
            .ToList();

        return updatedEntities;
    }
    
    private static List<TEntity> FindNewEntities<TEntity>(
        Dictionary<int, SyncEntityMetadata> localEntitiesMetadataDict,
        IReadOnlyCollection<TEntity> sourceEntities) where TEntity : Entity
    {
        return sourceEntities
            .Where(e => !localEntitiesMetadataDict.ContainsKey(e.Id))
            .ToList();
    }
    
    private async Task<int[]> DeletePublicationsNotInSourceAsync()
    {
        List<Publication> publicationsToDelete = _localPublicationsMetadata!
            .Where(sm => !_sourcePublicationIds!.Contains(sm.Id))
            .Select(sm => Publication.InitWithId(sm.Id))
            .ToList();

        await publicationsRepository.DeleteAsync(publicationsToDelete);

        var deletedIds = publicationsToDelete.Select(p => p.Id);
        
        _localPublicationsMetadata = _localPublicationsMetadata!
            .Where(p => !deletedIds.Contains(p.Id))
            .ToList();
        
        return deletedIds.ToArray();
    }
    
    private async Task<int[]> DeleteCollectionsNotInSourceAsync()
    {
        List<Collection> collectionsToDelete = _localCollectionsMetadata!
            .Where(sm => !_sourceCollectionIds!.Contains(sm.Id))
            .Select(sm => Collection.InitWithId(sm.Id))
            .ToList();

        await collectionsRepository.DeleteAsync(collectionsToDelete);

        var deletedIds = collectionsToDelete.Select(c => c.Id);
        
        _localCollectionsMetadata = _localCollectionsMetadata!
            .Where(c => !deletedIds.Contains(c.Id))
            .ToList();

        return deletedIds.ToArray();
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

    private static bool WasModifiedAfterLastSync(
        SyncEntityMetadata localMetadata, DateTime lastEditedTime)
    {
        return localMetadata.LastSynchronizedAt.ToUniversalTime() < lastEditedTime.ToUniversalTime();
    }
}