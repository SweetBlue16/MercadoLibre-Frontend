using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class PerfilClientService(HttpClient client)
{
    public async Task CambiarPasswordAsync(Models.CambioPassword model)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/auth/cambiar-password", model);
    }

    public async Task EnviarCodigoCambioPasswordAsync()
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/auth/cambiar-password/enviar-codigo", new { });
    }
}
