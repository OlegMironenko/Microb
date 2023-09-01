using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using Rozum.BrainTrainer.Identity.Api.Persistence;
using Rozum.BrainTrainer.Identity.Api.Persistence.Entities;
using Rozum.BrainTrainer.Identity.Api.Persistence.Stores;
using HealthChecks.UI.Core;

namespace Rozum.BrainTrainer.Identity.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRozumIdentity(this IServiceCollection services, IConfiguration configuration)
    {
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
                    serverBuilder.AllowAuthorizationCodeFlow()
                        .RequireProofKeyForCodeExchange();

                    serverBuilder.SetAuthorizationEndpointUris("/authorize")
                        .SetTokenEndpointUris("/token");

                    serverBuilder.UseReferenceAccessTokens();
                    serverBuilder.UseReferenceRefreshTokens();

                    serverBuilder.RegisterScopes(OpenIddictConstants.Permissions.Scopes.Email,
                        OpenIddictConstants.Permissions.Scopes.Profile,
                        OpenIddictConstants.Permissions.Scopes.Roles);

                    serverBuilder.SetAccessTokenLifetime(TimeSpan.FromMinutes(30));
                    serverBuilder.SetRefreshTokenLifetime(TimeSpan.FromDays(7));

                    serverBuilder.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    serverBuilder.UseAspNetCore()
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough();
                }
            )
            .AddValidation(builder =>
            {
                builder.UseLocalServer();
                builder.UseAspNetCore();
            });

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictConstants.Schemes.Bearer;
            options.DefaultChallengeScheme = OpenIddictConstants.Schemes.Bearer;
        });

        services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("UsersDatabase"));
                options.UseOpenIddict<Guid>();
            }
        );

        services.Configure<IdentityOptions>(options =>
        {
            options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
            options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
            options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
        });

        services.AddIdentity<User, Role>()
            .AddSignInManager()
            .AddUserStore<UserStore>()
            .AddRoleStore<RoleStore>()
            .AddUserManager<UserManager<User>>();

        return services;
    }

    public static IServiceCollection AddRozumHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddApplicationStatus()
            .AddNpgSql(configuration.GetConnectionString("UsersDatabase"));

        services.AddHealthChecksUI(setupSettings: setup => { setup.SetEvaluationTimeInSeconds(5); })
            .AddInMemoryStorage();

        return services;
    }
}
