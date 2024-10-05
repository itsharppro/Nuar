using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetJSON;
using Nuar.Auth;
using Nuar.Configuration;
using Nuar.Extensions;
using Nuar.Handlers;
using Nuar.Options;
using Nuar.Requests;
using Nuar.Routing;
using Nuar.WebApi;
using Polly;
using Nuar.Formatters;

[assembly: InternalsVisibleTo("Nuar.Tests.Unit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace Nuar
{
    public static class NuarExtensions
    {
        private const string Logo = @"
   
 .-----------------. .----------------.  .----------------.  .----------------.   
| .--------------. || .--------------. || .--------------. || .--------------. |  
| | ____  _____  | || | _____  _____ | || |      __      | || |  _______     | |  
| ||_   \|_   _| | || ||_   _||_   _|| || |     /  \     | || | |_   __ \    | |  
| |  |   \ | |   | || |  | |    | |  | || |    / /\ \    | || |   | |__) |   | |  
| |  | |\ \| |   | || |  | '    ' |  | || |   / ____ \   | || |   |  __ /    | |  
| | _| |_\   |_  | || |   \ `--' /   | || | _/ /    \ \_ | || |  _| |  \ \_  | |  
| ||_____|\____| | || |    `.__.'    | || ||____|  |____|| || | |____| |___| | |  
| |              | || |              | || |              | || |              | |  
| '--------------' || '--------------' || '--------------' || '--------------' |  
 '----------------'  '----------------'  '----------------'  '----------------'   

                 _  _____       _                           
     /\         (_)/ ____|     | |                          
    /  \   _ __  _| |  __  __ _| |_ _____      ____ _ _   _ 
   / /\ \ | '_ \| | | |_ |/ _` | __/ _ \ \ /\ / / _` | | | |
  / ____ \| |_) | | |__| | (_| | ||  __/\ V  V / (_| | |_| |
 /_/    \_\ .__/|_|\_____|\__,_|\__\___| \_/\_/ \__,_|\__, |
          | |                                          __/ |
          |_|                                         |___/ 


                                
                                /####  Nuar API Gateway  ####\
                            @#~    *Innovative & Fast*    ~#@

";


        public static IServiceCollection AddNuar(this IServiceCollection services)
        {
            var (configuration, optionsProvider) = BuildConfiguration(services);

            return services.AddCoreServices()
                .ConfigureLogging(configuration)
                .ConfigureHttpClient(configuration)
                .ConfigurePayloads(configuration)
                .AddNuarServices()
                .AddExtensions(optionsProvider);
        }

        private static (NuarOptions, OptionsProvider) BuildConfiguration(IServiceCollection services)
        {
            IConfiguration config;
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                config = scope.ServiceProvider.GetService<IConfiguration>();
            }

            var optionsProvider = new OptionsProvider(config);
            services.AddSingleton<IOptionsProvider>(optionsProvider);
            var options = optionsProvider.Get<NuarOptions>();
            services.AddSingleton(options);

            return (options, optionsProvider);
        }

        private static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddMvcCore(options =>
            {
                options.InputFormatters.Clear();
                options.OutputFormatters.Clear();
                options.InputFormatters.Add(new NetJsonInputFormatter());
                options.OutputFormatters.Add(new NetJsonOutputFormatter());
            })
            .AddApiExplorer();

            return services;
        }

        private static IServiceCollection ConfigureLogging(this IServiceCollection services, NuarOptions options)
        {
            services.AddLogging(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole();
            });

            return services;
        }

        private static IServiceCollection ConfigureHttpClient(this IServiceCollection services, NuarOptions options)
        {
            var http = options.Http ?? new Http();
            var httpClientBuilder = services.AddHttpClient("nuar");

            httpClientBuilder.AddTransientHttpErrorPolicy(p =>
                p.WaitAndRetryAsync(http.Retries, retryAttempt =>
                {
                    var interval = http.Exponential
                        ? Math.Pow(http.Interval, retryAttempt)
                        : http.Interval;

                    return TimeSpan.FromSeconds(interval);
                }));

            return services;
        }

        private static IServiceCollection ConfigurePayloads(this IServiceCollection services, NuarOptions options)
        {
            if (options.PayloadsFolder is null)
            {
                options.PayloadsFolder = "Payloads";
            }

            if (options.PayloadsFolder.EndsWith("/"))
            {
                options.PayloadsFolder = options.PayloadsFolder
                    .Substring(0, options.PayloadsFolder.Length - 1);
            }
            
            return services;
        }

        private static IServiceCollection AddNuarServices(this IServiceCollection services)
        {
            services.AddSingleton<IAuthenticationManager, AuthenticationManager>();
            services.AddSingleton<IAuthorizationManager, AuthorizationManager>();
            services.AddSingleton<IPolicyManager, PolicyManager>();
            services.AddSingleton<IDownstreamBuilder, DownstreamBuilder>();
            services.AddSingleton<IPayloadBuilder, PayloadBuilder>();
            services.AddSingleton<IPayloadManager, PayloadManager>();
            services.AddSingleton<IPayloadTransformer, PayloadTransformer>();
            services.AddSingleton<IPayloadValidator, PayloadValidator>();
            services.AddSingleton<IRequestExecutionValidator, RequestExecutionValidator>();
            services.AddSingleton<IRequestHandlerManager, RequestHandlerManager>();
            services.AddSingleton<IRequestProcessor, RequestProcessor>();
            services.AddSingleton<IRouteConfigurator, RouteConfigurator>();
            services.AddSingleton<IRouteProvider, RouteProvider>();
            services.AddSingleton<ISchemaValidator, SchemaValidator>();
            services.AddSingleton<IUpstreamBuilder, UpstreamBuilder>();
            services.AddSingleton<IValueProvider, ValueProvider>();
            services.AddSingleton<DownstreamHandler>();
            services.AddSingleton<ReturnValueHandler>();
            services.AddSingleton<WebApiEndpointDefinitions>();

            return services;
        }

        private static IServiceCollection AddExtensions(this IServiceCollection services, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.Get<NuarOptions>();
            var extensionProvider = new ExtensionProvider(options);
            services.AddSingleton<IExtensionProvider>(extensionProvider);

            foreach (var extension in extensionProvider.GetAll())
            {
                 if (extension.Options.Enabled == false)
                {
                    continue;
                }

                extension.Extension.Add(services, optionsProvider);
            }

            return services;
        }

        public static IApplicationBuilder UseNuar(this IApplicationBuilder app)
        {
            var newLine = Environment.NewLine;
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Nuar>>();
            logger.LogInformation($"{newLine}{Logo}{newLine}");
            var options = app.ApplicationServices.GetRequiredService<NuarOptions>();

            if (options.Auth?.Enabled == true)
            {
                logger.LogInformation("Authentication is enabled.");
                app.UseAuthentication();
            }
            else
            {
                logger.LogInformation("Authentication is disabled.");
            }

            if (options.UseForwardedHeaders)
            {
                logger.LogInformation("Headers forwarding is enabled.");
                app.UseForwardedHeaders(new ForwardedHeadersOptions
                {
                    ForwardedHeaders = ForwardedHeaders.All
                });
            }

            app.UseExtensions();
            app.RegisterRequestHandlers();
            app.AddRoutes();

            return app;
        }

        private static void RegisterRequestHandlers(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Nuar>>();
            var options = app.ApplicationServices.GetRequiredService<NuarOptions>();
            var requestHandlerManager = app.ApplicationServices.GetRequiredService<IRequestHandlerManager>();

            requestHandlerManager.AddHandler("downstream",
                app.ApplicationServices.GetRequiredService<DownstreamHandler>());
            requestHandlerManager.AddHandler("return_value",
                app.ApplicationServices.GetRequiredService<ReturnValueHandler>());

            if (options.Modules == null)
            {
                return;
            }

            var handlers = options.Modules
                .SelectMany(m => m.Value.Routes)
                .Select(r => r.Use)
                .Distinct()
                .ToArray();

            foreach (var handler in handlers)
            {
                 if (requestHandlerManager.GetHandler(handler) == null)
                {
                    throw new Exception($"Handler: '{handler}' was not defined.");
                }

                logger.LogInformation($"Added handler: '{handler}'");
            }
        }

        private class Nuar
        {
        }

        private static void AddRoutes(this IApplicationBuilder app)
        {

            var options = app.ApplicationServices.GetRequiredService<NuarOptions>();
            if (options.Modules is null)
            {
                return;
            }
            
            foreach (var route in options.Modules.SelectMany(m => m.Value.Routes))
            {
                if (route.Methods is {})
                {
                    if (route.Methods.Any(m => m.Equals(route.Method, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        throw new ArgumentException($"There's already a method {route.Method.ToUpperInvariant()} declared in route 'methods', as well as in 'method'.");
                    }
                    
                    continue;
                }

                route.Method = (string.IsNullOrWhiteSpace(route.Method) ? "get" : route.Method).ToLowerInvariant();
                route.DownstreamMethod =
                    (string.IsNullOrWhiteSpace(route.DownstreamMethod) ? route.Method : route.DownstreamMethod)
                    .ToLowerInvariant();
            }

            var routeProvider = app.ApplicationServices.GetRequiredService<IRouteProvider>();
            app.UseRouting();
            app.UseEndpoints(routeProvider.Build());
        }

        private static void UseExtensions(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Nuar>>();
            var optionsProvider = app.ApplicationServices.GetRequiredService<IOptionsProvider>();
            var extensionProvider = app.ApplicationServices.GetRequiredService<IExtensionProvider>();

            foreach (var extension in extensionProvider.GetAll())
            {
                 if (extension.Options.Enabled == false)
                {
                    continue;
                }

                extension.Extension.Use(app, optionsProvider);
                logger.LogInformation($"Enabled extension: '{extension.Extension.Name}'");
            }
        }

        public static IApplicationBuilder UseRequestHandler<T>(this IApplicationBuilder app, string name)
            where T : IHandler
        {
            var requestHandlerManager = app.ApplicationServices.GetRequiredService<IRequestHandlerManager>();
            var handler = app.ApplicationServices.GetRequiredService<T>();
            requestHandlerManager.AddHandler(name, handler);

            return app;
        }
    }
}
