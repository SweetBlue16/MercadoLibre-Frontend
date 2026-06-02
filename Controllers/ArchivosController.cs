using frontendnet.Models;
using frontendnet.Services;
using frontendnet.Services.Errors;
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
    private const long MaxImageSizeBytes = 5 * 1024 * 1024;

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
        if (!ModelState.IsValid) return BadRequest();

        try
        {
            var item = await archivos.GetAsync(id);
            if (item == null) return NotFound();

            ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
            return View(item);
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? RedirectToAction(SalirAction, AuthController)
                : NotFound();
        }
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync(Upload itemToCreate)
    {
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid || !ValidateImage(itemToCreate.Portada))
        {
            return View(itemToCreate);
        }

        try
        {
            await archivos.PostAsync(itemToCreate);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            ModelState.AddModelError(PortadaField, ex.Message);
            return View(itemToCreate);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(PortadaField, GenericActionErrorMessage);
            return View(itemToCreate);
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditarAsync(int id)
    {
        if (!ModelState.IsValid) return BadRequest();

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        try
        {
            var itemToEdit = await archivos.GetAsync(id);
            if (itemToEdit == null) return NotFound();

            ViewBag.ArchivoId = itemToEdit.ArchivoId;
            ViewBag.Nombre = itemToEdit.Nombre;
            return View();
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? RedirectToAction(SalirAction, AuthController)
                : NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditarAsync(int id, Upload itemToEdit)
    {
        if (itemToEdit == null || id != itemToEdit.ArchivoId) return NotFound();

        ViewBag.ArchivoId = itemToEdit.ArchivoId;
        ViewBag.Nombre = itemToEdit.Nombre;
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid || !ValidateImage(itemToEdit.Portada))
        {
            return View(itemToEdit);
        }

        try
        {
            await archivos.PutAsync(itemToEdit);
            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);

            ModelState.AddModelError(PortadaField, ex.Message);
            return View(itemToEdit);
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(PortadaField, GenericActionErrorMessage);
            return View(itemToEdit);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Eliminar(int id, bool? showError = false)
    {
        if (!ModelState.IsValid) return BadRequest();

        try
        {
            var itemToDelete = await archivos.GetAsync(id);
            if (itemToDelete == null) return NotFound();

            if (showError.GetValueOrDefault())
                ViewData["ErrorMessage"] = GenericActionErrorMessage;

            ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
            return View(itemToDelete);
        }
        catch (HttpRequestException ex)
        {
            return ex.StatusCode == System.Net.HttpStatusCode.Unauthorized
                ? RedirectToAction(SalirAction, AuthController)
                : NotFound();
        }
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

    private bool ValidateImage(IFormFile file)
    {
        if (file.Length > MaxImageSizeBytes)
        {
            ModelState.AddModelError(PortadaField, "El archivo supera el tamaño máximo permitido.");
            return false;
        }

        if (!IsAllowedImage(file.ContentType))
        {
            ModelState.AddModelError(PortadaField, "El archivo no es una imagen permitida.");
            return false;
        }

        return true;
    }
}
