using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class UsuariosClientService(HttpClient client)
{
    public async Task<List<Usuario>?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Usuario>>(client, "api/usuarios");
    }

    public async Task<Usuario?> GetAsync(string email)
    {
        return await ApiErrorMapper.GetFromJsonAsync<Usuario>(client, $"api/usuarios/{email}");
    }

    public async Task PostAsync(UsuarioPwd usuario)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/usuarios", usuario);
    }

    public async Task PutAsync(Usuario usuario)
    {
        await ApiErrorMapper.PutAsJsonAsync(client, $"api/usuarios/{usuario.Email}", usuario);
    }

    public async Task DeleteAsync(string email)
    {
        await ApiErrorMapper.DeleteAsync(client, $"api/usuarios/{email}");
    }
}
