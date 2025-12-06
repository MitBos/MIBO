// Summary: Defines persistence operations for endpoint relations between systems.
using MIBO.Domain.Models;

namespace MIBO.Application.Abstractions;

public interface IEndpointRelationStorage
{
    Task<List<EndpointRelation>> ListAsync(CancellationToken cancellationToken = default);
    Task<EndpointRelation?> LoadAsync(string sourceSystemKey, string sourceEndpointKey, CancellationToken cancellationToken = default);
    Task SaveAsync(EndpointRelation relation, CancellationToken cancellationToken = default);
    Task DeleteAsync(string sourceSystemKey, string sourceEndpointKey, CancellationToken cancellationToken = default);
}
