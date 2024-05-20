using Microsoft.AspNetCore.Mvc;
using Publications.API.Middleware;
using Publications.API.Serialization;
using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Services;
using Publications.Domain.Publications;
using Publications.Domain.Shared.Slugs;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Endpoints;

[ApiController]
[Route("publications")]
public class PublicationsController : ControllerBase
{
    private readonly IPublicationsService _publicationsService;

    public PublicationsController(
        IPublicationsService publicationsService)
    {
        _publicationsService = publicationsService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all publications",
        Description = "By default, returns latest publications (Descending order by LastModified date time).")]
    public async Task<IActionResult> GetAll(
        [FromQuery]FilterDTO filterDTO, 
        [FromQuery]PaginationDTO paginationDTO,
        CancellationToken cancellationToken)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsService
            .GetAllAsync(filterDTO, paginationDTO, cancellationToken);
            
        return Ok(publications);
    }
    
    [HttpGet("{id}")]
    [RequestAnalyticsFilter<Publication>]
    [ProducesResponseType(typeof(Publication), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get publication by its ID",
        Description = "Returns a single publication with the provided ID, if found."
    )]
    public async Task<IActionResult> GetById(
        [FromRoute]SlugDTO slug, CancellationToken cancellationToken)
    {
        Publication? publication = await _publicationsService
            .GetByIdAsync(slug.GetId(), cancellationToken);
        
        return publication is null
            ? NotFound($"Publication with ID '{slug.Slug}' not found.")
            : Ok(publication); 
    }
    
    [HttpGet("{id}/related-by-authors")]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get related publications by authors",
        Description = "Returns a list of publications that share " +
                      "at least one author with the provided publication."
    )]
    public async Task<IActionResult> GetRelatedByAuthors(
        [FromRoute] int id,
        [FromQuery] PaginationDTO paginationDTO,
        [FromQuery] AuthorFilterDTO authorFilterDto,
        CancellationToken cancellationToken)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsService
            .GetRelatedByAuthorsAsync(id, paginationDTO, authorFilterDto, cancellationToken);
        
        return Ok(publications);
    }
    
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>),StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "FTS, prefix match, fuzzy search publications",
        Description = "Searches for the SearchTerm in the publications' " +
                      "Title, Abstract, and Keywords, Publisher.Name, Author.Name."
    )]
    public async Task<IActionResult> GetBySearch(
        [FromQuery]FilterDTO filterDTO,
        [FromQuery]PaginationDTO paginationDTO,
        [FromQuery]SearchDTO searchDTO,
        CancellationToken cancellationToken)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsService
            .GetBySearchAsync(filterDTO, paginationDTO, searchDTO, cancellationToken);
            
        return Ok(publications);
    }
    
    [HttpGet("filters")]
    [ProducesResponseType(typeof(IReadOnlyCollection<FilterGroup>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all filters",
        Description = "Returns all FilterGroups for publications: Year, Type, Lang etc."
    )]
    public async Task<IActionResult> GetFilters(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<FilterGroup> filters = await _publicationsService
            .GetFiltersAsync(cancellationToken);
        
        return Ok(filters);
    }
    
    [HttpGet("filters/v2")]
    public async Task<IActionResult> GetFiltersWithCount(
        [FromQuery]FilterDTOV2 filterDtoV2,
        [FromQuery]PaginationDTO paginationDTO,
        [FromQuery]SearchDTO searchDTO,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<FilterGroup> filters = await _publicationsService
            .GetFiltersV2Async(filterDtoV2, paginationDTO, searchDTO, cancellationToken);
        
        return Ok(filters);
    }
}
