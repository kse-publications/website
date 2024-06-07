namespace Publications.Application.Services;

public interface IDbVersionService
{
    Task<bool> IsMajorVersionUpToDateAsync(Type indexType);
    Task UpdateDbIndexVersionAsync(Type indexType, DbVersion dbVersion);
    Task DeleteDbIndexVersionAsync(Type indexType);
    Task<DbVersion> GetDbIndexVersionAsync(Type type);
    DbVersion GetCurrentIndexVersionAsync(Type type);
}