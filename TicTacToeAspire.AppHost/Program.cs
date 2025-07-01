var builder = DistributedApplication.CreateBuilder(args);

var gameEngine = builder.AddProject<Projects.GameEngineService>("gameengine");
var gameSession = builder.AddProject<Projects.GameSessionService>("gamesession");

var apiGateway = builder.AddProject<Projects.ApiGateway>("apigateway")
    .WithReference(gameEngine)
    .WithReference(gameSession);

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(apiGateway)
       .WithEnvironment("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");

builder.Build().Run();