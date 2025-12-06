// Summary: Adds infrastructure (configs, registry, relations), notification service, and editor state to DI.
using MIBO.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MIBO.Web.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiboWebApp(
        this IServiceCollection services,
        string? integrationStorageDirectory = null,
        string? apiRegistryDirectory = null,
        string? endpointRelationDirectory = null)
    {
        services.AddMiboInfrastructure(integrationStorageDirectory, apiRegistryDirectory, endpointRelationDirectory);
        services.AddSingleton(NotificationService.Instance);
        services.AddScoped<IntegrationEditorState>();

        return services;
    }
}
