using HealthChecks.UI.Client;
using Microb.Identity.Api.Extensions;
using Microb.Identity.Api.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMicrobIdentity(builder.Configuration);
builder.Services.AddMicrobHealthCheck(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseHealthChecks("/healthcheck", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dataContext.Database.EnsureCreatedAsync();
    //await DataSeeder.Seed(scope, CancellationToken.None);
}

await app.RunAsync();
