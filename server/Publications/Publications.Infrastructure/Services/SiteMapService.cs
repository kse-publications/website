using System.Xml.Linq;
using Publications.Application.Repositories;

namespace Publications.Infrastructure.Services;

public class SiteMapService
{
    private readonly IPublicationsQueryRepository _publicationsRepository;

    public SiteMapService(IPublicationsQueryRepository publicationsRepository)
    {
        _publicationsRepository = publicationsRepository;
    }
    
    public async Task<XDocument> GetSiteMapXml(string baseUrl)
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