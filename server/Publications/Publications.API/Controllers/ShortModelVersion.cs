
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Publications.API.Repositories;
using Publications.API.Models;
using Publications.API.DTOs;
namespace Publications.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShortModelVersion(IPublicationsRepository repository) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<object>>> SearchPublications([FromQuery] PaginationSearchDTO paginationSearch)
        {
            try
            {
                
                var publications = await repository.GetByFullTextSearchAsync(paginationSearch);

                
                if (!publications.Any())
                {
                    return NotFound("No publications found.");
                }

                
                var shortPublication = publications.Select(publication => new 
                {
                    Id = publication.Id,
                    Title = publication.Title,
                    KeyWords = publication.Keywords,
                    Link = publication.Link
                });

                return Ok(shortPublication);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to search publications: {ex.Message}");
            }
        }
    }
}
