using Microsoft.EntityFrameworkCore;
using Rozum.BrainTrainer.Identity.Api.Persistence;

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
                    //serverBuilder.AllowClientCredentialsFlow();
                    serverBuilder.AllowAuthorizationCodeFlow()
                        .RequireProofKeyForCodeExchange();

                    serverBuilder.SetAuthorizationEndpointUris("authorize")
                        .SetTokenEndpointUris("token");

                    serverBuilder.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                    serverBuilder.DisableAccessTokenEncryption();

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

        services.AddDbContext<UserDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("UsersDatabase"));
                options.UseOpenIddict<Guid>();
            }
        );

        return services;
    }
}
