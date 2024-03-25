using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Publications.API.Repositories;
using Publications.API.Models;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Publications.API.DTOs;

namespace Publications.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutocompleteController(IPublicationsRepository _repository) : ControllerBase
    {
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

            var publications = await _repository.GetByAutoCompleteAsync(paginationSearchDto, cancellationToken);

            if (!publications.Any())
            {
                return NotFound("No publications found.");
            }

            return Ok(publications.Select(p => new { p.Id, p.Title }));
        }
    }
}
