using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class CategoriasClientService(HttpClient client)
{
    public async Task<List<Categoria>?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Categoria>>(client, "api/categorias");
    }

    public async Task<Categoria?> GetAsync(int id)
    {
        return await ApiErrorMapper.GetFromJsonAsync<Categoria>(client, $"api/categorias/{id}");
    }

    public async Task PostAsync(Categoria categoria)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/categorias", categoria);
    }

    public async Task PutAsync(Categoria categoria)
    {
        await ApiErrorMapper.PutAsJsonAsync(client, $"api/categorias/{categoria.CategoriaId}", categoria);
    }

    public async Task DeleteAsync(int id)
    {
        await ApiErrorMapper.DeleteAsync(client, $"api/categorias/{id}");
    }
}
