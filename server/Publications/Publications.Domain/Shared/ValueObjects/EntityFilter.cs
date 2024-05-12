namespace Publications.Domain.Shared.ValueObjects;

public class EntityFilter
{
    public int GroupId { get; set; }
    public string PropertyName { get; set; }

    public EntityFilter(int groupId, string propertyName)
    {
        GroupId = groupId;
        PropertyName = propertyName;
    }
}