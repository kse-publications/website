using System.ComponentModel.DataAnnotations;
using Publications.Application.Services;

namespace Publications.Infrastructure.Services.DbConfiguration;

public class RedisIndexesVersions
{
    [RegularExpression(@"^\d+\.\d+$")] 
    public string Publication { get; set; } = null!;
    
    [RegularExpression(@"^\d+\.\d+$")]
    public string Collection { get; set; } = null!;
    
    [RegularExpression(@"^\d+\.\d+$")]
    public string FilterGroup { get; set; } = null!;
    
    public DbVersion GetIndexVersion(Type type)
    {
        return DbVersion.FromString(GetType()
            .GetProperty(type.Name)!
            .GetValue(this)!
            .ToString()!);
    }
    
    public static string GetIndexName(Type type) => $"{type.Name.ToLower()}-idx"; 
}