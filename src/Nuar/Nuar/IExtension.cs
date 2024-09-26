using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Nuar
{
    public interface IExtension
    {
        string Name { get; }
        string Description { get; }
        string Version { get; }
        void Add(IServiceCollection services, IOptionsProvider optionsProvider);
        void Use(IApplicationBuilder app, IOptionsProvider optionsProvider);
    }
}