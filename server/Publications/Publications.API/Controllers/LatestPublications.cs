using Publications.API.DTOs;
using Publications.API.Repositories;
using Publications.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Publications.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LatestPublications(IPublicationsRepository repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publication>>> GetAllPublications([FromQuery] PaginationDTO pagination)
        {
            try
            {

                // Отримати всі публікації з використанням пагінації та відповідного сортування
                var publications = await repository.GetAllAsync(pagination);

                // Перевірка, чи є публікації
                if (publications.Count == 0)
                {
                    return NotFound("No publications found.");
                }

                return Ok(publications);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to retrieve publications: {ex.Message}");
            }
        }
    }
}