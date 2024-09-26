using Microsoft.AspNetCore.Http;

namespace Nuar
{
    internal interface IRequestExecutionValidator
    {
        Task<bool> TryExecuteAsync(HttpContext httpContext, RouteConfig routeConfig);
    }
}