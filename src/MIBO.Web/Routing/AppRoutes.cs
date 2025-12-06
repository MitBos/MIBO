// Summary: Centralized route helpers for navigation and API endpoints.
namespace MIBO.Web.Routing;

public static class AppRoutes
{
    public const string IntegrationsList = "/app/integrations";
    public const string IntegrationsListRoot = "/";
    public static string EditIntegration(string name) => $"/app/integrations/edit/{name}";
    public const string ApiSystems = "/app/api-systems";
    public static string ApiEndpointDetails(string systemKey, string endpointKey) => $"/app/api-systems/{systemKey}/{endpointKey}";
    public const string EndpointRelations = "/app/endpoint-relations";
}

public static class ApiRoutes
{
    public const string IntegrationsBase = "api/integrations";
    public static string Integration(string name) => $"{IntegrationsBase}/{name}";
    public const string SaveIntegration = $"{IntegrationsBase}/save";
    public const string RunIntegration = $"{IntegrationsBase}/run";
    public const string DuplicateIntegration = $"{IntegrationsBase}/duplicate";
    public const string ApiSystemsBase = "api/apisystems";
    public static string ApiSystem(string key) => $"{ApiSystemsBase}/{key}";
    public static string ApiEndpoints(string systemKey) => $"{ApiSystemsBase}/{systemKey}/endpoints";
    public static string ApiEndpoint(string systemKey, string endpointKey) => $"{ApiEndpoints(systemKey)}/{endpointKey}";
    public const string EndpointRelationsBase = "api/endpoint-relations";
    public static string EndpointRelation(string sourceSystemKey, string sourceEndpointKey) =>
        $"{EndpointRelationsBase}/{sourceSystemKey}/{sourceEndpointKey}";
}

