namespace MIBO.Models;

public class ApiCallResult
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? ResponseBody { get; set; }
    public List<string> Logs { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
