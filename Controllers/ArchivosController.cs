using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class ArchivosController(ArchivosClientService archivos, IConfiguration configuration) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string UrlWebApiKey = "UrlWebAPI";
    private const string UrlWebApiFallbackKey = "URLWebAPI";
    private const string PortadaField = "Portada";
    private const string GenericActionErrorMessage = "No ha sido posible realizar la acción. Inténtelo nuevamente.";

    public async Task<IActionResult> Index()
    {
        List<Archivo>? lista = [];

        try
        {
            lista = await archivos.GetAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        return View(lista);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Archivo? item = null;

        try
        {
            item = await archivos.GetAsync(id);
            if (item == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        return View(item);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync(Upload itemToCreate)
    {
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(PortadaField, GenericActionErrorMessage);
            return View(itemToCreate);
        }

        try
        {
            if (itemToCreate.Portada.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(PortadaField, $"El archivo de {itemToCreate.Portada.Length / 1024} KB supera el tamprecio máximo permitido.");
                return View(itemToCreate);
            }

            if (!IsAllowedImage(itemToCreate.Portada.ContentType))
            {
                ModelState.AddModelError(PortadaField, $"El archivo {itemToCreate.Portada.FileName} no tiene una extensión permitida.");
                return View(itemToCreate);
            }

            await archivos.PostAsync(itemToCreate);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ModelState.AddModelError(PortadaField, GenericActionErrorMessage);
        return View(itemToCreate);
    }

    public async Task<IActionResult> EditarAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        try
        {
            Archivo? itemToEdit = await archivos.GetAsync(id);
            ViewBag.ArchivoId = itemToEdit?.ArchivoId;
            ViewBag.Nombre = itemToEdit?.Nombre;

            if (itemToEdit == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> EditarAsync(int id, Upload itemToEdit)
    {
        if (itemToEdit == null) return NotFound();
        if (id != itemToEdit.ArchivoId) return NotFound();

        ViewBag.ArchivoId = itemToEdit.ArchivoId;
        ViewBag.Nombre = itemToEdit.Nombre;
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(PortadaField, GenericActionErrorMessage);
            return View(itemToEdit);
        }

        try
        {
            if (itemToEdit.Portada.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(PortadaField, $"El archivo de {itemToEdit.Portada.Length / 1024} KB supera el tamprecio máximo permitido.");
                return View(itemToEdit);
            }

            if (!IsAllowedImage(itemToEdit.Portada.ContentType))
            {
                ModelState.AddModelError(PortadaField, $"El archivo {itemToEdit.Portada.FileName} no tiene una extensión permitida.");
                return View(itemToEdit);
            }

            await archivos.PutAsync(itemToEdit);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ModelState.AddModelError(PortadaField, GenericActionErrorMessage);
        return View(itemToEdit);
    }

    public async Task<IActionResult> Eliminar(int id, bool? showError = false)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Archivo? itemToDelete = null;

        try
        {
            itemToDelete = await archivos.GetAsync(id);
            if (itemToDelete == null) return NotFound();

            if (showError.GetValueOrDefault())
                ViewData["ErrorMessage"] = GenericActionErrorMessage;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        return View(itemToDelete);
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Eliminar), new { id, showError = true });
        }

        try
        {
            await archivos.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Eliminar), new { id, showError = true });
    }

    private static bool IsAllowedImage(string contentType)
    {
        return contentType is "image/jpeg" or "image/png" or "image/webp";
    }
}