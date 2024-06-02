namespace Publications.Domain.Shared.ValueObjects;

public class EntityFilter
{
    public int GroupId { get; set; }
    public string PropertyName { get; set; }
    public SortOrder SortOrder { get; set; }

    public EntityFilter(
        int groupId,
        string propertyName,
        SortOrder sortOrder = SortOrder.None)
    {
        GroupId = groupId;
        PropertyName = propertyName;
        SortOrder = sortOrder;
    }
}

public enum SortOrder
{
    None,
    Ascending,
    Descending
}