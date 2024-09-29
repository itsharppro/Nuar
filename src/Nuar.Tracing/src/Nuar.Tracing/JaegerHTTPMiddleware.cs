using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpenTracing;
using OpenTracing.Tag;
using System.Collections.Generic;

namespace Nuar.Extensions.Tracing
{
    internal sealed class JaegerHttpMiddleware
    {
        private readonly RequestDelegate _next;

        public JaegerHttpMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ITracer tracer)
        {
            IScope scope = null;
            var span = tracer.ActiveSpan;
            var method = context.Request.Method;

            if (span == null)
            {
                var spanBuilder = tracer.BuildSpan($"HTTP {method}");
                scope = spanBuilder.StartActive(true);
                span = scope.Span;
            }

            span.Log(new Dictionary<string, object>
            {
                { "event", "request_processing" },
                { "method", method },
                { "path", context.Request.Path.ToString() }
            });

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                span.SetTag(Tags.Error, true);
                span.Log(new Dictionary<string, object>
                {
                    { "event", "error" },
                    { "message", ex.Message }
                });
                throw;
            }
            finally
            {
                scope?.Dispose();
            }
        }
    }
}
