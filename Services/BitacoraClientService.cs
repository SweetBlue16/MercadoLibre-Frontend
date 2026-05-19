using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class BitacoraClientService(HttpClient client)
{
    public async Task<List<Bitacora>?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Bitacora>>(client, "api/bitacora");
    }
}
