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
    private readonly IPublicationsRepository _publicationsRepository;
    private readonly ICollectionsRepository _collectionsRepository;
    
    private const string Schema = "http://www.sitemaps.org/schemas/sitemap/0.9";
    private static readonly Dictionary<string, double> StaticPages = new()
    {
        {"/", 1.0},
        { "/about", 0.9 },
        { "/submissions", 0.9 },
        { "/team", 0.9 }
    };
    
    public SiteMapController(
        IPublicationsRepository publicationsRepository, 
        ICollectionsRepository collectionsRepository)
    {
        _publicationsRepository = publicationsRepository;
        _collectionsRepository = collectionsRepository;
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
        var publicationsMetadata = await _publicationsRepository
            .GetSiteMapMetadataAsync();
        var collectionsMetadata = await _collectionsRepository
            .GetSiteMapMetadataAsync();

        IEnumerable<IEnumerable<XElement>> urls =
        [
            StaticPages.Select(page 
                => CreateStaticResourceXmlUrl(resourceUrl: baseUrl + page.Key, priority: page.Value)),
            
            publicationsMetadata.Select(meta 
                => CreateDynamicResourceXmlUrl(GetPublicationUrl(baseUrl, meta.Id), meta.LastModifiedAt)),
            
            collectionsMetadata.Select(meta 
                => CreateDynamicResourceXmlUrl(GetCollectionUrl(baseUrl, meta.Id), meta.LastModifiedAt))
        ];

        XElement xml = new((XNamespace)Schema + "urlset", urls.SelectMany(x => x));

        return new XDocument(xml);
    }
    
    private static XElement CreateStaticResourceXmlUrl(string resourceUrl, double priority)
    {
        return new XElement((XNamespace)Schema + "url",
                new XElement((XNamespace)Schema + "loc", resourceUrl),
                new XElement((XNamespace)Schema + "priority", priority));
    }
    
    private static XElement CreateDynamicResourceXmlUrl(
        string resourceUrl, DateTime lastModifiedAt, double priority = 0.8)
    {
        return new XElement((XNamespace)Schema + "url",
                new XElement((XNamespace)Schema + "loc", resourceUrl),
                new XElement((XNamespace)Schema + "lastmod", lastModifiedAt.ToString("yyyy-MM-dd")),
                new XElement((XNamespace)Schema + "priority", priority));
    }
    
    private static string GetPublicationUrl(string baseUrl, string id)
        => $"{baseUrl}/publications/{id}";
    
    private static string GetCollectionUrl(string baseUrl, string id)
        => $"{baseUrl}/collections/{id}";
}

public record BaseUrlDTO
{
    private string _baseUrl;
    
    [Required(AllowEmptyStrings = false)]
    [Url]
    public string BaseUrl
    {
        get => _baseUrl;
        init => _baseUrl = value.TrimEnd('/');
    }
};