using System.Net;

namespace frontendnet.Services.Errors;

public sealed class ApiClientException(
    string message,
    HttpStatusCode? statusCode = null,
    string? code = null,
    string? correlationId = null)
    : HttpRequestException(message, null, statusCode)
{
    public string? Code { get; } = code;
    public string? CorrelationId { get; } = correlationId;
}
