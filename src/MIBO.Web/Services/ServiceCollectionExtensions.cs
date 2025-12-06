// Summary: Adds infrastructure, singleton notification service, and integration editor state to DI.
using MIBO.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace MIBO.Web.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiboWebApp(this IServiceCollection services, string? storageDirectory = null)
    {
        services.AddMiboInfrastructure(storageDirectory);
        services.AddSingleton(NotificationService.Instance);
        services.AddScoped<IntegrationEditorState>();

        return services;
    }
}

