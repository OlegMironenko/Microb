using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microb.HealthChecks.Keycloak;

public class KeycloakHealthCheck : IHealthCheck
{
    private readonly IKeycloakService _keycloakService;

    public KeycloakHealthCheck(IKeycloakService keycloakService)
    {
        _keycloakService = keycloakService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return await _keycloakService.HealthyAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy();
    }
}
