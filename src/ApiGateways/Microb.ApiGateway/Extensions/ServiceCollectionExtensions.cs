namespace Microb.ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrobHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecksUI()
            .AddInMemoryStorage();

        return services;
    }
}
