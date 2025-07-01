var builder = DistributedApplication.CreateBuilder(args);

var gameEngine = builder.AddProject<Projects.GameEngineService>("gameengine")
    .WithReplicas(1);

var gameSession = builder.AddProject<Projects.GameSessionService>("gamesession")
    .WithReference(gameEngine);

var webApp = builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(gameSession);

builder.Build().Run();