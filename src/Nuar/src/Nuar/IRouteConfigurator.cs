using Nuar.Configuration;

namespace Nuar
{
    public interface IRouteConfigurator
    {
        RouteConfig Configure(Module module, Route route);
    }
}