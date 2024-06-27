using Publications.Domain.Collections;

namespace Publications.Application.DTOs.Response;

public class CollectionData
{
    public Collection Collection { get; init; }
    public PaginatedCollection<PublicationSummary> Publications { get; init; }
    
    public CollectionData(
        Collection collection,
        PaginatedCollection<PublicationSummary> publications)
    {
        Collection = collection;
        Publications = publications;
    }
}