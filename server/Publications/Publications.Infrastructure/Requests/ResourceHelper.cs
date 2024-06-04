using Publications.Domain.Collections;
using Publications.Domain.Publications;

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
            "collections" => typeof(Collection),
            _ => throw new ArgumentException("Invalid resource name")
        };
    }
}