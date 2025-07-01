using GameSessionService.Api;
using GameSessionService.Application;
using GameSessionService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISessionRepository, InMemorySessionRepository>();
builder.Services.AddSingleton<IAutomatedPlayer, RandomPlayer>();
builder.Services.AddScoped<ISessionManager, SessionManager>();
builder.Services.AddHttpClient<IGameEngineClient, GameEngineClient>(client =>
{
    client.BaseAddress = new("http://apigateway");
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapDefaultEndpoints();
app.MapSessionsEndpoints();

app.Run();