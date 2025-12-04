using Microsoft.Extensions.DependencyInjection;

namespace MIBO.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiboCore(this IServiceCollection services, string? storageDirectory = null)
    {
        storageDirectory ??= Path.Combine(AppContext.BaseDirectory, "Data", "Integrations");

        services.AddSingleton<IIntegrationConfigStorage>(_ => new FileIntegrationConfigStorage(storageDirectory));
        services.AddHttpClient<IApiCaller, HttpApiCaller>();
        services.AddSingleton<IMappingEngine, BasicMappingEngine>();
        services.AddSingleton(NotificationService.Instance);
        services.AddScoped<IntegrationEditorState>();

        return services;
    }
}
