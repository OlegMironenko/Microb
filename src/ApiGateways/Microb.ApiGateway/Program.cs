using HealthChecks.UI.Client;
using Microb.ApiGateway.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMicrobHealthCheck()
    .AddMicrobGateway(builder.Configuration)
    .AddMicrobIdentity();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("test_policy", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "profile"));

var app = builder.Build();

app.UseHsts();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHealthChecksUI(options => { options.UIPath = "/healthcheck-ui"; });

app.MapGet("/test", () => "Hello World!")
    .RequireAuthorization("test_policy");

await app.RunAsync();
