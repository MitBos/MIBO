using System.Net.Http.Json;
using MIBO.Application.Abstractions;
using MIBO.Domain.Models;

namespace MIBO.Infrastructure.Services;

public class HttpApiCaller : IApiCaller
{
    private readonly HttpClient _httpClient;

    public HttpApiCaller(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiCallResult> ExecuteAsync(ApiCallRequest request, CancellationToken cancellationToken = default)
    {
        var result = new ApiCallResult();

        try
        {
            using var httpRequest = new HttpRequestMessage(new HttpMethod(request.Method), request.Url);

            foreach (var header in request.Headers)
            {
                httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Body))
            {
                httpRequest.Content = new StringContent(request.Body, System.Text.Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(httpRequest, cancellationToken);
            result.StatusCode = (int)response.StatusCode;
            result.Success = response.IsSuccessStatusCode;
            result.ResponseBody = await response.Content.ReadAsStringAsync(cancellationToken);
            result.Logs.Add($"Executed {request.Method} {request.Url}");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.Logs.Add($"Error executing {request.Method} {request.Url}: {ex.Message}");
        }

        return result;
    }
}
