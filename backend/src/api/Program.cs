using Microsoft.Extensions.Options;
using PlatformBanking.Agents;
using PlatformBanking.Api.Middleware;
using PlatformBanking.Models;
using PlatformBanking.Policies;
using PlatformBanking.Services.Audit;
using PlatformBanking.Services.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddOptions<StorageOptions>()
    .Bind(builder.Configuration.GetSection(StorageOptions.SectionName))
    .Validate(options => options.IsValid(), "Storage options are invalid.");

builder.Services.AddSingleton<IAgentCapabilityRegistry>(_ =>
    new AgentCapabilityRegistry(AgentCapabilityRegistry.CreateDefault()));
builder.Services.AddSingleton<IPolicyClassificationService, PolicyClassificationService>();
builder.Services.AddSingleton<IAuditWriter, AppendOnlyInMemoryAuditWriter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRequestContext();
app.UseAuthSession();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/health", (IOptions<StorageOptions> storageOptions) =>
{
    return Results.Ok(new
    {
        status = "ok",
        storageConfigValid = storageOptions.Value.IsValid()
    });
});

app.MapGet("/session", (HttpContext context) =>
{
    if (context.Items.TryGetValue(nameof(CustomerIdentitySession), out var session))
    {
        return Results.Ok(session);
    }

    return Results.Unauthorized();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
