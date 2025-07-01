using GameEngineService.Api;
using GameEngineService.Application;
using GameEngineService.Infrastructure;
using System.Text.Json.Serialization;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// This ensures enums are serialized as strings for clear API responses
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<IGameLockProvider, GameLockProvider>();
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
builder.Services.AddScoped<IGameLogic, GameLogic>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(); // This will serve the UI at the /swagger endpoint

app.MapDefaultEndpoints();
app.MapGamesEndpoints();

app.Run();