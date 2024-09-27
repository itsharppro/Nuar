using System.Collections.Generic;

namespace Nuar.Cors.Core
{
    public class CorsOptions : IOptions
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
