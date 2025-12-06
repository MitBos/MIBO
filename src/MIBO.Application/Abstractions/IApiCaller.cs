// Summary: Abstraction for executing outbound API calls used by integration runs.
using MIBO.Domain.Models;

namespace MIBO.Application.Abstractions;

public interface IApiCaller
{
    Task<ApiCallResult> ExecuteAsync(ApiCallRequest request, CancellationToken cancellationToken = default);
}

