using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microb.HealthChecks.Keycloak.DependencyInjection;

public static class KeycloakHealthCheckBuilderExtensions
{
    private const string HEALTH_CHECK_NAME = "keycloak";

    public static IHealthChecksBuilder AddKeycloak(
        this IHealthChecksBuilder builder,
        string healthCheckName = HEALTH_CHECK_NAME,
        HealthStatus? failureStatus = default,
        IEnumerable<string>? tags = default,
        TimeSpan? timeout = default)
    {
        builder.Services.AddHttpClient();
        builder.Services.TryAddScoped<IKeycloakService, KeycloakService>();

        return builder.Add(new HealthCheckRegistration(
            string.IsNullOrEmpty(healthCheckName) ? HEALTH_CHECK_NAME : healthCheckName,
            sp => new KeycloakHealthCheck(sp.GetRequiredService<IKeycloakService>()),
            failureStatus,
            tags,
            timeout));
    }
}
