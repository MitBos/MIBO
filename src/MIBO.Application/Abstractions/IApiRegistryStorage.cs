// Summary: Defines persistence operations for API systems and their endpoints.
using MIBO.Domain.Models;

namespace MIBO.Application.Abstractions;

public interface IApiRegistryStorage
{
    Task<List<ApiSystem>> ListSystemsAsync(CancellationToken cancellationToken = default);
    Task<ApiSystem?> LoadSystemAsync(string systemKey, CancellationToken cancellationToken = default);
    Task SaveSystemAsync(ApiSystem system, CancellationToken cancellationToken = default);
    Task DeleteSystemAsync(string systemKey, CancellationToken cancellationToken = default);

    Task<List<ApiEndpoint>> ListEndpointsAsync(string systemKey, CancellationToken cancellationToken = default);
    Task<ApiEndpoint?> LoadEndpointAsync(string systemKey, string endpointKey, CancellationToken cancellationToken = default);
    Task SaveEndpointAsync(ApiEndpoint endpoint, CancellationToken cancellationToken = default);
    Task DeleteEndpointAsync(string systemKey, string endpointKey, CancellationToken cancellationToken = default);
}
