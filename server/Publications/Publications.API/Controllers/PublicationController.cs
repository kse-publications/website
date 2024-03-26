using Microsoft.AspNetCore.Mvc;
using Publications.API.Repositories;
using Publications.API.Models;
using Publications.API.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Controllers;

[ApiController]
[Route("publications")]
public class PublicationsController : ControllerBase
{
    private readonly IPublicationsRepository _publicationsRepository;

    public PublicationsController(IPublicationsRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedCollection<Publication>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all publications",
        Description = "By default, returns latest publications (Descending order by LastModified date time).")]
    public async Task<IActionResult> GetAll(
        [FromQuery]PaginationDTO paginationDto, CancellationToken cancellationToken)
    {
        PaginatedCollection<Publication> publications = await _publicationsRepository
            .GetAllAsync(paginationDto, cancellationToken);
        
        return Ok(publications);
    } 
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Publication), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get publication by its ID",
        Description = "Returns a single publication with the provided ID, if found."
    )]
    public async Task<IActionResult> GetById(
        [FromRoute]Guid id, CancellationToken cancellationToken)
    {
        Publication? publication = await _publicationsRepository
            .GetByIdAsync(id, cancellationToken);
        
        return publication is null
            ? NotFound($"Publication with ID {id} not found.")
            : Ok(publication); 
    }
    
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>),StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "FTS publications",
        Description = "Searches for the SearchTerm in the publications' Title, Abstract, and Keywords."
    )]
    public async Task<IActionResult> GetBySearch(
        [FromQuery]PaginationSearchDTO paginationSearch, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(paginationSearch.SearchTerm))
        {
            return Ok(new PaginatedCollection<PublicationSummary>(
                Array.Empty<PublicationSummary>(), 0));
        }
        
        PaginatedCollection<Publication> publications = await _publicationsRepository
            .GetByFullTextSearchAsync(paginationSearch, cancellationToken);

        IReadOnlyCollection<PublicationSummary> summaries = publications.Items
            .Select(PublicationSummary.FromPublication)
            .ToList()
            .AsReadOnly();
        
        PaginatedCollection<PublicationSummary> response = new(
            Items: summaries,
            Count: publications.Count);
            
        return Ok(response);
    }
    
    [HttpGet("auto-complete")]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Endpoint for autocomplete search",
        Description = "Performs Title.Contains() search on publications, sorts by relevance"
    )]
    public async Task<IActionResult> GetByAutoComplete(
        [FromQuery]PaginationSearchDTO paginationSearchDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(paginationSearchDto.SearchTerm))
        {
            return Ok(new PaginatedCollection<PublicationSummary>(
                Array.Empty<PublicationSummary>(), 0));
        }

        PaginatedCollection<Publication> publications = await _publicationsRepository
            .GetByAutoCompleteAsync(paginationSearchDto, cancellationToken);
        
        IReadOnlyCollection<PublicationSummary> summaries = publications.Items
            .Select(PublicationSummary.FromPublication)
            .ToList()
            .AsReadOnly();
        
        PaginatedCollection<PublicationSummary> response = new(
            Items: summaries,
            Count: publications.Count);
            
        return Ok(response);
    }
}
