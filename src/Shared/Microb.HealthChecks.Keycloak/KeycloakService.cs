using System.Net.Http.Json;

namespace Microb.HealthChecks.Keycloak;

public class KeycloakService : IKeycloakService
{
    private readonly HttpClient _httpClient;

    public KeycloakService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> HealthyAsync(CancellationToken cancellationToken)
    {
        const string healthCheckUrl = @"http://microb-keycloak-server:8080/health";

        try
        {
            var keycloakResponse = await _httpClient.GetAsync(healthCheckUrl, cancellationToken);
            keycloakResponse.EnsureSuccessStatusCode();

            var keycloakHealthStatus = await keycloakResponse.Content.ReadFromJsonAsync<KeycloackHealthCheckResponse>();
            return string.Equals(keycloakHealthStatus?.Status, "UP", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Keycloack error with exception: {e.ToString()}");
            return false;
        }
    }
}
