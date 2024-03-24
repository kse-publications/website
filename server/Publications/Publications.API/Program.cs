
using Microsoft.OpenApi.Models;
using Publications.API;
using Publications.API.Notion;
using Publications.API.Repositories;
using Redis.OM;

var builder = WebApplication.CreateBuilder(args);
{
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