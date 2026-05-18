using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class PedidosController(PedidosClientService pedidos) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<Pedido>? lista = [];

        try
        {
            lista = await pedidos.GetAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");

            ViewData["ErrorMessage"] = "No fue posible consultar pedidos. Verifique que el API este disponible.";
        }

        return View(lista ?? []);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        Pedido? item = null;

        try
        {
            item = await pedidos.GetAsync(id);
            if (item == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");
        }

        return View(item);
    }
}
