using Microsoft.AspNetCore.Mvc;
using Publications.API.Serialization;
using Publications.Application;
using Publications.Application.DTOs;
using Publications.Application.Repositories;
using Publications.Domain.Collections;
using Publications.Domain.Publications;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Endpoints;

[ApiController]
[Route("collections")]
public class CollectionsController: ControllerBase
{
    private readonly ICollectionsRepository _collectionsRepository;
    private readonly IPublicationsQueryRepository _publicationsQueryRepository;

    public CollectionsController(
        ICollectionsRepository collectionsRepository,
        IPublicationsQueryRepository publicationsQueryRepository)
    {
        _collectionsRepository = collectionsRepository;
        _publicationsQueryRepository = publicationsQueryRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<Collection>), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get all collections",
        Description = "Returns all collections.")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<Collection> collections = await _collectionsRepository
            .GetAllAsync(cancellationToken);
        
        return Ok(collections);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Collection), StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get collection by its ID",
        Description = "Returns paginated publications from the collection with the provided ID.")]
    public async Task<IActionResult> GetById(
        [FromRoute]SlugDTO slug,
        [FromQuery]PaginationDTO paginationDTO,
        CancellationToken cancellationToken)
    {
        PaginatedCollection<PublicationSummary> publications = await _publicationsQueryRepository
            .GetFromCollectionAsync(slug.GetId(), paginationDTO, cancellationToken);
        
        return Ok(publications);
    }
}