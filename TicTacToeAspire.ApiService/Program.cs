using GameEngineService.Api;
using GameEngineService.Application;
using GameEngineService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
builder.Services.AddScoped<IGameLogic, GameLogic>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapDefaultEndpoints();
app.MapGamesEndpoints();

app.Run();