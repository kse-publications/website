using Publications.API.Middleware;
using Publications.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddLogging();
    builder.Services.AddControllers();
    builder.Services.AddSwagger();
    
    builder.Services.AddCorsPolicies(builder.Configuration);
    builder.Services.AddSingleton<ErrorHandlingMiddleware>();

    builder.Services.AddRedis(builder.Configuration);
    builder.Services.AddNotionClient(builder.Configuration);
    
    builder.Services.AddRepositories();

    builder.Services.AddBackgroundJobs(builder.Configuration);
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.Services.UseBackgroundJobs();
    
    app.UseCorsPolicies();
    app.UseHttpsRedirection();
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.MapControllers();
app.MapSyncEndpoint();

app.Run();