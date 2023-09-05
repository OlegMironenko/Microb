using Microb.ApiGateway.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddMicrobHealthCheck(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();

app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthcheck";
});

await app.RunAsync();
