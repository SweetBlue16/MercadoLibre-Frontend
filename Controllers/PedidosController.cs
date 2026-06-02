using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class PedidosController(PedidosClientService pedidos, IConfiguration configuration) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string UrlWebApiKey = "UrlWebAPI";
    private const string UrlWebApiFallbackKey = "URLWebAPI";
    private const string ErrorTempDataKey = "Error";
    private const string MessageTempDataKey = "Mensaje";
    private const string InvalidOrderStatusMessage = "Estado de pedido invalido.";
    private const string OrderStatusUpdatedMessage = "Estado actualizado correctamente.";
    private const string OrderStatusUpdateErrorMessage = "No fue posible actualizar el estado.";
    private const string OrdersQueryErrorMessage = "No fue posible consultar pedidos. Verifique que el API este disponible.";

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
                return RedirectToAction(SalirAction, AuthController);

            ViewData["ErrorMessage"] = OrdersQueryErrorMessage;
        }

        return View(lista ?? []);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        Pedido? item = null;

        try
        {
            item = await pedidos.GetAsync(id);
            if (item == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> ActualizarEstado(int id, ActualizarEstadoPedido model)
    {
        if (!ModelState.IsValid)
        {
            TempData[ErrorTempDataKey] = InvalidOrderStatusMessage;
            return RedirectToAction(nameof(Detalle), new { id });
        }

        if (!EstadosPermitidos.Contains(model.Estado))
        {
            TempData[ErrorTempDataKey] = InvalidOrderStatusMessage;
            return RedirectToAction(nameof(Detalle), new { id });
        }

        try
        {
            await pedidos.ActualizarEstadoAsync(id, model.Estado);
            TempData[MessageTempDataKey] = OrderStatusUpdatedMessage;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            TempData[ErrorTempDataKey] = OrderStatusUpdateErrorMessage;
        }

        return RedirectToAction(nameof(Detalle), new { id });
    }
}