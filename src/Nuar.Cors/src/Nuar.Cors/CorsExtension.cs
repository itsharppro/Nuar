using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nuar.Cors;
using Nuar.Cors.Core;

namespace Nuar.Extensions.Cors
{
    public class CorsExtension : IExtension
    {
        public string Name => "cors";
        public string Description => "Cross-Origin Resource Sharing";
        public string Version => "1.0.0";

        public void Add(IServiceCollection services, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<CorsOptions>(Name); // Ensure CorsOptions implements IOptions

            services.AddCors(cors =>
            {
                var allowedHeaders = options.AllowedHeaders ?? Enumerable.Empty<string>();
                var allowedMethods = options.AllowedMethods ?? Enumerable.Empty<string>();
                var allowedOrigins = options.AllowedOrigins ?? Enumerable.Empty<string>();
                var exposedHeaders = options.ExposedHeaders ?? Enumerable.Empty<string>();

                cors.AddPolicy("CorsPolicy", builder =>
                {
                    var origins = allowedOrigins.ToArray();
                    if (options.AllowCredentials && origins.FirstOrDefault() != "*")
                    {
                        builder.AllowCredentials();
                    }
                    else
                    {
                        builder.DisallowCredentials();
                    }

                    // Handle Denied Origins
                    if (options.DeniedOrigins != null && options.DeniedOrigins.Any())
                    {
                        origins = origins.Except(options.DeniedOrigins).ToArray();
                    }

                    builder.WithHeaders(allowedHeaders.ToArray())
                        .WithMethods(allowedMethods.ToArray())
                        .WithOrigins(origins)
                        .WithExposedHeaders(exposedHeaders.ToArray());

                    // Apply MaxAge
                    if (options.MaxAge.HasValue)
                    {
                        builder.SetPreflightMaxAge(TimeSpan.FromSeconds(options.MaxAge.Value));
                    }
                });
            });
        }

        public void Use(IApplicationBuilder app, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<CorsOptions>(Name); // Ensure CorsOptions implements IOptions

            // Handle Logging
            if (options.LoggingEnabled)
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<CorsExtension>>();
                logger.LogInformation("CORS policy is applied.");
            }

            app.UseCors("CorsPolicy");
        }
    }
}
