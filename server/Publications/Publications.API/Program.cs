
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontEndClient");
app.UseHttpsRedirection();

app.MapGet("/test" , () => "CD github workflow test");

app.Run();