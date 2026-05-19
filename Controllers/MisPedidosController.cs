using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class MisPedidosController(PedidosClientService pedidos, IConfiguration configuration) : Controller
{
    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await pedidos.GetMisPedidosAsync() ?? []);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");

            ViewData["ErrorMessage"] = "No fue posible consultar tus pedidos.";
            return View(new List<Pedido>());
        }
    }

    public async Task<IActionResult> Detalle(int id)
    {
        ViewBag.Url = configuration["UrlWebAPI"] ?? configuration["URLWebAPI"];
        try
        {
            var item = await pedidos.GetMiPedidoAsync(id);
            if (item == null) return NotFound();
            return View(item);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");

            return NotFound();
        }
    }
}
