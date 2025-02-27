﻿using System.Text.Json;
using Microsoft.OpenApi.Models;
using Publications.API.Middleware;
using Publications.API.Serialization;
using Publications.Domain.Collections;
using Publications.Domain.Filters;
using Publications.Domain.Publications;
using Redis.OM.Modeling;

namespace Publications.API;

public static class Installer
{
    public static void ConfigureJsonOptions(this IMvcBuilder builder)
    {
        builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.AddResponseJsonConverters();
        });

        builder.Services.AddSingleton(new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new DateTimeJsonConverter()
            }
        });
    }
    
    private static JsonSerializerOptions AddResponseJsonConverters(this JsonSerializerOptions options)
    {
        options.Converters.Add(new ResponseJsonConverter<Publication>());
        options.Converters.Add(new ResponseJsonConverter<Publisher>());
        options.Converters.Add(new ResponseJsonConverter<Author>());
        options.Converters.Add(new ResponseJsonConverter<FilterGroup>());
        options.Converters.Add(new ResponseJsonConverter<Collection>());
        
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
    
    public static void UseCorsPolicies(this WebApplication app)
    {
        app.UseCors("FrontEndClient");
    }
    
    public static void UseErrorHandlerMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
    }
    
    public static void AddMinimalApiLogger(this IServiceCollection services)
    {
        services.AddTransient<ILogger>(p =>
        {
            var logger = p.GetRequiredService<ILoggerFactory>();
            return logger.CreateLogger("Publications.API");
        });
    }
}