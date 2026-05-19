using System.Security.Claims;
using frontendnet.Services;

namespace frontendnet.Middlewares;

public class RefrescaTokenDelegatingHandler(AuthClientService auth, IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.Headers.Contains("Set-Authorization"))
        {
            string jwt = response.Headers.GetValues("Set-Authorization").FirstOrDefault()!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)!),
                new(ClaimTypes.GivenName, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.GivenName)!),
                new("jwt", jwt),
                new(ClaimTypes.Role, httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role)!),
                new("emailConfirmado", httpContextAccessor.HttpContext?.User.FindFirstValue("emailConfirmado") ?? "False"),
            };

            await auth.IniciaSesionAsync(claims);
        }

        return response;
    }
}
