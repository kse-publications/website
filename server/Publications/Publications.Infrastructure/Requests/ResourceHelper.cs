using Publications.Domain.Authors;
using Publications.Domain.Publications;
using Publications.Domain.Publishers;

namespace Publications.Infrastructure.Requests;

public class ResourceHelper
{
    public static string GetResourceName<TResource>()
    {
        return typeof(TResource).Name.ToLower() + "s";
    }

    public static Type GetResourceType(string resourceName)
    {
        return resourceName switch
        {
            "publications" => typeof(Publication),
            "authors" => typeof(Author),
            "publishers" => typeof(Publisher),
            _ => throw new ArgumentException("Invalid resource name")
        };
    }
}