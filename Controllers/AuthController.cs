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
    private const string MensajeKey = "Mensaje";

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Registro()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Registro(Registro model)
    {
        model.Email = model.Email?.Trim() ?? string.Empty;
        model.Nombre = model.Nombre?.Trim() ?? string.Empty;

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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult ConfirmarCorreo(string? email)
    {
        var emailProtegido = TempData.Peek(ConfirmacionEmailTempDataKey) as string;
        var resolvedEmail = !string.IsNullOrWhiteSpace(emailProtegido) ? emailProtegido : email ?? string.Empty;
        if (string.IsNullOrWhiteSpace(resolvedEmail))
        {
            ViewData[MensajeKey] = "No hay una verificación de correo pendiente. Regístrate nuevamente.";
        }

        return View(new ConfirmarCorreo
        {
            Email = resolvedEmail,
            EmailBloqueado = !string.IsNullOrWhiteSpace(emailProtegido)
        });
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmarCorreo(ConfirmarCorreo model)
    {
        AplicarEmailProtegido(model);
        if (!ModelState.IsValid) return View(model);

        try
        {
            await auth.ConfirmarCorreoAsync(model);
            TempData.Remove(ConfirmacionEmailTempDataKey);
            TempData[MensajeKey] = "Correo confirmado correctamente. Ya puedes iniciar sesión.";
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
        AplicarEmailProtegido(model);
        ModelState.Remove(nameof(model.Codigo));

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "No hay una verificación de correo pendiente. Regístrate nuevamente.");
        }

        if (!ModelState.IsValid)
        {
            if (model.EmailBloqueado) TempData.Keep(ConfirmacionEmailTempDataKey);
            return View(nameof(ConfirmarCorreo), model);
        }

        try
        {
            await auth.ReenviarConfirmacionAsync(model.Email);
            ViewData[MensajeKey] = "Si la cuenta requiere confirmación, recibirás un nuevo código.";
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }

        if (model.EmailBloqueado) TempData.Keep(ConfirmacionEmailTempDataKey);
        return View(nameof(ConfirmarCorreo), model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult OlvidePassword()
    {
        return View(new OlvidePassword());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> OlvidePassword(OlvidePassword model)
    {
        model.Email = model.Email?.Trim() ?? string.Empty;
        if (!ModelState.IsValid) return View(model);

        try
        {
            await auth.SolicitarResetPasswordAsync(model);
            TempData[MensajeKey] = "Si el correo está registrado, recibirás instrucciones para restablecer tu contraseña.";
            return RedirectToAction(nameof(RestablecerPassword), new { email = model.Email });
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
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
            TempData[MensajeKey] = "Contraseña restablecida correctamente. Ya puedes iniciar sesión.";
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
        if (!ModelState.IsValid) return View(model);

        try
        {
            var token = await auth.ObtenTokenAsync(model.Email.Trim(), model.Password);
            if (token == null)
            {
                ModelState.AddModelError(nameof(model.Email), "Credenciales incorrectas.");
                return View(model);
            }

            await auth.IniciaSesionAsync(BuildClaims(token));
            return token.Rol == "Administrador"
                ? RedirectToAction("Index", "Productos")
                : RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(nameof(model.Email), ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Administrador, Usuario")]
    public async Task<IActionResult> SalirAsync()
    {
        await auth.CerrarSesionAsync();
        Response.Headers.CacheControl = "no-store, no-cache, must-revalidate";
        Response.Headers.Pragma = "no-cache";
        Response.Headers.Expires = "0";
        TempData[MensajeKey] = "Tu sesión se cerró correctamente.";
        return RedirectToAction("Index", "Auth");
    }

    private void AplicarEmailProtegido(ConfirmarCorreo model)
    {
        var emailProtegido = TempData.Peek(ConfirmacionEmailTempDataKey) as string;
        if (string.IsNullOrWhiteSpace(emailProtegido)) return;

        model.Email = emailProtegido;
        model.EmailBloqueado = true;
    }

    private static List<Claim> BuildClaims(AuthUser token)
    {
        return
        [
            new Claim(ClaimTypes.Name, token.Email),
            new Claim(ClaimTypes.GivenName, token.Nombre),
            new Claim("jwt", token.Jwt),
            new Claim(ClaimTypes.Role, token.Rol),
            new Claim("emailConfirmado", token.EmailConfirmado.ToString()),
        ];
    }
}
