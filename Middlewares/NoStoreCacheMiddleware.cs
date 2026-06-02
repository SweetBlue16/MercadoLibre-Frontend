namespace frontendnet.Middlewares;

public sealed class NoStoreCacheMiddleware(RequestDelegate next)
{
    private static readonly PathString[] SensitivePaths =
    [
        "/Auth/ConfirmarCorreo",
        "/Auth/OlvidePassword",
        "/Auth/RestablecerPassword",
        "/Carrito",
        "/MisPedidos",
        "/Perfil",
        "/Usuarios",
        "/Pedidos",
        "/Productos",
        "/Archivos",
        "/Bitacora",
    ];

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldDisableCache(context))
        {
            context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
            context.Response.Headers["Pragma"] = "no-cache";
            context.Response.Headers["Expires"] = "0";
        }

        await next(context);
    }

    private static bool ShouldDisableCache(HttpContext context)
    {
        return context.User.Identity?.IsAuthenticated == true
            || SensitivePaths.Any(path => context.Request.Path.StartsWithSegments(path));
    }
}
