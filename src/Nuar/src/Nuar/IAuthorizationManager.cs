using System.Security.Claims;

namespace Nuar
{
    public interface IAuthorizationManager
    {
        bool IsAuthorized(ClaimsPrincipal user, RouteConfig routeConfig);
    }
}