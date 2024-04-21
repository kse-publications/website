using Publications.API.Models;

namespace Publications.API.Services;

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