namespace MIBO.Domain.Models;

public class RunIntegrationResponse
{
    public ApiCallResult? ApiResult { get; set; }
    public string? MappedJson { get; set; }
}
