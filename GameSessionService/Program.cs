using GameSessionService.Api;
using GameSessionService.Application;
using GameSessionService.Infrastructure;
using Microsoft.OpenApi.Models; // Add this using statement

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

// --- Aspire and Service Defaults ---
builder.AddServiceDefaults();

// --- API and Swagger Configuration ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // This adds more details to your Swagger UI page
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "GameSessionService API",
        Description = "An API for managing Tic-Tac-Toe game sessions."
    });
});

// --- Dependency Injection ---
builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
builder.Services.AddSingleton<IAutomatedPlayer, RandomPlayer>();
builder.Services.AddScoped<ISessionManager, SessionManager>();

builder.Services.AddHttpClient<IGameEngineClient, GameEngineClient>(client =>
{
    client.BaseAddress = new("http://gameengine");
});

var app = builder.Build();

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    // Use the default UseSwagger() - no special configuration needed
    app.UseSwagger();

    // Configure the Swagger UI
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameSessionService v1");
        // Optional: Make Swagger UI the default page
        c.RoutePrefix = string.Empty;
    });
}

app.MapDefaultEndpoints();
app.MapSessionsEndpoints();

app.Run();