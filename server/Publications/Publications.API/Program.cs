using Publications.API;
using Publications.BackgroundJobs;
using Publications.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddLogging();
    builder.Services.AddControllers().ConfigureJsonOptions();
    builder.Services.AddSwagger();
    
    builder.Services.AddCorsPolicies(builder.Configuration);
    builder.Services.AddErrorHandlerMiddleware();

    builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddBackgroundJobs(builder.Configuration);
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
    app.UseErrorHandlerMiddleware();
}

app.MapControllers();

app.Run();