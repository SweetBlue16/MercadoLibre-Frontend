using frontendnet.Models;
using frontendnet.Services.Errors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace frontendnet.Services;

public class AuthClientService(HttpClient client, IHttpContextAccessor httpContextAccessor)
{
    public async Task<AuthUser?> ObtenTokenAsync(string email, string password)
    {
        Login usuario = new() { Email = email, Password = password };

        // Realizo la llamada al Web API
        var response = await ApiErrorMapper.PostAsJsonAsync(client, "api/auth", usuario);

        var token = await response.Content.ReadFromJsonAsync<AuthUser>();

        return token;
    }

    public async Task<bool> RegistrarAsync(Registro registro)
    {
        var payload = new
        {
            registro.Email,
            registro.Nombre,
            registro.Password
        };

        await ApiErrorMapper.PostAsJsonAsync(client, "api/usuarios/registro", payload);

        return true;
    }

    public async Task ConfirmarCorreoAsync(ConfirmarCorreo model)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/auth/confirmar-correo", new { model.Email, codigo = model.Codigo });
    }

    public async Task ReenviarConfirmacionAsync(string email)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/auth/reenviar-confirmacion", new { email });
    }

    public async Task SolicitarResetPasswordAsync(OlvidePassword model)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/auth/olvide-password", new { model.Email });
    }

    public async Task RestablecerPasswordAsync(RestablecerPassword model)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/auth/restablecer-password", model);
    }

    public async Task IniciaSesionAsync(List<Claim> claims)
    {
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = false,
            AllowRefresh = true,
        };

        if (httpContextAccessor.HttpContext is not null)
            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }

    public async Task CerrarSesionAsync()
{
    if (httpContextAccessor.HttpContext is null) return;

    var token = httpContextAccessor.HttpContext.User.FindFirstValue("jwt");

    if (!string.IsNullOrWhiteSpace(token))
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            using var response = await client.SendAsync(request);
        }
        catch
        {
            // El cierre local debe completarse aunque el API no este disponible.
        }
    }

    await httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    httpContextAccessor.HttpContext.Response.Cookies.Delete(".frontendnet", new CookieOptions
    {
        Path = "/",
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        Secure = true,
    });
}
}
