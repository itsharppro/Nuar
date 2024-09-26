using Microsoft.AspNetCore.Routing;

namespace Nuar
{
    internal interface IRouteProvider
    {
        Action<IEndpointRouteBuilder> Build();
    }
}