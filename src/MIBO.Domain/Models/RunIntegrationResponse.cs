// Summary: Response DTO returned after executing an integration run with raw and mapped results.
namespace MIBO.Domain.Models;

public class RunIntegrationResponse
{
    public ApiCallResult? ApiResult { get; set; }
    public string? MappedJson { get; set; }
}

