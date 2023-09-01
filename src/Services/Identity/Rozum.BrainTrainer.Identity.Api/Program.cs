using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Rozum.BrainTrainer.Identity.Api.Extensions;
using Rozum.BrainTrainer.Identity.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
//     {
//         options.LoginPath = "/account/login";
//     });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRozumHealthCheck(builder.Configuration);
builder.Services.AddRozumIdentity(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecksUI();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dataContext.Database.EnsureCreatedAsync();
    //await DataSeeder.Seed(scope, CancellationToken.None);
}

await app.RunAsync();
