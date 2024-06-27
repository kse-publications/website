using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notion.Client;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Shared.Slugs;
using Publications.Infrastructure.Publications;
using Publications.Infrastructure.Requests;
using Publications.Infrastructure.Services;
using Publications.Infrastructure.Services.DbConfiguration;
using Publications.Infrastructure.Source;
using Publications.Infrastructure.Statistics;
using Redis.OM;
using Redis.OM.Contracts;
using StackExchange.Redis;

namespace Publications.Infrastructure;

public static class Installer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddRedis(configuration)
            .AddSqliteDb(configuration)
            .AddDbConfigurationServices(configuration)
            .AddRepositories()
            .AddServices()
            .AddNotionClient(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionMultiplexer = ConnectionMultiplexer
            .Connect(configuration.GetConnectionString("Redis")!);
        
        services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
        
        services.AddSingleton<IRedisConnectionProvider>(
            new RedisConnectionProvider(connectionMultiplexer));
        
        return services;
    }
    
    private static IServiceCollection AddDbConfigurationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<RedisIndexesVersions>()
            .ValidateDataAnnotations()
            .Bind(configuration.GetSection("Redis:IndexesVersions"));
        
        services.AddTransient<IDbConfigurationService, DbConfigurationService>();
        
        return services;
    }
    
    private static IServiceCollection AddNotionClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptionsWithValidateOnStart<NotionDatabaseOptions>()
            .ValidateDataAnnotations()
            .Bind(configuration.GetSection("Notion:Databases"));
        
        string? authToken = configuration["Notion:AuthToken"];
        if (string.IsNullOrWhiteSpace(authToken))
            throw new InvalidOperationException("Notion AuthToken was not provided.");
        
        services.AddScoped<INotionClient>(provider => NotionClientFactory.Create(
            new ClientOptions{ AuthToken = authToken }));
        
        return services;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPublicationsRepository, PublicationsRepository>();
        services.AddScoped<IFiltersRepository, FiltersRepository>();
        services.AddScoped<ICollectionsRepository, CollectionsRepository>();
        
        services.AddScoped<ISourceRepository, NotionRepository>();
        
        services.AddScoped<IRequestsRepository, RequestsRepository>();
        services.AddScoped<IStatisticsRepository, StatisticsRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPublicationsService, PublicationsService>();
        services.AddScoped<ICollectionsService, CollectionsService>();
        services.AddScoped<IFiltersService, FiltersService>();
        services.AddScoped<IWordsService, WordsService>();
        services.AddScoped<IDbVersionService, DbVersionService>();
        
        return services;
    }
    
    private static IServiceCollection AddSqliteDb(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<RequestsHistoryDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("Sqlite"));
        });
        
        return services;
    }
}