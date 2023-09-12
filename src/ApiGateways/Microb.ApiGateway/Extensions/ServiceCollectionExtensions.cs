using HealthChecks.ApplicationStatus.DependencyInjection;
using Microb.ApiGateway.Persistence;
using Microb.ApiGateway.Persistence.Entities;
using Microb.ApiGateway.Persistence.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;

namespace Microb.ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrobGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMicrobHealthCheck(configuration)
            .AddMicrobDatabases(configuration)
            .AddMicrobCors();
            //.AddMicrobIdentity();

        return services;
    }

    private static IServiceCollection AddMicrobHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        var usersDbConnectionString = configuration.GetConnectionString("UsersDatabase");
        if (string.IsNullOrWhiteSpace(usersDbConnectionString))
        {
            throw new ArgumentException("Connection string for UsersDatabase does not set");
        }

        services
            .AddHealthChecksUI()
            .AddInMemoryStorage();

        services.AddHealthChecks()
            .AddApplicationStatus(name: "api-gateway")
            .AddNpgSql(usersDbConnectionString, "SELECT 1", name: "identity-db");

        return services;
    }

    private static IServiceCollection AddMicrobDatabases(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("UsersDatabase"));
                options.UseOpenIddict<Guid>();
            }
        );

        return services;
    }

    private static IServiceCollection AddMicrobCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder
                        // .AllowCredentials()
                        .AllowAnyOrigin()
                        // .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }

    private static IServiceCollection AddMicrobIdentity(this IServiceCollection services)
    {
        services.AddIdentity<User, Role>()
            .AddSignInManager()
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddUserManager<UserManager<User>>();

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
        });

        services.AddOpenIddict()
            .AddCore(coreBuilder =>
                {
                    coreBuilder.UseEntityFrameworkCore()
                        .UseDbContext<UserDbContext>()
                        .ReplaceDefaultEntities<Guid>();
                }
            )
            .AddServer(serverBuilder =>
                {
                    serverBuilder.SetAuthorizationEndpointUris("/connect/authorize")
                        .SetDeviceEndpointUris("/connect/device")
                        .SetLogoutEndpointUris("/connect/logout")
                        .SetIntrospectionEndpointUris("/connect/introspect")
                        .SetTokenEndpointUris("/connect/token")
                        .SetUserinfoEndpointUris("/connect/userinfo")
                        .SetVerificationEndpointUris("/connect/verify");

                    serverBuilder.AllowAuthorizationCodeFlow()
                        .RequireProofKeyForCodeExchange();

                    serverBuilder.RegisterScopes(OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles);

                    serverBuilder.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    serverBuilder.UseAspNetCore()
                        .EnableStatusCodePagesIntegration()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserinfoEndpointPassthrough()
                        .EnableVerificationEndpointPassthrough()
                        .DisableTransportSecurityRequirement();

                    serverBuilder.UseReferenceAccessTokens();
                    serverBuilder.UseReferenceRefreshTokens();

                    serverBuilder.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
                    serverBuilder.SetRefreshTokenLifetime(TimeSpan.FromDays(7));
                }
            )
            .AddValidation(builder =>
            {
                builder.UseLocalServer();
                builder.UseAspNetCore();
                //TODO. Think about storing tokens in database.
                // For applications that need immediate access token or authorization
                // revocation, the database entry of the received tokens and their
                // associated authorizations can be validated for each API call.
                // Enabling these options may have a negative impact on performance.
                // options.EnableAuthorizationEntryValidation();
                // options.EnableTokenEntryValidation();
            });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
            options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
        });

        return services;
    }
}
