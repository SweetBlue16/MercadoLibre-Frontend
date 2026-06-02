using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class CarritoController(CarritoClientService carrito, PedidosClientService pedidos, IConfiguration configuration) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string UrlWebApiKey = "UrlWebAPI";
    private const string UrlWebApiFallbackKey = "URLWebAPI";
    private const string CartErrorMessage = "No fue posible consultar el carrito. Verifique que el API este disponible.";
    private const string PurchaseSuccessMessage = "Compra confirmada correctamente.";
    private const string PurchaseErrorMessage = "No fue posible confirmar la compra.";

    public async Task<IActionResult> Index()
    {
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        CarritoResumen? resumen = new();

        try
        {
            resumen = await carrito.GetAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            ViewData["ErrorMessage"] = CartErrorMessage;
        }

        return View(resumen ?? new CarritoResumen());
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(int id)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await carrito.AgregarAsync(id);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Actualizar(int id, int cantidad)
    {
        if (!ModelState.IsValid || cantidad < 1 || cantidad > 99)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await carrito.ActualizarAsync(id, cantidad);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await carrito.EliminarAsync(id);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Vaciar()
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await carrito.VaciarAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar()
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await pedidos.ConfirmarAsync();
            TempData["Mensaje"] = PurchaseSuccessMessage;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            TempData["Error"] = PurchaseErrorMessage;
        }

        return RedirectToAction(nameof(Index));
    }
}