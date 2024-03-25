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
    public class ShortModelController(IPublicationsRepository repository) : ControllerBase
    {
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
    }
