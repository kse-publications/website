using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Publications.Infrastructure.Services;

namespace Publications.API.Endpoints;

[Route("sitemap.xml")]
public class SiteMapController : ControllerBase
{
    private readonly SiteMapService _siteMapService;

    public SiteMapController(SiteMapService siteMapService)
    {
        _siteMapService = siteMapService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetSiteMap([FromQuery(Name = "baseUrl")] string baseUrl)
    {
        XDocument siteMap = await _siteMapService.GetSiteMapXml(baseUrl);

        using MemoryStream memoryStream = new();
        siteMap.Save(memoryStream);

        return File(memoryStream.ToArray(), "application/xml");
    }
}