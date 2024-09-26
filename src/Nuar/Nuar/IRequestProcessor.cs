using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nuar
{
    public interface IRequestProcessor
    {
        Task<ExecutionData> ProcessAsync(RouteConfig routeConfig, HttpContext httpContext);
    }
}