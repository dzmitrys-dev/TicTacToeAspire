using GameSessionService.Api;
using GameSessionService.Application;
using GameSessionService.Infrastructure;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
builder.Services.AddSingleton<IAutomatedPlayer, RandomPlayer>();
builder.Services.AddScoped<ISessionManager, SessionManager>();

builder.Services.AddHttpClient<IGameEngineClient, GameEngineClient>(client =>
{
    client.BaseAddress = new("http://gameengine");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(); // This will serve the UI at the /swagger endpoint

app.MapDefaultEndpoints();
app.MapSessionsEndpoints();

app.Run();