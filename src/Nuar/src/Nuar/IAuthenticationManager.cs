using Microsoft.AspNetCore.Http;

namespace Nuar
{
    public interface IAuthenticationManager
    {
        Task<bool> TryAuthenticateAsync(HttpRequest request, RouteConfig routeConfig);
    }
}