using Microb.ApiGateway.Extensions;
using Microb.ApiGateway.Persistence;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMicrobGateway(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowAllOrigins");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();
app.MapControllers();

app.UseHealthChecksUI(options =>
{
    options.UIPath = "/healthcheck-ui";
});

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    await dataContext.Database.EnsureCreatedAsync();
    //await DataSeeder.Seed(scope, CancellationToken.None);
}

await app.RunAsync();
