using frontendnet.Models;
using frontendnet.Services;
using frontendnet.Services.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class ComprarController(ProductosClientService productos, IConfiguration configuration) : Controller
{
    public async Task<IActionResult> Index(string? s)
    {
        List<Producto>? lista = [];

        try
        {
            lista = await productos.GetAsync(s);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");
        }

        ViewBag.Url = (configuration["UrlWebAPI"] ?? configuration["URLWebAPI"]);
        ViewBag.search = s;
        return View(lista);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        try
        {
            var producto = await productos.GetAsync(id);
            ViewBag.Url = configuration["UrlWebAPI"] ?? configuration["URLWebAPI"];
            return View(producto);
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return RedirectToAction("Salir", "Auth");
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            Response.StatusCode = 404;
            ViewData["ErrorMessage"] = ex.Message;
            return View((Producto?)null);
        }
        catch (HttpRequestException)
        {
            ViewData["ErrorMessage"] = "El servidor no esta disponible en este momento. Intentalo mas tarde.";
            return View((Producto?)null);
        }
    }
}
