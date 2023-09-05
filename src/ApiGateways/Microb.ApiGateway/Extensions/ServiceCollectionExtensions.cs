using HealthChecks.ApplicationStatus.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microb.ApiGateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrobHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy());
        services
            .AddHealthChecksUI(settings =>
            {
                settings.SetEvaluationTimeInSeconds(10);
                settings.MaximumHistoryEntriesPerEndpoint(100);
            })
            .AddInMemoryStorage();

        return services;
    }
}
