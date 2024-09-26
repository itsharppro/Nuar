using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nuar
{
    internal interface IDownstreamBuilder
    {
        string GetDownstream(RouteConfig routeConfig, HttpRequest request, RouteData data);
    }
}