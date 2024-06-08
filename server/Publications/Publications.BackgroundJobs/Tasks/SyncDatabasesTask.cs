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
    private IReadOnlyCollection<Author>? _sourceAuthors;
    private IReadOnlyCollection<Publisher>? _sourcePublishers;
    
    private IReadOnlyCollection<Publication>? _localPublications;
    private IReadOnlyCollection<Collection>? _localCollections;
    
    protected override async Task DoRetriableTaskAsync()
    {
        _sourceCollections = await sourceRepository.GetCollectionsAsync();
        _sourcePublications = await sourceRepository.GetPublicationsAsync();
        _sourceAuthors = await sourceRepository.GetAuthorsAsync();
        _sourcePublishers = await sourceRepository.GetPublishersAsync();
    }

    protected override async Task OnSuccessAsync()
    {
        _localPublications = await publicationsRepository.GetAllAsync();
        _localCollections = await collectionsRepository.GetAllAsync();

        await DeletePublicationsNotInSourceAsync();
        
        bool forceUpdateAllCollections = !await dbVersionService.IsMajorVersionUpToDateAsync(typeof(Collection));
        List<Collection> newOrUpdatedCollections = FindNewOrUpdatedCollections(forceUpdateAllCollections);

        bool forceUpdateAllPublications = !await dbVersionService.IsMajorVersionUpToDateAsync(typeof(Publication));
        var newOrUpdatedPublications = FindNewOrUpdatedPublications(
            newOrUpdatedCollections, forceUpdateAllPublications);
        
        _sourceFilters = await filtersService.GetFiltersForPublicationsAsync(_sourcePublications!);
        
        newOrUpdatedPublications = filtersService
            .AssignFiltersToPublicationsAsync(newOrUpdatedPublications, _sourceFilters.ToList())
            .ToList();
            
        await publicationsRepository.InsertOrUpdateAsync(newOrUpdatedPublications);
        await UpdatePublicationsViews(newOrUpdatedPublications);
        await collectionsRepository.InsertOrUpdateAsync(newOrUpdatedCollections);
        await filtersRepository.ReplaceWithNewAsync(_sourceFilters);
        await statisticsRepository.SetTotalPublicationsCountAsync(_sourcePublications!.Count);
        
        taskLogger.LogInformation(
            "Databases synchronized successfully. {0} publications updated. {1} collections updated.",
            newOrUpdatedPublications.Count, newOrUpdatedCollections.Count);
    }
    
    private async Task DeletePublicationsNotInSourceAsync()
    {
        var sourcePublicationsDict = _sourcePublications!.ToDictionary(p => p.Id);

        List<Publication> publicationsToDelete = _localPublications!
            .Where(p => !sourcePublicationsDict.ContainsKey(p.Id))
            .ToList();

        await publicationsRepository.DeleteAsync(publicationsToDelete);

        var deletedIds = publicationsToDelete.Select(p => p.Id);
        
        _localPublications = _localPublications!
            .Where(p => !deletedIds.Contains(p.Id))
            .ToList();
    }

    private List<Publication> FindNewOrUpdatedPublications(
        IReadOnlyCollection<Collection> newOrUpdatedCollections,
        bool forceUpdateAllPublication)
    {
        var newOrUpdatedPublications = FindNewOrUpdatedEntities(
            _localPublications!, _sourcePublications!, forceUpdateAllPublication);
        
        if (forceUpdateAllPublication)
            return newOrUpdatedPublications;
        
        var publicationsInUpdatedCollections = FindPublicationsInUpdatedCollections(
            newOrUpdatedCollections);
        
        var publicationsWithUpdatedAuthors = FindPublicationsWithUpdatedAuthors();
        var publicationsWithUpdatedPublishers = FindPublicationsWithUpdatedPublishers();
        
        return newOrUpdatedPublications
            .Concat(publicationsInUpdatedCollections)
            .Concat(publicationsWithUpdatedAuthors)
            .Concat(publicationsWithUpdatedPublishers)
            .DistinctBy(p => p.Id)
            .ToList();
    }
    
    private List<Collection> FindNewOrUpdatedCollections(bool forceUpdateAllCollections)
    {
        return FindNewOrUpdatedEntities(
            _localCollections!, _sourceCollections!, forceUpdateAllCollections);
    }
    
    private List<Publication> FindPublicationsInUpdatedCollections(
        IReadOnlyCollection<Collection> newOrUpdatedCollections)
    {
        var publicationIdsInUpdatedCollections = newOrUpdatedCollections
            .SelectMany(c => c.PublicationsIds)
            .ToHashSet();
        
        return _sourcePublications!
            .Where(p => publicationIdsInUpdatedCollections.Contains(p.Id))
            .ToList();
    }
    
    private List<Publication> FindPublicationsWithUpdatedAuthors()
    {
        return _sourcePublications!
            .Where(p => p.Authors.Any(a => p.LastSynchronizedAt < a.LastModifiedAt))
            .ToList();
    }
    
    private List<Publication> FindPublicationsWithUpdatedPublishers()
    {
        return _sourcePublications!
            .Where(p => 
                p.Publisher is not null && 
                p.LastSynchronizedAt < p.Publisher.LastModifiedAt)
            .ToList();
    }
    
    private static List<TEntity> FindNewOrUpdatedEntities<TEntity>(
        IReadOnlyCollection<TEntity> localEntities,
        IReadOnlyCollection<TEntity> sourceEntities,
        bool forceUpdateAll) where TEntity : Entity<TEntity>
    {
        if (forceUpdateAll)
        {
            return sourceEntities.ToList();
        }
        
        var localIdsHashSet = localEntities.Select(e => e.Id).ToHashSet();
        var sourceEntitiesDict = sourceEntities.ToDictionary(e => e.Id);

        // updated entities
        List<TEntity> newOrUpdatedEntities = localEntities
            .Where(localEntity =>
                sourceEntitiesDict.TryGetValue(localEntity.Id, out var sourceEntity) &&
                localEntity.LastSynchronizedAt < sourceEntity.LastModifiedAt)
            .Select(localEntity => sourceEntitiesDict[localEntity.Id])
            .ToList();

        // new entities
        newOrUpdatedEntities.AddRange(sourceEntities
            .Where(e => !localIdsHashSet.Contains(e.Id)));

        return newOrUpdatedEntities;
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