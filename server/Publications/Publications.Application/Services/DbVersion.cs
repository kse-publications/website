
namespace Publications.Application.Services;

public record DbVersion(int Major, int Minor)
{
    public static bool operator < (DbVersion v1, DbVersion v2) 
        => v1.Major < v2.Major || v1.Major == v2.Major && v1.Minor < v2.Minor;
    
    public static bool operator > (DbVersion v1, DbVersion v2)
        => v1.Major > v2.Major || v1.Major == v2.Major && v1.Minor > v2.Minor;
    
    public static DbVersion FromString(string version)
    {
        string[] parts = version.Split('.');
        return new DbVersion(int.Parse(parts[0]), int.Parse(parts[1]));
    }
    
    public static string GetIndexVersionKey(string indexName) => $"{indexName}-version";

    public override string ToString()
    {
        return $"{Major}.{Minor}";
    }
}