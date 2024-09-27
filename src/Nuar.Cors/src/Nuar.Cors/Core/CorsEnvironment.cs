using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nuar.Cors.Core
{
    public class CorsEnvironment
    {
        public IEnumerable<string> AllowedOrigins { get; set; }
    }
}