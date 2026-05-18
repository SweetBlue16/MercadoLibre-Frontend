using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Usuario")]
public class CarritoController(CarritoClientService carrito, PedidosClientService pedidos, IConfiguration configuration) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.Url = configuration["UrlWebAPI"] ?? configuration["URLWebAPI"];
        CarritoResumen? resumen = new();

        try
        {
            resumen = await carrito.GetAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");

            ViewData["ErrorMessage"] = "No fue posible consultar el carrito. Verifique que el API este disponible.";
        }

        return View(resumen ?? new CarritoResumen());
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(int id)
    {
        try
        {
            await carrito.AgregarAsync(id);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Actualizar(int id, int cantidad)
    {
        if (cantidad < 1 || cantidad > 99)
            return RedirectToAction(nameof(Index));

        try
        {
            await carrito.ActualizarAsync(id, cantidad);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            await carrito.EliminarAsync(id);
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Vaciar()
    {
        try
        {
            await carrito.VaciarAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Confirmar()
    {
        try
        {
            await pedidos.ConfirmarAsync();
            TempData["Mensaje"] = "Compra confirmada correctamente.";
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction("Salir", "Auth");

            TempData["Error"] = "No fue posible confirmar la compra.";
        }

        return RedirectToAction(nameof(Index));
    }
}
