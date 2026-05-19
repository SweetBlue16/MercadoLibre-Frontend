using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class PedidosClientService(HttpClient client)
{
    public async Task<List<Pedido>?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Pedido>>(client, "api/pedidos");
    }

    public async Task<Pedido?> GetAsync(int id)
    {
        return await ApiErrorMapper.GetFromJsonAsync<Pedido>(client, $"api/pedidos/{id}");
    }

    public async Task<Pedido?> ConfirmarAsync()
    {
        var response = await ApiErrorMapper.PostAsync(client, "api/pedidos/confirmar", null);
        return await response.Content.ReadFromJsonAsync<Pedido>();
    }

    public async Task<List<Pedido>?> GetMisPedidosAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Pedido>>(client, "api/pedidos/mios");
    }

    public async Task<Pedido?> GetMiPedidoAsync(int id)
    {
        return await ApiErrorMapper.GetFromJsonAsync<Pedido>(client, $"api/pedidos/mios/{id}");
    }

    public async Task ActualizarEstadoAsync(int id, string estado)
    {
        await ApiErrorMapper.PutAsJsonAsync(client, $"api/pedidos/{id}/estado", new { estado });
    }
}
