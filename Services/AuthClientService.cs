using frontendnet.Models;
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
        var response = await client.PostAsJsonAsync("api/auth", usuario);
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

        var response = await client.PostAsJsonAsync("api/usuarios/registro", payload);
        return response.IsSuccessStatusCode;
    }

    public async Task IniciaSesionAsync(List<Claim> claims)
    {
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties();

        if (httpContextAccessor.HttpContext is not null)
            await httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
    }
}
