using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class PedidosController(PedidosClientService pedidos, IConfiguration configuration) : Controller
{
    private static readonly string[] EstadosPermitidos =
    [
        "Recibido",
        "Procesado",
        "Enviado",
        "En ruta de entrega",
        "Entregado",
    ];

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
        ViewBag.Url = configuration["UrlWebAPI"] ?? configuration["URLWebAPI"];
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

    [HttpPost]
    public async Task<IActionResult> ActualizarEstado(int id, ActualizarEstadoPedido model)
    {
        if (!EstadosPermitidos.Contains(model.Estado))
        {
            TempData["Error"] = "Estado de pedido invalido.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        try
        {
            await pedidos.ActualizarEstadoAsync(id, model.Estado);
            TempData["Mensaje"] = "Estado actualizado correctamente.";
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");

            TempData["Error"] = "No fue posible actualizar el estado.";
        }

        return RedirectToAction(nameof(Detalle), new { id });
    }
}
