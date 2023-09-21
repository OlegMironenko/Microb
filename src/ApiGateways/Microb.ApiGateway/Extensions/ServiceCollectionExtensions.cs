using Microb.HealthChecks.Keycloak.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Microb.ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrobHealthCheck(this IServiceCollection services)
    {
        services
            .AddHealthChecks()
            .AddKeycloak();

        services
            .AddHealthChecksUI()
            .AddInMemoryStorage();

        return services;
    }

    public static IServiceCollection AddMicrobGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        return services;
    }

    public static IServiceCollection AddMicrobIdentity(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            })
            .AddOpenIdConnect(options =>
            {
                options.Authority = "http://microb-keycloak-server:8080/realms/microb";
                options.ClientId = "microb";
                options.ClientSecret = "";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    RoleClaimType = "roles"
                };
                options.RequireHttpsMetadata = false;
            });

        return services;
    }
}
