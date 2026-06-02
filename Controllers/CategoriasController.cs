using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class CategoriasController(CategoriasClientService categorias) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string NombreField = "Nombre";
    private const string GenericActionErrorMessage = "No ha sido posible realizar la acción. Inténtelo nuevamente.";

    public async Task<IActionResult> Index()
    {
        List<Categoria>? lista = [];

        try
        {
            lista = await categorias.GetAsync();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(lista);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Categoria? item = null;

        try
        {
            item = await categorias.GetAsync(id);
            if (item == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(item);
    }

    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync(Categoria itemToCreate)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(NombreField, GenericActionErrorMessage);
            return View(itemToCreate);
        }

        try
        {
            await categorias.PostAsync(itemToCreate);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ModelState.AddModelError(NombreField, GenericActionErrorMessage);
        return View(itemToCreate);
    }

    public async Task<IActionResult> EditarAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Categoria? itemToEdit = null;

        try
        {
            itemToEdit = await categorias.GetAsync(id);
            if (itemToEdit == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(itemToEdit);
    }

    [HttpPost]
    public async Task<IActionResult> EditarAsync(int id, Categoria itemToEdit)
    {
        if (id != itemToEdit.CategoriaId) return NotFound();

        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(NombreField, GenericActionErrorMessage);
            return View(itemToEdit);
        }

        try
        {
            await categorias.PutAsync(itemToEdit);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ModelState.AddModelError(NombreField, GenericActionErrorMessage);
        return View(itemToEdit);
    }

    public async Task<IActionResult> Eliminar(int id, bool? showError = false)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Categoria? itemToDelete = null;

        try
        {
            itemToDelete = await categorias.GetAsync(id);
            if (itemToDelete == null) return NotFound();

            if (showError.GetValueOrDefault())
                ViewData["ErrorMessage"] = GenericActionErrorMessage;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return View(itemToDelete);
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(int id)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Eliminar), new { id, showError = true });
        }

        try
        {
            await categorias.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Eliminar), new { id, showError = true });
    }
}