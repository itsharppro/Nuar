using System;
using System.Collections.Generic;
using System.Linq;

namespace Nuar.Cors.Core
{
    public class CorsOptionsBuilder : IOptionsBuilder<CorsOptions>
    {
        private readonly CorsOptions _options;

        public CorsOptionsBuilder()
        {
            _options = new CorsOptions
            {
                AllowedOrigins = new List<string>(),
                AllowedMethods = new List<string>(),
                AllowedHeaders = new List<string>(),
                ExposedHeaders = new List<string>(),
                Environment = new Dictionary<string, CorsEnvironment>()
            };
        }

        public CorsOptionsBuilder AllowCredentials(bool allow)
        {
            _options.AllowCredentials = allow;
            return this;
        }

        public CorsOptionsBuilder WithAllowedOrigins(params string[] origins)
        {
            _options.AllowedOrigins = origins;
            return this;
        }

        public CorsOptionsBuilder WithDeniedOrigins(params string[] deniedOrigins)
        {
            _options.DeniedOrigins = deniedOrigins;
            return this;
        }

        public CorsOptionsBuilder WithAllowedMethods(params string[] methods)
        {
            _options.AllowedMethods = methods;
            return this;
        }

        public CorsOptionsBuilder WithAllowedHeaders(params string[] headers)
        {
            _options.AllowedHeaders = headers;
            return this;
        }

        public CorsOptionsBuilder WithExposedHeaders(params string[] headers)
        {
            _options.ExposedHeaders = headers;
            return this;
        }

        public CorsOptionsBuilder WithMaxAge(int maxAge)
        {
            _options.MaxAge = maxAge;
            return this;
        }

        public CorsOptionsBuilder EnableLogging(bool enable)
        {
            _options.LoggingEnabled = enable;
            return this;
        }

        public CorsOptionsBuilder WithEnvironment(string environment, CorsEnvironment corsEnvironment)
        {
            _options.Environment[environment] = corsEnvironment;
            return this;
        }

        public CorsOptions Build() => _options;
    }
}
