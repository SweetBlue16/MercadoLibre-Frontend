using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Helpers;

public static class ImagenUrlHelper
{
    private const string PlaceholderProducto = "~/images/temp.png";

    public static string Producto(IUrlHelper url, string? urlWebApi, int? archivoId, string? imagenUrl = null)
    {
        var resolvedUrl = ResolveApiImageUrl(urlWebApi, imagenUrl);
        if (!string.IsNullOrWhiteSpace(resolvedUrl))
            return resolvedUrl;

        if (archivoId is null or <= 0)
            return url.Content(PlaceholderProducto);

        var backend = NormalizeApiBase(urlWebApi);
        return string.IsNullOrWhiteSpace(backend)
            ? url.Content(PlaceholderProducto)
            : $"{backend}/api/archivos/{archivoId}";
    }

    public static string Archivo(IUrlHelper url, string? urlWebApi, int? archivoId)
    {
        return Producto(url, urlWebApi, archivoId);
    }

    public static string Placeholder(IUrlHelper url)
    {
        return url.Content(PlaceholderProducto);
    }

    private static string? ResolveApiImageUrl(string? urlWebApi, string? imagenUrl)
    {
        if (string.IsNullOrWhiteSpace(imagenUrl))
            return null;

        var safeImagenUrl = imagenUrl.Trim();
        if (Uri.TryCreate(safeImagenUrl, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.Scheme is "http" or "https" ? safeImagenUrl : null;
        }

        if (!safeImagenUrl.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            return null;

        var backend = NormalizeApiBase(urlWebApi);
        return string.IsNullOrWhiteSpace(backend) ? null : $"{backend}{safeImagenUrl}";
    }

    private static string NormalizeApiBase(string? urlWebApi)
    {
        var backend = (urlWebApi ?? string.Empty).TrimEnd('/');
        return backend.EndsWith("/api", StringComparison.OrdinalIgnoreCase)
            ? backend[..^4]
            : backend;
    }
}
