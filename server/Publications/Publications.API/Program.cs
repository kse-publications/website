using Coravel;
using Coravel.Scheduling.Schedule.Interfaces;
using Microsoft.OpenApi.Models;
using Publications.API.Middleware;
using Notion.Client;
using Publications.API.BackgroundJobs;
using Publications.API.DTOs;
using Publications.API.Repositories;
using Publications.API.Repositories.Abstractions;
using Publications.API.Services;
using Redis.OM;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddLogging();
    builder.Services.AddControllers();
    
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

    builder.Services.AddSingleton<ErrorHandlingMiddleware>();

    builder.Services.AddSingleton(new RedisConnectionProvider(
        connectionString: builder.Configuration.GetConnectionString("Redis")!));

    builder.Services.AddHostedService<RedisHostedService>();
    builder.Services.AddScoped<IPublicationsRepository, PublicationsRepository>();
    builder.Services.AddScoped<IPublicationsService, PublicationsService>();
    
    builder.Services.AddScoped<IAuthorsRepository, AuthorsRepository>();
    builder.Services.AddScoped<IPublishersRepository, PublishersRepository>();
    
    builder.Services.Configure<NotionDatabaseOptions>(
        builder.Configuration.GetSection("Notion:Databases"));
    
    builder.Services.AddScoped<INotionClient>(provider => NotionClientFactory.Create(
        new ClientOptions{ AuthToken = builder.Configuration["Notion:AuthToken"] }));

    builder.Services.AddScoped<IPublicationsSourceRepository, NotionRepository>();

    builder.Services.Configure<RetriableTaskOptions>(
        builder.Configuration.GetSection("BackgroundTasks:SyncWithNotion"));
    builder.Services.AddTransient<SyncWithNotionBackgroundTask>();
    builder.Services.AddScheduler();
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.Services.UseScheduler(scheduler =>
    // {
    //     scheduler.Schedule<SyncWithNotionBackgroundTask>()
    //         .Hourly()
    //         .RunOnceAtStart()
    //         .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
    // });

    
    app.UseCors("FrontEndClient");
    app.UseHttpsRedirection();
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.MapControllers();

app.MapPost("/EB292BF0-E995-491A-A98E-6121601E1069/sync", 
    (ILogger<Program> logger, IScheduler scheduler) =>
{
    logger.LogInformation("/sync endpoint hit");
    scheduler.Schedule<SyncWithNotionBackgroundTask>()
        .EverySecond()
        .Once()
        .PreventOverlapping(nameof(SyncWithNotionBackgroundTask));
});

app.Run();