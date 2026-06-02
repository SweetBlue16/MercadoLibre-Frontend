using System.Security.Claims;
using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

public class AuthController(AuthClientService auth) : Controller
{
    private const string ConfirmacionEmailTempDataKey = "ConfirmacionEmail";

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [AllowAnonymous]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Registro(Registro model)
    {
        if (string.Equals(model.Email, model.Password, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(model.Password), "La contraseña no puede ser igual al correo.");

        if (!ModelState.IsValid) return View(model);

        try
        {
            await auth.RegistrarAsync(model);
            TempData[ConfirmacionEmailTempDataKey] = model.Email;
            return RedirectToAction(nameof(ConfirmarCorreo));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [AllowAnonymous]
    public IActionResult ConfirmarCorreo(string? email)
    {
        var emailProtegido = TempData.Peek(ConfirmacionEmailTempDataKey) as string;
        return View(new ConfirmarCorreo
        {
            Email = !string.IsNullOrWhiteSpace(emailProtegido) ? emailProtegido : email ?? string.Empty,
            EmailBloqueado = !string.IsNullOrWhiteSpace(emailProtegido)
        });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmarCorreo(ConfirmarCorreo model)
    {
        var emailProtegido = TempData.Peek(ConfirmacionEmailTempDataKey) as string;
        if (!string.IsNullOrWhiteSpace(emailProtegido))
        {
            model.Email = emailProtegido;
            model.EmailBloqueado = true;
        }

        if (!ModelState.IsValid) return View(model);

        try
        {
            await auth.ConfirmarCorreoAsync(model);
            TempData.Remove(ConfirmacionEmailTempDataKey);
            TempData["Mensaje"] = "Correo confirmado correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            if (model.EmailBloqueado) TempData.Keep(ConfirmacionEmailTempDataKey);
            return View(model);
        }
    }

    [HttpPost]
    [AllowAnonymous]

public async Task<IActionResult> ReenviarConfirmacion(ConfirmarCorreo model)
{
    var emailProtegido = TempData.Peek(ConfirmacionEmailTempDataKey) as string;

    if (!string.IsNullOrWhiteSpace(emailProtegido))
    {
        model.Email = emailProtegido;
        model.EmailBloqueado = true;
    }

    if (!ModelState.IsValid)
    {
        if (model.EmailBloqueado) TempData.Keep(ConfirmacionEmailTempDataKey);
        return View(nameof(ConfirmarCorreo), model);
    }

    if (string.IsNullOrWhiteSpace(model.Email))
    {
        ModelState.AddModelError(nameof(model.Email), "El correo es obligatorio.");
        return View(nameof(ConfirmarCorreo), model);
    }

    try
    {
        await auth.ReenviarConfirmacionAsync(model.Email);
        ViewData["Mensaje"] = "Si la cuenta requiere confirmación, recibirás un nuevo código.";

        if (model.EmailBloqueado) TempData.Keep(ConfirmacionEmailTempDataKey);
    }
    catch (HttpRequestException ex)
    {
        ModelState.AddModelError(string.Empty, ex.Message);

        if (model.EmailBloqueado) TempData.Keep(ConfirmacionEmailTempDataKey);
    }

    return View(nameof(ConfirmarCorreo), model);
}

    [AllowAnonymous]
    public IActionResult OlvidePassword()
    {
        return View(new OlvidePassword());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> OlvidePassword(OlvidePassword model)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await auth.SolicitarResetPasswordAsync(model);
            TempData["Mensaje"] = "Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña.";
            return RedirectToAction(nameof(RestablecerPassword), new { email = model.Email });
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [AllowAnonymous]
    public IActionResult RestablecerPassword(string? email)
    {
        return View(new RestablecerPassword { Email = email ?? string.Empty });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> RestablecerPassword(RestablecerPassword model)
    {
        if (string.Equals(model.Email, model.Password, StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(model.Password), "La contraseña no puede ser igual al correo.");

        if (!ModelState.IsValid) return View(model);

        try
        {
            await auth.RestablecerPasswordAsync(model);
            TempData["Mensaje"] = "Contraseña restablecida correctamente. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Login model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var token = await auth.ObtenTokenAsync(model.Email, model.Password);

                if (token == null)
                {
                    ModelState.AddModelError("Email", "Credenciales no válidas. Inténtelo nuevamente.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, token.Email),
                    new(ClaimTypes.GivenName, token.Nombre),
                    new("jwt", token.Jwt),
                    new(ClaimTypes.Role, token.Rol),
                    new("emailConfirmado", token.EmailConfirmado.ToString()),
                };

                await auth.IniciaSesionAsync(claims);

                if (token.Rol == "Administrador")
                    return RedirectToAction("Index", "Productos");

                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError("Email", ex.Message);
            }
        }

        return View(model);
    }

    [AcceptVerbs("GET", "POST")]
    [Authorize(Roles = "Administrador, Usuario")]
    public async Task<IActionResult> SalirAsync()
    {
        await auth.CerrarSesionAsync();
        Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.Expires = "0";
        TempData["Mensaje"] = "Tu sesion se cerro correctamente.";
        return RedirectToAction("Index", "Auth");
    }
}
