using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Publications.API.Repositories;
using Publications.API.Models;
using Publications.API.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Controllers;

[ApiController]
[Route("[controller]")]

public class PublicationController(IPublicationsRepository repository) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Get all publications", Description = "Retrieves all publications with optional pagination.")]
    [ProducesResponseType(typeof(IEnumerable<Publication>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Publication>>> GetAllPublications([FromQuery] PaginationDTO pagination)
    {
        var publications = await repository.GetAllAsync(pagination);

        if (publications.Count == 0)
        {
            return NotFound("No publications found.");
        }

        return Ok(publications);
    }
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Publication), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Get publication by ID",
            Description = "Retrieves a single publication based on its unique identifier."
        )]
        public async Task<ActionResult<Publication>> GetPublicationById(Guid id)
        {
            var publication = await repository.GetByIdAsync(id);

            if (publication == null)
            {
                return NotFound($"Publication with ID {id} not found.");
            }
            return Ok(publication); 
        }
        [HttpGet("search")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Search publications",
            Description = "Searches for publications based on the provided criteria."
        )]
        
        public async Task<ActionResult<IEnumerable<object>>> SearchPublications([FromQuery] PaginationSearchDTO paginationSearch)
        {
            var publications = await repository.GetByFullTextSearchAsync(paginationSearch);
                
            if (!publications.Any())
            {
                return NotFound("No publications found.");
            }

            var shortPublications = publications.Select(pub => new PublicationSummary());
                
            return Ok(shortPublications);
        }
        
        [HttpGet("autocomplete")]
        [ProducesResponseType(typeof(IEnumerable<PublicationSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Search publications by autocomplete",
            Description = "Performs an autocomplete search for publications based on a partial search term."
        )]
        public async Task<ActionResult<IEnumerable<Publication>>> GetByAutoCompleteAsync([FromQuery] PaginationSearchDTO paginationSearchDto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(paginationSearchDto.SearchTerm))
            {
                return BadRequest("Search term is required.");
            }

            var publications = await repository.GetByAutoCompleteAsync(paginationSearchDto, cancellationToken);

            if (!publications.Any())
            {
                return NotFound("No publications found.");
            }

            return Ok(publications.Select(p => new { p.Id, p.Title }));
        }
    }
