using System.Security.Claims;
using frontendnet.Models;
using frontendnet.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace frontendnet;

[Authorize]
public class PerfilController(PerfilClientService perfil) : Controller
{
    private const string MostrarCambioPasswordKey = "MostrarCambioPassword";

    public async Task<IActionResult> IndexAsync()
    {
        var model = await CrearPerfilViewModelAsync();
        if (model is null) return RedirectToAction("Salir", "Auth");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EnviarCodigoCambioPassword()
    {
        var model = await CrearPerfilViewModelAsync();
        if (model is null) return RedirectToAction("Salir", "Auth");

        try
        {
            await perfil.EnviarCodigoCambioPasswordAsync();
            TempData["Mensaje"] = "Código de verificación enviado.";
            TempData[MostrarCambioPasswordKey] = "true";
            return RedirectToAction("Index");
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> CambiarPassword([Bind(Prefix = "CambioPassword")] CambioPassword cambioPassword)
    {
        var model = await CrearPerfilViewModelAsync();
        if (model is null) return RedirectToAction("Salir", "Auth");

        model.CambioPassword = cambioPassword;
        model.CodigoCambioPasswordSolicitado = true;
        if (!ModelState.IsValid) return View("Index", model);

        try
        {
            await perfil.CambiarPasswordAsync(cambioPassword);
            TempData["Mensaje"] = "Contraseña actualizada correctamente.";
            TempData.Remove(MostrarCambioPasswordKey);
            return RedirectToAction("Index");
        }
        catch (HttpRequestException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("Index", model);
        }
    }

    private async Task<PerfilViewModel?> CrearPerfilViewModelAsync()
    {
        var email = User.FindFirstValue(ClaimTypes.Name);
        var nombre = User.FindFirstValue(ClaimTypes.GivenName);
        var rol = User.FindFirstValue(ClaimTypes.Role);
        var jwt = User.FindFirstValue("jwt");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(rol) || string.IsNullOrWhiteSpace(jwt))
            return null;

        return new PerfilViewModel
        {
            Usuario = new AuthUser
            {
                Email = email,
                Nombre = nombre,
                Rol = rol,
                Jwt = jwt,
                EmailConfirmado = bool.TryParse(User.FindFirstValue("emailConfirmado"), out var confirmado) && confirmado,
            },
            CodigoCambioPasswordSolicitado = string.Equals(TempData.Peek(MostrarCambioPasswordKey) as string, "true", StringComparison.OrdinalIgnoreCase),
        };
    }
}
