using System.Text;

namespace frontendnet.Middlewares;

public sealed class SecurityHeadersMiddleware(RequestDelegate next, IConfiguration configuration, IWebHostEnvironment environment)
{
    private readonly string contentSecurityPolicy = BuildContentSecurityPolicy(configuration, environment);

    public async Task InvokeAsync(HttpContext context)
    {
        ApplySecurityHeaders(context.Response.Headers);
        await next(context);
    }

    private void ApplySecurityHeaders(IHeaderDictionary headers)
    {
        headers["Content-Security-Policy"] = contentSecurityPolicy;
        headers["X-Frame-Options"] = "DENY";
        headers["X-Content-Type-Options"] = "nosniff";
        headers["Referrer-Policy"] = "no-referrer";
        headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=(), interest-cohort=()";
        headers["Cross-Origin-Opener-Policy"] = "same-origin";
    }

    private static string BuildContentSecurityPolicy(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var apiOrigin = GetOrigin(configuration["UrlWebAPI"] ?? configuration["URLWebAPI"]);
        var connectSources = new List<string> { "'self'" };
        var imageSources = new List<string> { "'self'", "data:", "blob:", "https:" };

        AddIfPresent(connectSources, apiOrigin);
        AddIfPresent(imageSources, apiOrigin);

        if (environment.IsDevelopment())
        {
            AddIfPresent(connectSources, "http://localhost:3000");
            AddIfPresent(connectSources, "http://localhost:5000");
            AddIfPresent(imageSources, "http://localhost:3000");
            AddIfPresent(imageSources, "http://localhost:5000");
        }

        var csp = new StringBuilder()
            .Append("default-src 'self'; ")
            .Append("script-src 'self'; ")
            .Append("style-src 'self' 'unsafe-inline'; ")
            .Append("img-src ").AppendJoin(' ', imageSources).Append("; ")
            .Append("font-src 'self' data:; ")
            .Append("connect-src ").AppendJoin(' ', connectSources).Append("; ")
            .Append("object-src 'none'; ")
            .Append("base-uri 'self'; ")
            .Append("form-action 'self'; ")
            .Append("frame-ancestors 'none';");

        if (!environment.IsDevelopment())
        {
            csp.Append(" upgrade-insecure-requests;");
        }

        return csp.ToString();
    }

    private static string? GetOrigin(string? configuredUrl)
    {
        return Uri.TryCreate(configuredUrl?.Trim(), UriKind.Absolute, out var uri)
            ? uri.GetLeftPart(UriPartial.Authority)
            : null;
    }

    private static void AddIfPresent(ICollection<string> values, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) && !values.Contains(value))
        {
            values.Add(value);
        }
    }
}
