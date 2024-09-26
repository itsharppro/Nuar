using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nuar
{
    internal interface IValueProvider
    {
        IEnumerable<string> Tokens { get; }
        string GetValue(string value, HttpRequest httpRequest, RouteData routeData);
    }
}