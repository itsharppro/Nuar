using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NuarRoute = Nuar.Configuration.Route;

namespace Nuar
{
    internal interface IPayloadTransformer
    {
        bool HasTransformations(string resourceId, NuarRoute route);
        PayloadSchema Transform(string payload, string resourceId, NuarRoute route, HttpRequest request, RouteData data);
    }
}