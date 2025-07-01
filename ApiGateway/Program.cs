var builder = WebApplication.CreateBuilder(args);

// This is critical: It adds Aspire's service discovery components.
builder.AddServiceDefaults();

// Add YARP and explicitly configure it to load its routing rules
// from the "ReverseProxy" section of your appsettings.json file.
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Add default health checks and other endpoints.
app.MapDefaultEndpoints();

// Launch the reverse proxy.
app.MapReverseProxy();

app.Run();