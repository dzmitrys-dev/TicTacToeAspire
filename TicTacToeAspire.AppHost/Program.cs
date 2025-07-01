var builder = DistributedApplication.CreateBuilder(args);

// Add game engine service, exposing an HTTP endpoint
var gameEngine = builder.AddProject<Projects.GameEngineService>("gameengine")
    .WithReplicas(1);

// Add game session service, which communicates with the game engine
var gameSession = builder.AddProject<Projects.GameSessionService>("gamesession")
    .WithReference(gameEngine);

// Add the Blazor Web App UI, which communicates with the game session service
builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(gameSession);

builder.Build().Run();