using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Publications.API.Repositories;
using Publications.API.DTOs;
using Publications.API.Models;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FullModelController(IPublicationsRepository repository) : ControllerBase
    {
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
    }
}
