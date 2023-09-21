namespace Microb.HealthChecks.Keycloak;

public interface IKeycloakService
{
    Task<bool> HealthyAsync(CancellationToken cancellationToken);
}