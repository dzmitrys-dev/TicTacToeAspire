var builder = WebApplication.CreateBuilder(args);

// Add YARP and load configuration
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Enable the reverse proxy
app.MapReverseProxy();

app.Run();