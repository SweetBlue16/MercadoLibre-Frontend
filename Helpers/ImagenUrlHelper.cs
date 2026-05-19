using Microsoft.AspNetCore.Mvc;

namespace frontendnet.Helpers;

public static class ImagenUrlHelper
{
    private const string PlaceholderProducto = "~/images/temp.png";

    public static string Producto(IUrlHelper url, string? urlWebApi, int? archivoId)
    {
        if (archivoId is null or <= 0)
            return url.Content(PlaceholderProducto);

        var backend = (urlWebApi ?? string.Empty).TrimEnd('/');
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

    public static string FallbackOnError(IUrlHelper url)
    {
        return $"this.onerror=null;this.src='{Placeholder(url)}';";
    }
}
