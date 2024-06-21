using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<IDbConfigurationService> _logger;
    
    private readonly Dictionary<string, Type> _indexesToCreate = new[]
    {
        typeof(Publication),
        typeof(Collection),
        typeof(FilterGroup)
    }.ToDictionary(RedisIndexesVersions.GetIndexName);
    
    public DbConfigurationService(
        IConnectionMultiplexer connectionMultiplexer,
        RequestsHistoryDbContext dbContext,
        IDbVersionService versionService,
        ILogger<IDbConfigurationService> logger)
    {
        _redisConnectionProvider = new RedisConnectionProvider(connectionMultiplexer);
        _ft = connectionMultiplexer.GetDatabase().FT();
        _dbContext = dbContext;
        _versionService = versionService;
        _logger = logger;
    }

    public async Task ConfigureAsync()
    {
        await ConfigureRedisAsync();
        await ConfigureSqliteAsync();
    }

    private async Task ConfigureRedisAsync()
    {
        var existingIndexes = (await _redisConnectionProvider.Connection
            .ExecuteAsync("FT._LIST")).ToArray();
        
        await DeleteUnusedIndexes(existingIndexes);
        await CreateIndexesAsync(existingIndexes);
    }
    
    private async Task ConfigureSqliteAsync()
    {
        await _dbContext.Database.MigrateAsync();   
    }

    private async Task CreateIndexesAsync(RedisReply[] indexes)
    {
        foreach (var currentIndex in _indexesToCreate.Keys)
        {
            DbVersion? currentVersion = _versionService
                .GetCurrentIndexVersion(_indexesToCreate[currentIndex]);

            if (currentVersion is null)
            {
                throw new InvalidOperationException($"Index version for {currentIndex} is not set.");
            }
            
            if (indexes.All(i => i != currentIndex))
            {
                await CreateIndexAsync(type: _indexesToCreate[currentIndex], version: currentVersion);
                _logger.LogInformation("Index '{index}' CREATED with version {version}", 
                    currentIndex, currentVersion);
                
                continue;
            }
            
            DbVersion dbVersion = await _versionService
                .GetDbIndexVersionAsync(_indexesToCreate[currentIndex]);
            
            if (dbVersion == currentVersion)
                continue;
            
            bool deleteAssociatedData = dbVersion.Major != currentVersion.Major;
            
            await _ft.DropIndexAsync(currentIndex, dd: deleteAssociatedData);
            await CreateIndexAsync(type: _indexesToCreate[currentIndex], version: currentVersion);
            
            _logger.LogInformation("Index '{index}' UPDATED from {dbV} to {currV}", 
                currentIndex, dbVersion, currentVersion);
        }
    }

    private async Task CreateIndexAsync(Type type, DbVersion version)
    {
        await _redisConnectionProvider.Connection.CreateIndexAsync(type);
        await _versionService.UpdateDbIndexVersionAsync(type, version);
    }
    
    private async Task DeleteUnusedIndexes(RedisReply[] indexes)
    {
        foreach (var existingIndex in indexes)
        {
            if (_indexesToCreate.ContainsKey(existingIndex))
                continue;
            
            await _ft.DropIndexAsync(existingIndex, dd: true);
            await _versionService.DeleteDbIndexVersionAsync(_indexesToCreate[existingIndex]);
            _logger.LogInformation("Index '{index}' DELETED", (string)existingIndex);
        }
    }
}