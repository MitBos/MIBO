using Microsoft.Extensions.DependencyInjection;
using MIBO.Application.Abstractions;
using MIBO.Infrastructure.Services;

namespace MIBO.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiboInfrastructure(this IServiceCollection services, string? storageDirectory = null)
    {
        storageDirectory ??= Path.Combine(AppContext.BaseDirectory, "Data", "Integrations");

        services.AddSingleton<IIntegrationConfigStorage>(_ => new FileIntegrationConfigStorage(storageDirectory));
        services.AddHttpClient<IApiCaller, HttpApiCaller>();
        services.AddSingleton<IMappingEngine, BasicMappingEngine>();

        return services;
    }
}
