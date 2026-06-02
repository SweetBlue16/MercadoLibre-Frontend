using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class MisPedidosController(PedidosClientService pedidos, IConfiguration configuration) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string UrlWebApiKey = "UrlWebAPI";
    private const string UrlWebApiFallbackKey = "URLWebAPI";
    private const string OrdersErrorMessage = "No fue posible consultar tus pedidos.";

    public async Task<IActionResult> Index()
    {
        try
        {
            return View(await pedidos.GetMisPedidosAsync() ?? []);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            ViewData["ErrorMessage"] = OrdersErrorMessage;
            return View(new List<Pedido>());
        }
    }

    public async Task<IActionResult> Detalle(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        try
        {
            var item = await pedidos.GetMiPedidoAsync(id);
            if (item == null) return NotFound();

            return View(item);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            return NotFound();
        }
    }
}