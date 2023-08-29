using Microsoft.EntityFrameworkCore;

namespace Rozum.BrainTrainer.Identity.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRozumIdentity(this IServiceCollection services)
    {
        services.AddOpenIddict()
            .AddCore(coreBuilder =>
                {
                    coreBuilder.UseEntityFrameworkCore()
                        .UseDbContext<DbContext>();
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
            .AddValidation(configuration =>
            {
                configuration.UseLocalServer();
                configuration.UseAspNetCore();
            });

        services.AddDbContext<DbContext>(options =>
            {
                options.UseInMemoryDatabase(nameof(DbContext));
                options.UseOpenIddict();
            }
        );

        return services;
    }
}
