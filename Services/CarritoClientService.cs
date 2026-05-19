using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class CarritoClientService(HttpClient client)
{
    public async Task<CarritoResumen?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<CarritoResumen>(client, "api/carrito");
    }

    public async Task AgregarAsync(int productoId, int cantidad = 1)
    {
        await ApiErrorMapper.PostAsJsonAsync(client, "api/carrito", new { productoid = productoId, cantidad });
    }

    public async Task ActualizarAsync(int productoId, int cantidad)
    {
        await ApiErrorMapper.PutAsJsonAsync(client, $"api/carrito/{productoId}", new { cantidad });
    }

    public async Task EliminarAsync(int productoId)
    {
        await ApiErrorMapper.DeleteAsync(client, $"api/carrito/{productoId}");
    }

    public async Task VaciarAsync()
    {
        await ApiErrorMapper.DeleteAsync(client, "api/carrito");
    }
}
