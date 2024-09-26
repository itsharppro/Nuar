using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Nuar
{
    public interface IRequestHandlerManager
    {
        IHandler GetHandler(string name);
        void AddHandler(string name, IHandler handler);
        Task HandleAsync(string handler, HttpContext httpContext, RouteConfig config);
    }
}