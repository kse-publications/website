using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Publications.API.Middleware;
using Publications.API.Serialization;
using Publications.Application;
using Publications.Domain.Filters;
using Publications.Domain.Publications;

namespace Publications.API;

public static class Installer
{
    public static IMvcBuilder ConfigureJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.AddResponseJsonConverters();
        });
    }
    
    private static JsonSerializerOptions AddResponseJsonConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new ResponseJsonConverter<Publication>());
        options.Converters.Add(new ResponseJsonConverter<Publisher>());
        options.Converters.Add(new ResponseJsonConverter<Author>());
        options.Converters.Add(new ResponseJsonConverter<FilterGroup>());
        
        return options;
    }
    
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Publications.API", Version = "v1" });
            options.SchemaFilter<SwaggerIgnoreFilter>();
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
    
    public static void ConfigureFeatureFlags(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<FeatureFlags>(configuration.GetSection("FeatureFlags"));
    }
    
    public static IOptionsMonitor<FeatureFlags> GetFeatureFlagsMonitor(this WebApplication app)
    {
        return app.Services.GetRequiredService<IOptionsMonitor<FeatureFlags>>();
    }
    
    public static void UseCorsPolicies(this WebApplication app)
    {
        app.UseCors("FrontEndClient");
    }
    
    public static void UseErrorHandlerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
}