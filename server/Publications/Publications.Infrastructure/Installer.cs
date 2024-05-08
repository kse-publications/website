using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notion.Client;
using Publications.Application.Repositories;
using Publications.Application.Services;
using Publications.Domain.Shared;
using Publications.Infrastructure.Authors;
using Publications.Infrastructure.Filters;
using Publications.Infrastructure.Publications;
using Publications.Infrastructure.Publishers;
using Publications.Infrastructure.Requests;
using Publications.Infrastructure.Shared;
using Publications.Infrastructure.Source;
using Redis.OM;
using Redis.OM.Contracts;

namespace Publications.Infrastructure;

public static class Installer
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddRedis(configuration)
            .AddSqliteDb(configuration)
            .AddRepositories()
            .AddServices()
            .AddNotionClient(configuration);
        
        return services;
    }
    
    private static IServiceCollection AddRedis(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(
            connectionString: configuration.GetConnectionString("Redis")!));
        services.AddTransient<IDbConfigurationService, RedisConfigurationService>();
        
        return services;
    }
    
    private static IServiceCollection AddNotionClient(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<NotionDatabaseOptions>(
            configuration.GetSection("Notion:Databases"));
        
        services.AddScoped<INotionClient>(provider => NotionClientFactory.Create(
            new ClientOptions{ AuthToken = configuration["Notion:AuthToken"] }));
        
        return services;
    }
    
    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPublicationsRepository, PublicationsRepository>();
        services.AddScoped<IAuthorsRepository, AuthorsRepository>();
        services.AddScoped<IPublishersRepository, PublishersRepository>();
        
        services.AddScoped<ISourceRepository, NotionRepository>();
        
        services.AddScoped<IRequestsRepository, RequestsRepository>();

        services.AddScoped<IFiltersRepository, FiltersRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPublicationsService, PublicationsService>();
        services.AddScoped<IFiltersService, FiltersService>();
        services.AddScoped<IWordsService, WordsService>();
        
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

    public static void UpdateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RequestsHistoryDbContext>();
        dbContext.Database.Migrate();
    }
}