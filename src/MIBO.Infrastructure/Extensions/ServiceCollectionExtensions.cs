// Summary: Registers infrastructure services (storage, HTTP caller, mapping engine) into DI.
using Microsoft.Extensions.DependencyInjection;
using MIBO.Application.Abstractions;
using MIBO.Infrastructure.Services;

namespace MIBO.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiboInfrastructure(
        this IServiceCollection services,
        string? integrationStorageDirectory = null,
        string? apiRegistryDirectory = null,
        string? endpointRelationDirectory = null)
    {
        integrationStorageDirectory ??= Path.Combine(AppContext.BaseDirectory, "Data", "Integrations");
        apiRegistryDirectory ??= Path.Combine(AppContext.BaseDirectory, "Data", "ApiSystems");
        endpointRelationDirectory ??= Path.Combine(AppContext.BaseDirectory, "Data", "EndpointRelations");

        services.AddSingleton<IIntegrationConfigStorage>(_ => new FileIntegrationConfigStorage(integrationStorageDirectory));
        services.AddSingleton<IApiRegistryStorage>(_ => new FileApiRegistryStorage(apiRegistryDirectory));
        services.AddSingleton<IEndpointRelationStorage>(_ => new FileEndpointRelationStorage(endpointRelationDirectory));
        services.AddHttpClient<IApiCaller, HttpApiCaller>();
        services.AddSingleton<IMappingEngine, BasicMappingEngine>();

        return services;
    }
}

