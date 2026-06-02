using frontendnet.Models;
using frontendnet.Services;
using frontendnet.Services.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class UsuariosController(UsuariosClientService usuarios, RolesClientService roles) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string EmailField = "Email";
    private const string NombreField = "Nombre";
    private const string ErrorTempDataKey = "Error";
    private const string MessageTempDataKey = "Mensaje";
    private const string GenericActionErrorMessage = "No ha sido posible realizar la acción. Inténtelo nuevamente.";
    private const string UserDeletedMessage = "Usuario eliminado correctamente.";

    public async Task<IActionResult> Index()
    {
        List<Usuario>? lista = [];

        try
        {
            lista = await usuarios.GetAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(lista);
    }

    public async Task<IActionResult> Detalle(string id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Usuario? item = null;

        try
        {
            item = await usuarios.GetAsync(id);
            if (item == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(item);
    }

    public async Task<IActionResult> Crear()
    {
        await RolesDropDownListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync(UsuarioPwd itemToCreate)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(EmailField, GenericActionErrorMessage);
            await RolesDropDownListAsync();
            return View(itemToCreate);
        }

        try
        {
            await usuarios.PostAsync(itemToCreate);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ModelState.AddModelError(EmailField, GenericActionErrorMessage);
        await RolesDropDownListAsync();
        return View(itemToCreate);
    }

    [HttpGet("[controller]/[action]/{email}")]
    public async Task<IActionResult> EditarAsync(string email)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Usuario? itemToEdit = null;

        try
        {
            itemToEdit = await usuarios.GetAsync(email);
            if (itemToEdit == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.PuedeEditar = !(User.Identity?.Name == email);
        await RolesDropDownListAsync(itemToEdit?.Rol);
        return View(itemToEdit);
    }

    [HttpPost("[controller]/[action]/{email}")]
    public async Task<IActionResult> EditarAsync(string email, Usuario itemToEdit)
    {
        if (email != itemToEdit.Email) return NotFound();

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(NombreField, GenericActionErrorMessage);
            ViewBag.PuedeEditar = !(User.Identity?.Name == email);
            await RolesDropDownListAsync(itemToEdit?.Rol);
            return View(itemToEdit);
        }

        try
        {
            await usuarios.PutAsync(itemToEdit);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ModelState.AddModelError(NombreField, GenericActionErrorMessage);
        ViewBag.PuedeEditar = !(User.Identity?.Name == email);
        await RolesDropDownListAsync(itemToEdit?.Rol);
        return View(itemToEdit);
    }

    [HttpGet]
public async Task<IActionResult> Eliminar(string id, bool showError)
{
    if (!ModelState.IsValid)
    {
        return BadRequest();
    }

    Usuario? itemToDelete = null;

    try
    {
        itemToDelete = await usuarios.GetAsync(id);
        if (itemToDelete == null) return NotFound();

        if (showError)
            ViewData["ErrorMessage"] = GenericActionErrorMessage;
    }
    catch (HttpRequestException ex)
    {
        if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return RedirectToAction(SalirAction, AuthController);
    }

    ViewBag.PuedeEditar = !(User.Identity?.Name == id);
    return View(itemToDelete);
}

    [HttpPost]
    public async Task<IActionResult> Eliminar(string id)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Eliminar), new { id, showError = true });
        }

        try
        {
            await usuarios.DeleteAsync(id);
            TempData[MessageTempDataKey] = UserDeletedMessage;
            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Conflict
            && ex.Code == ErrorCodeCatalog.UserHasAssociatedOrders)
        {
            TempData[ErrorTempDataKey] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            return RedirectToAction(SalirAction, AuthController);
        }
        catch (ApiClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
        {
            TempData[ErrorTempDataKey] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
        catch (ApiClientException)
        {
            TempData[ErrorTempDataKey] = GenericActionErrorMessage;
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Eliminar), new { id, showError = true });
    }

    private async Task RolesDropDownListAsync(object? rolSeleccionado = null)
    {
        var listado = await roles.GetAsync();
        ViewBag.Rol = new SelectList(listado, "Nombre", "Nombre", rolSeleccionado);
    }
}