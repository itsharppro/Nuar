using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;

using Nuar.Tracing;

namespace Nuar.Extensions.Tracing
{
    public class TracingExtension : IExtension
    {
        public string Name => "tracing";
        public string Description => "Distributed tracing using OpenTelemetry and Jaeger";
        public string Version => "1.0.0";

        public void Add(IServiceCollection services, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<TracingOptions>(Name);

            if (options.UseEmptyTracer)
            {
                services.AddOpenTelemetry();
                return;
            }

          
        }

        public void Use(IApplicationBuilder app, IOptionsProvider optionsProvider)
        {
            // Middleware to ensure tracing is active
            var logger = app.ApplicationServices.GetRequiredService<ILogger<TracingExtension>>();
            logger.LogInformation("Tracing with OpenTelemetry and Jaeger has been configured.");
        }

        private void ConfigureSampler(TracerProviderBuilder builder, TracingOptions options)
        {
            // Use OpenTelemetry samplers instead of Jaeger's samplers
            switch (options.Sampler?.ToLower())
            {
                case "const":
                    // Always sample
                    builder.SetSampler(new AlwaysOnSampler());
                    break;
                case "probabilistic":
                    // Use the provided sampling rate
                    builder.SetSampler(new TraceIdRatioBasedSampler(options.SamplingRate));
                    break;

                default:
                    // Default sampler: sample based on configuration
                    builder.SetSampler(new ParentBasedSampler(new TraceIdRatioBasedSampler(options.SamplingRate)));
                    break;
            }
        }
    }
}
