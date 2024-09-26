using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar.WebApi
{
    public class WebApiEndpointDefinition
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public IEnumerable<WebApiEndpointParameter> Parameters { get; set; } = new List<WebApiEndpointParameter>();
        public IEnumerable<WebApiEndpointResponse> Responses { get; set; } = new List<WebApiEndpointResponse>();
    }
}