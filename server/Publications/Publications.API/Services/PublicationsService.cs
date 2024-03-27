﻿using Publications.API.DTOs;
using Publications.API.Models;
using Publications.API.Repositories;
using Publications.API.Repositories.Publications;

namespace Publications.API.Services;

public class PublicationsService: IPublicationsService
{
    private readonly IPublicationsRepository _publicationsRepository;

    public PublicationsService(IPublicationsRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetAllAsync(
        PaginationFilterDTO paginationFilterDTO, CancellationToken cancellationToken = default)
    {
        PaginatedCollection<Publication> publications = await _publicationsRepository
            .GetAllAsync(paginationFilterDTO, cancellationToken);
        
        return GetPublicationsSummaries(publications);
    }

    public async Task<PaginatedCollection<PublicationSummary>> GetBySearchAsync(
        PaginationSearchDTO paginationSearchDTO, 
        CancellationToken cancellationToken = default)
    {
        const int minSearchTermLength = 2;
        
        if (paginationSearchDTO.SearchTerm.Length < minSearchTermLength)
            return EmptyResponse;
        
        PaginatedCollection<Publication> matchedPublications = await _publicationsRepository
            .GetBySearchAsync(
                paginationSearchDTO,
                paginationSearchDTO.CanFuzzyMatch(),
                cancellationToken);
        
        return GetPublicationsSummaries(matchedPublications);
    }

    public async Task<Publication?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _publicationsRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task InsertOrUpdateAsync(
        IEnumerable<Publication> publications,
        CancellationToken cancellationToken = default)
    {
        await _publicationsRepository.InsertOrUpdateAsync(
            publications, cancellationToken);
    }

    private static PaginatedCollection<PublicationSummary> GetPublicationsSummaries(
        PaginatedCollection<Publication> publications)
    {
        IReadOnlyCollection<PublicationSummary> summaries = publications.Items
            .Select(PublicationSummary.FromPublication)
            .ToList()
            .AsReadOnly();
        
        return new PaginatedCollection<PublicationSummary>(
            Items: summaries,
            ResultCount: publications.ResultCount,
            TotalCount: publications.TotalCount);
    }
    
    private static PaginatedCollection<PublicationSummary> EmptyResponse => 
        new PaginatedCollection<PublicationSummary>(
            Items: new List<PublicationSummary>(), ResultCount: 0, TotalCount: 0);
}