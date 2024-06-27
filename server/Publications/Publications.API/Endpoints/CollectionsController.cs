using Microsoft.AspNetCore.Mvc;
using Publications.API.Serialization;
using Publications.Application.DTOs.Request;
using Publications.Application.DTOs.Response;
using Publications.Application.Services;
using Publications.Domain.Collections;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Endpoints;

[ApiController]
[Route("collections")]
public class CollectionsController: ControllerBase
{
    private readonly ICollectionsService _collectionsService;

    public CollectionsController(ICollectionsService collectionsService)
    {
        _collectionsService = collectionsService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Collection>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all collections",
        Description = "Returns all collections.")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Collection> collections = await _collectionsService
            .GetAllAsync(cancellationToken);
        
        return Ok(collections);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CollectionData), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(
        Summary = "Get collection by its ID",
        Description = "Returns paginated publications from the collection with the provided ID.")]
    public async Task<IActionResult> GetById(
        [FromRoute]SlugDTO slug,
        [FromQuery]PaginationDTO paginationDTO,
        CancellationToken cancellationToken)
    {
        CollectionData? collection = await _collectionsService
            .GetByIdAsync(slug.GetId(), paginationDTO, cancellationToken);
        
        if (collection is null)
            return NotFound();
        
        return Ok(collection);
    }
}