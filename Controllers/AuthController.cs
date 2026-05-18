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

        if (ModelState.IsValid)
        {
            try
            {
                var creado = await auth.RegistrarAsync(model);
                if (creado)
                    return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(string.Empty, "No fue posible conectar con el API. Intente nuevamente.");
            }
        }

        ModelState.AddModelError(nameof(model.Email), "No fue posible crear la cuenta. Revise los datos capturados.");
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IndexAsync(Login model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                // Esta función verifica en backend que el correo y contraseña sean válidos
                var token = await auth.ObtenTokenAsync(model.Email, model.Password);

                if (token == null)
                {
                    ModelState.AddModelError("Email", "Credenciales no válidas. Inténtelo nuevamente.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    // Todo esto se guarda en la Cookie
                    new(ClaimTypes.Name, token.Email),
                    new(ClaimTypes.GivenName, token.Nombre),
                    new("jwt", token.Jwt),
                    new(ClaimTypes.Role, token.Rol),
                };

                await auth.IniciaSesionAsync(claims);

                // Usuario válido
                if (token.Rol == "Administrador")
                    return RedirectToAction("Index", "Productos");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("Email", "Credenciales no válidas. Inténtelo nuevamente.");
            }
        }

        return View(model);
    }

    [Authorize(Roles = "Administrador, Usuario")]
    public async Task<IActionResult> SalirAsync()
    {
        // Cierra la sesión
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Sino, se redirige a la página inicial
        return RedirectToAction("Index", "Auth");
    }
}
