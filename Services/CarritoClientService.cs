using frontendnet.Models;

namespace frontendnet.Services;

public class CarritoClientService(HttpClient client)
{
    public async Task<CarritoResumen?> GetAsync()
    {
        return await client.GetFromJsonAsync<CarritoResumen>("api/carrito");
    }

    public async Task AgregarAsync(int productoId, int cantidad = 1)
    {
        var response = await client.PostAsJsonAsync("api/carrito", new { productoid = productoId, cantidad });
        response.EnsureSuccessStatusCode();
    }

    public async Task ActualizarAsync(int productoId, int cantidad)
    {
        var response = await client.PutAsJsonAsync($"api/carrito/{productoId}", new { cantidad });
        response.EnsureSuccessStatusCode();
    }

    public async Task EliminarAsync(int productoId)
    {
        var response = await client.DeleteAsync($"api/carrito/{productoId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task VaciarAsync()
    {
        var response = await client.DeleteAsync("api/carrito");
        response.EnsureSuccessStatusCode();
    }
}
