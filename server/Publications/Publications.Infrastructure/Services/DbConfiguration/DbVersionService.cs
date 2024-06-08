using Microsoft.Extensions.Options;
using Publications.Application.Services;
using StackExchange.Redis;

namespace Publications.Infrastructure.Services.DbConfiguration;

public class DbVersionService: IDbVersionService
{
    private readonly IDatabase _db;
    private readonly RedisIndexesVersions _redisIndexesVersions;

    public DbVersionService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisIndexesVersions> redisIndexesVersions)
    {
        _db = connectionMultiplexer.GetDatabase();
        _redisIndexesVersions = redisIndexesVersions.Value;
    }

    public async Task<bool> IsMajorVersionUpToDateAsync(Type indexType)
    {
        var currentVersion = _redisIndexesVersions.GetIndexVersion(indexType);
        var dbVersion = await GetDbIndexVersionAsync(indexType);

        return currentVersion.Major == dbVersion.Major;
    }
    
    public async Task UpdateDbIndexVersionAsync(Type indexType, DbVersion dbVersion)
    {
        var indexName = RedisIndexesVersions.GetIndexName(indexType);
        await _db.StringSetAsync(DbVersion.GetIndexVersionKey(indexName), dbVersion.ToString());
    }
    
    public async Task DeleteDbIndexVersionAsync(Type indexType)
    {
        var indexName = RedisIndexesVersions.GetIndexName(indexType);
        await _db.KeyDeleteAsync(DbVersion.GetIndexVersionKey(indexName));
    }

    public async Task<DbVersion> GetDbIndexVersionAsync(Type type)
    {
        var indexName = RedisIndexesVersions.GetIndexName(type);
        RedisValue dbVersionValue = await _db.StringGetAsync(DbVersion.GetIndexVersionKey(indexName));
        return DbVersion.FromString(dbVersionValue.IsNullOrEmpty ? "0.0" : dbVersionValue.ToString());
    }

    public DbVersion GetCurrentIndexVersion(Type type)
    {
        return _redisIndexesVersions.GetIndexVersion(type);
    }
}