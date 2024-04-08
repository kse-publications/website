using Publications.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddLogging();
    builder.Services.AddControllers().ConfigureJsonOptions();
    builder.Services.AddSwagger();
    
    builder.Services.AddCorsPolicies(builder.Configuration);
    builder.Services.AddErrorHandlerMiddleware();

    builder.Services.AddRedis(builder.Configuration);
    builder.Services.AddSqliteDb(builder.Configuration);
    
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
    
    app.Services.UpdateDatabase();
    
    app.Services.UseBackgroundJobs();
    
    app.UseCorsPolicies();
    app.UseHttpsRedirection();
    app.UseErrorHandlerMiddleware();
}

app.MapControllers();
app.MapSyncEndpoint();

app.Run();