using Publications.API.DTOs;
using Publications.API.Repositories;
using Publications.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Publications.API.Controllers;

    public class LatestPublicationsController(IPublicationsRepository repository) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publication>>> GetAllPublications([FromQuery] PaginationDTO pagination)
        {
                var publications = await repository.GetAllAsync(pagination);

                if (publications.Count == 0)
                {
                    return NotFound("No publications found.");
                }

                return Ok(publications);
        }
    }