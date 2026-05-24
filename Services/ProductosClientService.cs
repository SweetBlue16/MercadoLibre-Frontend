using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class ProductosClientService(HttpClient client)
{
    public async Task<List<Producto>?> GetAsync(string? search)
    {
        var query = string.IsNullOrWhiteSpace(search) ? string.Empty : $"?s={Uri.EscapeDataString(search)}";
        return await ApiErrorMapper.GetFromJsonAsync<List<Producto>>(client, $"api/productos{query}");
    }

    public async Task<Producto?> GetAsync(int id)
    {
        return await ApiErrorMapper.GetFromJsonAsync<Producto>(client, $"api/productos/{id}");
    }

    public async Task PostAsync(Producto producto)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/productos", producto);
    }

    public async Task PutAsync(Producto producto)
    {
        await ApiErrorMapper.PutAsJsonAsync(client, $"api/productos/{producto.ProductoId}", producto);
    }

    public async Task DeleteAsync(int id)
    {
        await ApiErrorMapper.DeleteAsync(client, $"api/productos/{id}");
    }

    public async Task PostAsync(int id, int categoriaid)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, $"api/productos/{id}/categoria", new { categoriaid });
    }

    public async Task DeleteAsync(int id, int categoriaid)
    {
        await ApiErrorMapper.DeleteAsync(client, $"api/productos/{id}/categoria/{categoriaid}");
    }
}
