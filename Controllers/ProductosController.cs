using System.Security.Claims;
using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace frontendnet;

[Authorize(Roles = "Administrador")]
public class ProductosController(
    ProductosClientService productos,
    CategoriasClientService categorias,
    ArchivosClientService archivos,
    IConfiguration configuration
) : Controller
{
    private const string AuthController = "Auth";
    private const string SalirAction = "Salir";
    private const string UrlWebApiKey = "UrlWebAPI";
    private const string UrlWebApiFallbackKey = "URLWebAPI";
    private const string NombreField = "Nombre";
    private const string IdField = "id";
    private const string GenericActionErrorMessage = "No ha sido posible realizar la acción. Inténtelo nuevamente.";

    public async Task<IActionResult> Index(string? s)
    {
        List<Producto>? lista = [];

        try
        {
            lista = await productos.GetAsync(NormalizeSearch(s));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        if (User.FindFirstValue(ClaimTypes.Role) == "Administrador")
            ViewBag.SoloAdmin = true;

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        ViewBag.search = NormalizeSearch(s);

        return View(lista);
    }

    public async Task<IActionResult> Detalle(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Producto? item = null;
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        try
        {
            item = await productos.GetAsync(id);
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
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        await ProductosDropDownListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CrearAsync(Producto itemToCreate)
    {
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid)
        {
            await ProductosDropDownListAsync(itemToCreate.ArchivoId);
            ModelState.AddModelError(NombreField, GenericActionErrorMessage);
            return View(itemToCreate);
        }

        try
        {
            await productos.PostAsync(itemToCreate);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        await ProductosDropDownListAsync(itemToCreate.ArchivoId);
        ModelState.AddModelError(NombreField, GenericActionErrorMessage);
        return View(itemToCreate);
    }

    public async Task<IActionResult> EditarAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Producto? itemToEdit = null;
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        try
        {
            itemToEdit = await productos.GetAsync(id);
            if (itemToEdit == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        if (itemToEdit == null) return View("Error");

        await ProductosDropDownListAsync(itemToEdit.ArchivoId);
        return View(itemToEdit);
    }

    [HttpPost]
    public async Task<IActionResult> EditarAsync(int id, Producto itemToEdit)
    {
        if (id != itemToEdit.ProductoId) return NotFound();

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        if (!ModelState.IsValid)
        {
            await ProductosDropDownListAsync(itemToEdit.ArchivoId);
            ModelState.AddModelError(NombreField, GenericActionErrorMessage);
            return View(itemToEdit);
        }

        try
        {
            await productos.PutAsync(itemToEdit);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        await ProductosDropDownListAsync(itemToEdit.ArchivoId);
        ModelState.AddModelError(NombreField, GenericActionErrorMessage);
        return View(itemToEdit);
    }

    [HttpGet]
        public async Task<IActionResult> Eliminar(int id, bool showError)
            {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Producto? itemToDelete = null;

        try
        {
            itemToDelete = await productos.GetAsync(id);
            if (itemToDelete == null) return NotFound();

            if (showError)
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
            await productos.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(Eliminar), new { id, showError = true });
    }

    [AcceptVerbs("GET", "POST")]
    public IActionResult ValidaPoster(string Poster)
    {
        if (Uri.IsWellFormedUriString(Poster, UriKind.Absolute) || Poster.Equals("N/A"))
            return Json(true);

        return Json(false);
    }

    public async Task<IActionResult> Categorias(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        Producto? itemToView = null;

        try
        {
            itemToView = await productos.GetAsync(id);
            if (itemToView == null) return NotFound();
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        if (itemToView == null) return View("Error");

        ViewData["ProductoId"] = itemToView.ProductoId;
        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];

        return View(itemToView);
    }

    public async Task<IActionResult> CategoriasAgregar(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ProductoCategoria? itemToView = null;

        try
        {
            Producto? producto = await productos.GetAsync(id);
            if (producto == null) return NotFound();

            await CategoriasDropDownListAsync();
            itemToView = new ProductoCategoria { Producto = producto };
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        return View(itemToView);
    }

    [HttpPost]
    public async Task<IActionResult> CategoriasAgregar(int id, int categoriaid)
    {
        Producto? producto = null;

        if (!ModelState.IsValid)
        {
            ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
            ModelState.AddModelError(IdField, GenericActionErrorMessage);
            await CategoriasDropDownListAsync();
            return View(new ProductoCategoria { Producto = producto });
        }

        try
        {
            producto = await productos.GetAsync(id);
            if (producto == null) return NotFound();

            Categoria? categoria = await categorias.GetAsync(categoriaid);
            if (categoria == null) return NotFound();

            await productos.PostAsync(id, categoriaid);
            return RedirectToAction(nameof(Categorias), new { id });
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        ModelState.AddModelError(IdField, GenericActionErrorMessage);
        await CategoriasDropDownListAsync();

        return View(new ProductoCategoria { Producto = producto });
    }

    [HttpGet]
public async Task<IActionResult> CategoriasRemover(int id, int categoriaid, bool showError)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        ProductoCategoria? itemToView = null;

        try
        {
            Producto? producto = await productos.GetAsync(id);
            if (producto == null) return NotFound();

            Categoria? categoria = await categorias.GetAsync(categoriaid);
            if (categoria == null) return NotFound();

            itemToView = new ProductoCategoria
            {
                Producto = producto,
                CategoriaId = categoriaid,
                Nombre = categoria.Nombre
            };

           if (showError)
                ViewData["ErrorMessage"] = GenericActionErrorMessage;
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        ViewBag.Url = configuration[UrlWebApiKey] ?? configuration[UrlWebApiFallbackKey];
        return View(itemToView);
    }

    [HttpPost]
    public async Task<IActionResult> CategoriasRemover(int id, int categoriaid)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(CategoriasRemover), new { id, categoriaid, showError = true });
        }

        try
        {
            await productos.DeleteAsync(id, categoriaid);
            return RedirectToAction(nameof(Categorias), new { id });
        }
        catch (HttpRequestException ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return RedirectToAction(SalirAction, AuthController);
        }

        return RedirectToAction(nameof(CategoriasRemover), new { id, categoriaid, showError = true });
    }

    private async Task CategoriasDropDownListAsync(object? itemSeleccionado = null)
    {
        var listado = await categorias.GetAsync();
        ViewBag.Categoria = new SelectList(listado, "CategoriaId", "Nombre", itemSeleccionado);
    }

    private async Task ProductosDropDownListAsync(object? itemSeleccionado = null)
    {
        var listado = await archivos.GetAsync();
        ViewBag.Archivo = new SelectList(listado, "ArchivoId", "Nombre", itemSeleccionado);
    }

    private static string? NormalizeSearch(string? search)
    {
        var value = search?.Trim();
        return string.IsNullOrWhiteSpace(value) ? null : value[..Math.Min(value.Length, 40)];
    }
}
