using Microsoft.AspNetCore.Mvc;
using Publications.API.Models;
using Publications.API.DTOs;
using Publications.API.Middleware;
using Publications.API.Repositories.Shared;
using Publications.API.Services;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Controllers;

[ApiController]
[Route("publications")]
public class PublicationsController : ControllerBase
{
    private readonly IPublicationsService _publicationsService;

    public PublicationsController(IPublicationsService publicationsService)
    {
        _publicationsService = publicationsService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all publications",
        Description = "By default, returns latest publications (Descending order by LastModified date time).")]
    public async Task<IActionResult> GetAll(
        [FromQuery]PublicationsPaginationFilterDTO paginationDTO, CancellationToken cancellationToken)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsService
            .GetAllAsync(paginationDTO, cancellationToken);
            
        return Ok(publications);
    } 
    
    [HttpGet("{id}")]
    [IdExtractionFilter]
    [RequestAnalyticsFilter<Publication>]
    [ProducesResponseType(typeof(Publication), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get publication by its ID",
        Description = "Returns a single publication with the provided ID, if found."
    )]
    public async Task<IActionResult> GetById(
        [FromRoute]string id, CancellationToken cancellationToken)
    {
        int parsedId = int.Parse(id);
        Publication? publication = await _publicationsService
            .GetByIdAsync(parsedId, cancellationToken);
        
        return publication is null
            ? NotFound($"Publication with ID {id} not found.")
            : Ok(publication); 
    }
    
    [HttpGet("search")]
    [ProducesResponseType(typeof(PaginatedCollection<PublicationSummary>),StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "FTS, prefix match, fuzzy search publications",
        Description = "Searches for the SearchTerm in the publications' " +
                      "Title, Abstract, and Keywords, Publisher.Name, Author.Name."
    )]
    public async Task<IActionResult> GetBySearch(
        [FromQuery]PublicationsPaginationSearchDTO searchDTO, CancellationToken cancellationToken)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsService
            .GetBySearchAsync(searchDTO, cancellationToken);
            
        return Ok(publications);
    }
}
