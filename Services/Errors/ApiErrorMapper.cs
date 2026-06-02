using System.Net;
using System.Net.Http.Json;

namespace frontendnet.Services.Errors;

public static class ApiErrorMapper
{
    public static async Task<T?> GetFromJsonAsync<T>(HttpClient client, string requestUri)
    {
        var response = await SendAsync(() => client.GetAsync(requestUri));
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public static async Task<string> GetStringAsync(HttpClient client, string requestUri)
    {
        var response = await SendAsync(() => client.GetAsync(requestUri));
        return await response.Content.ReadAsStringAsync();
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(HttpClient client, string requestUri, T payload)
    {
        return SendAsync(() => client.PostAsJsonAsync(requestUri, payload));
    }

    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(HttpClient client, string requestUri, T payload)
    {
        return SendAsync(() => client.PutAsJsonAsync(requestUri, payload));
    }

    public static Task<HttpResponseMessage> SendPutMultipartAsync(HttpClient client, string requestUri, HttpContent content)
    {
        return SendAsync(() => client.PutAsync(requestUri, content));
    }

    public static Task<HttpResponseMessage> PostAsync(HttpClient client, string requestUri, HttpContent? content)
    {
        return SendAsync(() => client.PostAsync(requestUri, content));
    }

    public static Task<HttpResponseMessage> DeleteAsync(HttpClient client, string requestUri)
    {
        return SendAsync(() => client.DeleteAsync(requestUri));
    }

    private static async Task<HttpResponseMessage> SendAsync(Func<Task<HttpResponseMessage>> action)
    {
        try
        {
            var response = await action();
            await EnsureSuccessAsync(response);
            return response;
        }
        catch (TaskCanceledException ex)
        {
            throw Unavailable(ex);
        }
        catch (HttpRequestException ex) when (ex is not ApiClientException)
        {
            throw Unavailable(ex);
        }
    }

    public static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        ApiErrorResponse? payload = null;
        try
        {
            payload = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
        }
        catch
        {
            // Non-JSON errors are intentionally collapsed into safe messages.
        }

        var code = payload?.Code ?? CodeFromStatus(response.StatusCode);
        var message = MessageCatalog.GetMessage(code);
        throw new ApiClientException(message, response.StatusCode, code, payload?.CorrelationId);
    }

    private static ApiClientException Unavailable(Exception ex)
    {
        return new ApiClientException(MessageCatalog.ServerUnavailable, null, ErrorCodeCatalog.ServerUnavailable);
    }

    private static string CodeFromStatus(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.Unauthorized => ErrorCodeCatalog.AuthSessionExpired,
            HttpStatusCode.Forbidden => ErrorCodeCatalog.Forbidden,
            HttpStatusCode.BadRequest => ErrorCodeCatalog.ValidationError,
            HttpStatusCode.NotFound => ErrorCodeCatalog.ResourceNotFound,
            HttpStatusCode.ServiceUnavailable => ErrorCodeCatalog.ServerUnavailable,
            _ => ErrorCodeCatalog.InternalError,
        };
    }

    private sealed class ApiErrorResponse
    {
        public string? Code { get; set; }
        public string? CorrelationId { get; set; }
    }
}
