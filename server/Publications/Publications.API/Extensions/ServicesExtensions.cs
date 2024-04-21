using Coravel;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models; 
using Notion.Client;
using Publications.API.BackgroundJobs;
using Publications.API.BackgroundJobs.Abstractions;
using Publications.API.DTOs;
using Publications.API.Middleware;
using Publications.API.Models;
using Publications.API.Repositories.Authors;
using Publications.API.Repositories.Publications;
using Publications.API.Repositories.Publishers;
using Publications.API.Repositories.Requests;
using Publications.API.Repositories.Shared;
using Publications.API.Repositories.Source;
using Publications.API.Serialization;
using Publications.API.Services;
using Redis.OM;
using Redis.OM.Contracts;

namespace Publications.API.Extensions;

public static class ServicesExtensions
{
    public static IMvcBuilder ConfigureJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new ResponseJsonConverter<Publication>());
            options.JsonSerializerOptions.Converters.Add(new ResponseJsonConverter<Publisher>());
            options.JsonSerializerOptions.Converters.Add(new ResponseJsonConverter<Author>());
        });
    }
    
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Publications.API", Version = "v1" });
        });
    }
    
    public static void AddErrorHandlerMiddleware(this IServiceCollection services)
    {
        services.AddSingleton<ErrorHandlingMiddleware>();
    }
    
    public static void AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("FrontEndClient",
                corsBuilder =>
                {
                    corsBuilder.WithOrigins(configuration
                            .GetSection("AllowedCorsOrigins").Get<string[]>()!)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });
    }
    
    public static void UseCorsPolicies(this WebApplication app)
    {
        app.UseCors("FrontEndClient");
    }
    
    public static void UseErrorHandlerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
    
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IRedisConnectionProvider>(new RedisConnectionProvider(
            connectionString: configuration.GetConnectionString("Redis")!));

        services.AddHostedService<RedisHostedService>();
    }
    
    public static void AddNotionClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<NotionDatabaseOptions>(
            configuration.GetSection("Notion:Databases"));
        
        services.AddScoped<INotionClient>(provider => NotionClientFactory.Create(
            new ClientOptions{ AuthToken = configuration["Notion:AuthToken"] }));
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IPublicationsRepository, PublicationsRepository>();
        services.AddScoped<IPublicationsService, PublicationsService>();
        services.AddScoped<IAuthorsRepository, AuthorsRepository>();
        services.AddScoped<IPublishersRepository, PublishersRepository>();
        
        services.AddScoped<ISourceRepository, NotionRepository>();
        
        services.AddScoped<IRequestsRepository, RequestsRepository>();

        services.AddScoped<IEntityRepository<Publication>, EntityRepository<Publication>>();
        services.AddScoped<IEntityRepository<Publisher>, EntityRepository<Publisher>>();
        services.AddScoped<IEntityRepository<Author>, EntityRepository<Author>>();
    }
    
    public static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RetriableTaskOptions>(
            configuration.GetSection("BackgroundTasks:SyncWithNotion"));
        
        services.AddTransient<SyncWithNotionBackgroundTask>();
        services.AddScheduler();
        
        services.AddTransient<StoreRequestAnalyticsTask>();
        services.AddTransient<UpdateResourceViewsTask>();
        services.AddQueue();
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider)
    {
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncWithNotionBackgroundTask>()
                .Hourly()
                .RunOnceAtStart()
                .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));

            scheduler.Schedule<UpdateResourceViewsTask>()
                .Cron("0 */2 * * *") // Every 2 hours
                .RunOnceAtStart()
                .PreventOverlapping(nameof(UpdateResourceViewsTask));
        });
    }
    
    public static void AddSqliteDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RequestsHistoryDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("Sqlite"));
        });
    }
    
    public static void UpdateDatabase(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RequestsHistoryDbContext>();
        dbContext.Database.Migrate();
    }
}