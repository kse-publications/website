using Microsoft.Extensions.Options;
using Publications.Application.Services;
using StackExchange.Redis;

namespace Publications.Infrastructure.Services.DbConfiguration;

public class DbVersionService: IDbVersionService
{
    private readonly IDatabase _db;
    private readonly RedisIndexVersionInfo _redisIndexVersionInfo;

    public DbVersionService(
        IConnectionMultiplexer connectionMultiplexer,
        IOptions<RedisIndexVersionInfo> redisIndexesVersions)
    {
        _db = connectionMultiplexer.GetDatabase();
        _redisIndexVersionInfo = redisIndexesVersions.Value;
    }
    
    public async Task UpdateDbIndexVersionAsync(Type indexType, DbVersion dbVersion)
    {
        var indexName = RedisIndexVersionInfo.GetIndexName(indexType);
        await _db.StringSetAsync(DbVersion.GetIndexVersionKey(indexName), dbVersion.ToString());
    }
    
    public async Task DeleteDbIndexVersionAsync(Type indexType)
    {
        var indexName = RedisIndexVersionInfo.GetIndexName(indexType);
        await _db.KeyDeleteAsync(DbVersion.GetIndexVersionKey(indexName));
    }

    public async Task<DbVersion> GetDbIndexVersionAsync(Type type)
    {
        var indexName = RedisIndexVersionInfo.GetIndexName(type);
        RedisValue dbVersionValue = await _db.StringGetAsync(DbVersion.GetIndexVersionKey(indexName));
        return DbVersion.FromString(dbVersionValue.IsNullOrEmpty ? "0.0" : dbVersionValue.ToString());
    }

    public DbVersion? GetCurrentIndexVersion(Type type)
    {
        return _redisIndexVersionInfo.GetIndexVersion(type);
    }
}