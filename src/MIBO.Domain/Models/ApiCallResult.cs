// Summary: Captures the outcome of an API call including status, body, and diagnostic logs/errors.
namespace MIBO.Domain.Models;

public class ApiCallResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public List<string> Logs { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

