using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Nuar
{
    public interface IRouteConfigurator
    {
        RouteConfig Configure(Module module, Route route);
    }
}