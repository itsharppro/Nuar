using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NuarRoute = Nuar.Configuration.Route;

namespace Nuar
{
    public interface IHandler
    {
        string GetInfo(NuarRoute route);
        Task HandleAsync(HttpContext context, RouteConfig config);
    }
}