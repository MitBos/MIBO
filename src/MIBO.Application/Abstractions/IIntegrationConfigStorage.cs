using MIBO.Domain.Models;

namespace MIBO.Application.Abstractions;

public interface IIntegrationConfigStorage
{
    Task<List<string>> ListAsync(CancellationToken cancellationToken = default);
    Task<IntegrationConfig?> LoadAsync(string name, CancellationToken cancellationToken = default);
    Task SaveAsync(IntegrationConfig config, CancellationToken cancellationToken = default);
    Task DeleteAsync(string name, CancellationToken cancellationToken = default);
}
