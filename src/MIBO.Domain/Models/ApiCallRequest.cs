namespace MIBO.Domain.Models;

public class ApiCallRequest
{
    public string Url { get; set; } = string.Empty;
    public string Method { get; set; } = "GET";
    public string? Body { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
}
