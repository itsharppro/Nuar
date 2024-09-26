using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Nuar.Configuration;
using Nuar.Options;

namespace Nuar.Routing
{
    internal sealed class UpstreamBuilder : IUpstreamBuilder
    {
        private readonly NuarOptions _options;
        private readonly IRequestHandlerManager _requestHandlerManager;
        private readonly ILogger<UpstreamBuilder> _logger;

        public UpstreamBuilder(NuarOptions options, IRequestHandlerManager requestHandlerManager,
            ILogger<UpstreamBuilder> logger)
        {
            _options = options;
            _requestHandlerManager = requestHandlerManager;
            _logger = logger;
        }

        public string Build(Module module, Route route)
        {
            var path = module.Path;
            var upstream = string.IsNullOrWhiteSpace(route.Upstream) ? string.Empty : route.Upstream;

            if (!string.IsNullOrWhiteSpace(path))
            {
                var modulePath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path;

                if (!upstream.StartsWith("/"))
                {
                    upstream = $"/{upstream}";
                }

                upstream = $"{modulePath}{upstream}";
            }

            if (upstream.EndsWith("/") && upstream != "/")
            {
                upstream = upstream.Substring(0, upstream.Length - 1);
            }

            if (route.MatchAll)
            {
                upstream = $"{upstream}/{{*url}}";
            }

            var handler = _requestHandlerManager.GetHandler(route.Use);
            var routeInfo = handler.GetInfo(route);

            var isPublicInfo = _options.Auth is null || !_options.Auth.Global && route.Auth is null || route.Auth == false
                ? "public"
                : "protected";

            var methods = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(route.Method))
            {
                methods.Add(route.Method.ToUpperInvariant());
            }

            if (route.Methods != null)
            {
                foreach (var method in route.Methods)
                {
                    if (!string.IsNullOrWhiteSpace(method))
                    {
                        methods.Add(method.ToUpperInvariant());
                    }
                }
            }

            _logger.LogInformation($"Added {isPublicInfo} route for upstream: [{string.Join(", ", methods)}] " +
                                   $"'{upstream}' -> {routeInfo}");

            return upstream;
        }
    }
}
