using frontendnet.Models;
using frontendnet.Services.Errors;

namespace frontendnet.Services;

public class RolesClientService(HttpClient client)
{
    public async Task<List<Rol>?> GetAsync()
    {
        return await ApiErrorMapper.GetFromJsonAsync<List<Rol>>(client, "api/roles");
    }
}
