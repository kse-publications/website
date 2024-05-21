using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Publications.Application.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace Publications.API.Endpoints;

[ApiController]
[Route("sitemap.xml")]
public class SiteMapController : ControllerBase
{
    private readonly IPublicationsQueryRepository _publicationsRepository;

    public SiteMapController(IPublicationsQueryRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [SwaggerOperation(
        Summary = "Get site map",
        Description = "Returns a sitemap.xml with all publication URLs.")]
    public async Task<IActionResult> GetSiteMap([FromQuery]BaseUrlDTO urlDTO)
    {
        XDocument siteMap = await GetSiteMapXml(urlDTO.BaseUrl);

        using MemoryStream memoryStream = new();
        siteMap.Save(memoryStream);

        return File(memoryStream.ToArray(), "application/xml");
    }
    
    private async Task<XDocument> GetSiteMapXml(string baseUrl)
    {
        IReadOnlyCollection<string> slugs = await _publicationsRepository.GetAllSlugsAsync();
        
        const string schema = "http://www.sitemaps.org/schemas/sitemap/0.9";
        IEnumerable<XElement> urls = slugs
            .Select(slug => 
                new XElement((XNamespace)schema + "url", 
                    new XElement((XNamespace)schema + "loc", GetUrl(baseUrl, slug))));
        
        XElement xml = new((XNamespace)schema + "urlset", urls);

        return new XDocument(xml);
    }
    
    private static string GetUrl(string baseUrl, string slug)
        => $"{baseUrl}/publications/{slug}";
}

public record BaseUrlDTO
{
    [Required(AllowEmptyStrings = false)] 
    [Url]
    public string BaseUrl { get; init; } = null!;
};