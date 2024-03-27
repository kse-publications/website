using Coravel;
using Microsoft.OpenApi.Models;
using Notion.Client;
using Publications.API.BackgroundJobs;
using Publications.API.DTOs;
using Publications.API.Repositories.Authors;
using Publications.API.Repositories.Publications;
using Publications.API.Repositories.Publishers;
using Publications.API.Repositories.Source;
using Publications.API.Services;
using Redis.OM;

namespace Publications.API.Extensions;

public static class ServicesExtensions
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Publications.API", Version = "v1" });
        });
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
                        .AllowAnyMethod();
                });
        });
    }
    
    public static void UseCorsPolicies(this WebApplication app)
    {
        app.UseCors("FrontEndClient");
    }
    
    public static void AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new RedisConnectionProvider(
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
    }
    
    public static void AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RetriableTaskOptions>(
            configuration.GetSection("BackgroundTasks:SyncWithNotion"));
        
        services.AddTransient<SyncWithNotionBackgroundTask>();
        services.AddScheduler();
    }
    
    public static void UseBackgroundJobs(this IServiceProvider serviceProvider)
    {
        serviceProvider.UseScheduler(scheduler =>
        {
            scheduler.Schedule<SyncWithNotionBackgroundTask>()
                .Hourly()
                .RunOnceAtStart()
                .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
        });
    }
}