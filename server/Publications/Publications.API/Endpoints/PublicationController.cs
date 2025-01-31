using Microsoft.AspNetCore.Mvc;
using Publications.API.Middleware;
using Publications.API.Serialization;
using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Application.Services;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
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
    [TypeFilter(typeof(RequestAnalyticsFilter<Publication>))]
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
        
        return publication is not null
            ? Ok(publication) 
            : NotFound(new
            {
                Error = "not_found",
                Message = $"Publication with ID '{slug.Slug}' not found."
            }); 
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
    
    [HttpGet("{id}/similar")]
    [ProducesResponseType(typeof(IReadOnlyCollection<PublicationSummary>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get similar publications",
        Description = "Finds k nearest publications using vector similarity search."
    )]
    public async Task<IActionResult> GetSimilar(
        [FromRoute] int id,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<PublicationSummary> publications = await _publicationsService
            .GetSimilarAsync(id, cancellationToken);
        
        return Ok(publications);
    }
    
    [HttpGet("search")]
    [TypeFilter(typeof(SearchAnalyticsFilter))]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>),StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "FTS & prefix match of publications",
        Description = "Searches for the SearchTerm in the publications' " +
                      "Title, Abstract, Publisher.Name, Authors.Name."
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
        Summary = "Get filters for publications",
        Description = "Returns a list of filters (including matchedPublicationCount)."
    )]
    public async Task<IActionResult> GetFilters(
        [FromQuery]FilterDTO filterDTO,
        [FromQuery]PaginationDTO paginationDTO,
        [FromQuery]SearchDTO searchDTO,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<FilterGroup> filters = await _publicationsService
            .GetFiltersAsync(filterDTO, paginationDTO, searchDTO, cancellationToken);
        
        return Ok(filters);
    }
}
