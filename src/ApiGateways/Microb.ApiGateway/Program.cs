using Microb.ApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddMicrobHealthCheck(builder.Configuration);

var app = builder.Build();

app.MapReverseProxy();

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthcheck-ui";
});

await app.RunAsync();
