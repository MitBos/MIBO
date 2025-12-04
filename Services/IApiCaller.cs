using MIBO.Models;

namespace MIBO.Services;

public interface IApiCaller
{
    Task<ApiCallResult> ExecuteAsync(ApiCallRequest request, CancellationToken cancellationToken = default);
}
