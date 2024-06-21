namespace Publications.Application.Services;

public interface IDbVersionService
{
    Task UpdateDbIndexVersionAsync(Type indexType, DbVersion dbVersion);
    Task DeleteDbIndexVersionAsync(Type indexType);
    Task<DbVersion> GetDbIndexVersionAsync(Type type);
    DbVersion? GetCurrentIndexVersion(Type type);
}