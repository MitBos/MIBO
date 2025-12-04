namespace MIBO.Web.Routing;

public static class AppRoutes
{
    public const string IntegrationsList = "/app/integrations";
    public const string IntegrationsListRoot = "/";
    public static string EditIntegration(string name) => $"/app/integrations/edit/{name}";
}

public static class ApiRoutes
{
    public const string IntegrationsBase = "api/integrations";
    public static string Integration(string name) => $"{IntegrationsBase}/{name}";
    public const string SaveIntegration = $"{IntegrationsBase}/save";
    public const string RunIntegration = $"{IntegrationsBase}/run";
    public const string DuplicateIntegration = $"{IntegrationsBase}/duplicate";
}
