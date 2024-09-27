using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nuar.Cors.Core;

namespace Nuar.Cors
{
    public class CorsOptions
    {
        public bool AllowCredentials { get; set; }
        public IEnumerable<string> AllowedOrigins { get; set; }
        public IEnumerable<string> DeniedOrigins { get; set; }
        public IEnumerable<string> AllowedMethods { get; set; }
        public IEnumerable<string> AllowedHeaders { get; set; }
        public IEnumerable<string> ExposedHeaders { get; set; }
        public int? MaxAge { get; set; }
        public bool LoggingEnabled { get; set; }  
        public Dictionary<string, CorsEnvironment> Environment { get; set; }
    }
}