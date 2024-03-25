using Microsoft.OpenApi.Models;
using Notion.Client;
using Publications.API.BackgroundJobs;
using Publications.API.DTOs;
using Publications.API.Repositories;
using Redis.OM;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddLogging();
    builder.Services.AddControllers();
    
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Publications.API", Version = "v1" });
    });
    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("FrontEndClient",
            corsBuilder =>
            {
                corsBuilder.WithOrigins(builder.Configuration
                        .GetSection("AllowedCorsOrigins").Get<string[]>()!)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

    builder.Services.AddSingleton(new RedisConnectionProvider(
        connectionString: builder.Configuration.GetConnectionString("Redis")!));

    builder.Services.AddHostedService<RedisHostedService>();
    builder.Services.AddScoped<IPublicationsRepository, PublicationsRepository>();
    
    builder.Services.Configure<NotionDatabaseOptions>(
        builder.Configuration.GetSection("Notion:Databases"));
    
    builder.Services.AddScoped<NotionClient>(provider => NotionClientFactory.Create(
        new ClientOptions{ AuthToken = builder.Configuration["Notion:Token"] }));

    builder.Services.AddScoped<IPublicationsSourceRepository, NotionRepository>();

    builder.Services.AddHostedService<SyncWithNotionBackgroundTask>(
        provider => new SyncWithNotionBackgroundTask(
            provider.GetRequiredService<ILogger<SyncWithNotionBackgroundTask>>(),
            provider,
            interval: TimeSpan.FromHours(2),
            maxRetries: 3,
            retryDelay: TimeSpan.FromSeconds(5),
            runAtStartup: true));
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.UseCors("FrontEndClient");
    app.UseHttpsRedirection();
}

app.MapGet("/test" , () => "CD github workflow test!");

app.Run();