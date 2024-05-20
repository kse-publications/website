using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Publications.Infrastructure.Services;

namespace Publications.API.Endpoints;

[ApiController]
[Route("sitemap.xml")]
public class SiteMapController : ControllerBase
{
    private readonly SiteMapService _siteMapService;

    public SiteMapController(SiteMapService siteMapService)
    {
        _siteMapService = siteMapService;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSiteMap([FromQuery]BaseUrlDTO urlDTO)
    {
        XDocument siteMap = await _siteMapService.GetSiteMapXml(urlDTO.BaseUrl);

        using MemoryStream memoryStream = new();
        siteMap.Save(memoryStream);

        return File(memoryStream.ToArray(), "application/xml");
    }
}

public record BaseUrlDTO
{
    [Required(AllowEmptyStrings = false)] 
    [Url]
    public string BaseUrl { get; init; } = null!;
};