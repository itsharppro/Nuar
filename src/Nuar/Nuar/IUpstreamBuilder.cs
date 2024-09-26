using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Nuar
{
    internal interface IUpstreamBuilder
    {
        string Build(Module module, Route route);
    }
}