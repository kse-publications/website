using Microsoft.EntityFrameworkCore;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using Publications.Application.Services;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Publications.Infrastructure.Requests;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace Publications.Infrastructure.Services.DbConfiguration;

public class DbConfigurationService : IDbConfigurationService
{
    private readonly IRedisConnectionProvider _redisConnectionProvider;
    private readonly SearchCommands _ft;
    private readonly RequestsHistoryDbContext _dbContext;
    private readonly IDbVersionService _versionService;

    public DbConfigurationService(
        IConnectionMultiplexer connectionMultiplexer,
        RequestsHistoryDbContext dbContext,
        IDbVersionService versionService)
    {
        _redisConnectionProvider = new RedisConnectionProvider(connectionMultiplexer);
        _ft = connectionMultiplexer.GetDatabase().FT();
        _dbContext = dbContext;
        _versionService = versionService;
    }

    public async Task ConfigureAsync()
    {
        await ConfigureRedisAsync();
        await ConfigureSqliteAsync();
    }

    private async Task ConfigureRedisAsync()
    {
        var indexes = (await _redisConnectionProvider.Connection
            .ExecuteAsync("FT._LIST")).ToArray();
        
        await CreateIndexesAsync(indexes,
            typeof(Publication),
            typeof(Collection),
            typeof(FilterGroup));
    }
    
    private async Task ConfigureSqliteAsync()
    {
        await _dbContext.Database.MigrateAsync();   
    }

    private async Task CreateIndexesAsync(RedisReply[] indexes, params Type[] types)
    {
        Dictionary<string, Type> indexesToCreate = types
            .ToDictionary(RedisIndexesVersions.GetIndexName);
        
        foreach (var currentIndex in indexesToCreate.Keys)
        {
            if (indexes.All(i => i != currentIndex))
            {
                await CreateIndexAsync(type: indexesToCreate[currentIndex]);
                continue;
            }
            
            DbVersion currentVersion = _versionService.GetCurrentIndexVersionAsync(indexesToCreate[currentIndex]);
            DbVersion dbVersion = await _versionService.GetDbIndexVersionAsync(indexesToCreate[currentIndex]);
            
            if (currentVersion == dbVersion)
                continue;
            
            bool deleteAssociatedData = currentVersion.Major != dbVersion.Major;
            
            await _ft.DropIndexAsync(currentIndex, dd: deleteAssociatedData);
            await CreateIndexAsync(type: indexesToCreate[currentIndex]);
        }
        
        await DeleteUnusedIndexes(indexes, indexesToCreate);
    }

    private async Task CreateIndexAsync(Type type)
    {
        await _redisConnectionProvider.Connection.CreateIndexAsync(type);
        
        var versionToSet = _versionService.GetCurrentIndexVersionAsync(type);
        await _versionService.UpdateDbIndexVersionAsync(type, versionToSet);
    }
    
    private async Task DeleteUnusedIndexes(
        RedisReply[] indexes, Dictionary<string, Type> indexesToCreate)
    {
        foreach (var existingIndex in indexes)
        {
            if (indexesToCreate.ContainsKey(existingIndex))
                continue;
            
            await _ft.DropIndexAsync(existingIndex, dd: true);
            await _versionService.DeleteDbIndexVersionAsync(indexesToCreate[existingIndex]);
        }
    }
}