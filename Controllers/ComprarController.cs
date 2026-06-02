using frontendnet.Models;
using frontendnet.Services;
using frontendnet.Services.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class ComprarController(ProductosClientService productos, IConfiguration configuration) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string UrlWebApiKey = "UrlWebAPI";
    private const string UrlWebApiFallbackKey = "URLWebAPI";
    private const string ServerUnavailableMessage = "El servidor no está disponible en este momento. Inténtalo más tarde.";

    public async Task<IActionResult> Index(string? s)
    {
        List<Producto>? lista = [];

        try
        {
            lista = await productos.GetAsync(NormalizeSearch(s));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        ViewBag.search = NormalizeSearch(s);
        return View(lista);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        try
        {
            var producto = await productos.GetAsync(id);
            ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
            return View(producto);
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return RedirectToAction(SalirAction, AuthController);
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Response.StatusCode = 404;
            ViewData["ErrorMessage"] = ex.Message;
            return View((Producto?)null);
        }
        catch (HttpRequestException)
        {
            ViewData["ErrorMessage"] = ServerUnavailableMessage;
            return View((Producto?)null);
        }
    }

    private static string? NormalizeSearch(string? search)
    {
        var value = search?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value[..Math.Min(value.Length, 40)];
    }
}
