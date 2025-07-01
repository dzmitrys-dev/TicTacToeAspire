Console.OutputEncoding = System.Text.Encoding.UTF8;

var builder = DistributedApplication.CreateBuilder(args);

// Define the backend services
var gameEngine = builder.AddProject<Projects.GameEngineService>("gameengine").WithReplicas(1);
var gameSession = builder.AddProject<Projects.GameSessionService>("gamesession");

// The GameSessionService now needs a direct reference to the GameEngineService
gameSession.WithReference(gameEngine);

// The WebApp now needs a direct reference to the GameSessionService
builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(gameSession);

builder.Build().Run();