using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nuar.RabbitMq.Clients;
using Nuar.RabbitMq.Contexts;
using Nuar.RabbitMq.Handlers;
using Nuar.Options;
using RabbitMQ.Client;
using Nuar.RabbitMQ.Contexts;

namespace Nuar.RabbitMq
{
    public class RabbitMqExtension : IExtension
    {
        public string Name => "rabbitmq";
        public string Description => "RabbitMQ message broker";
        public string Version => "1.0.0";
        public void Add(IServiceCollection services, IOptionsProvider optionsProvider)
        {
            var options = optionsProvider.GetForExtension<RabbitMqOptions>(Name);
            services.AddSingleton(options);

            services.AddSingleton(sp =>
            {
                var connectionFactory = new ConnectionFactory
                {
                    HostName = options.Hostnames?.FirstOrDefault(),
                    Port = options.Port,
                    VirtualHost = options.VirtualHost,
                    UserName = options.Username,
                    Password = options.Password,
                    RequestedConnectionTimeout = options.RequestedConnectionTimeout,
                    SocketReadTimeout = options.SocketReadTimeout,
                    SocketWriteTimeout = options.SocketWriteTimeout,
                    RequestedChannelMax = options.RequestedChannelMax,
                    RequestedFrameMax = options.RequestedFrameMax,
                    RequestedHeartbeat = options.RequestedHeartbeat,
                    UseBackgroundThreadsForIO = options.UseBackgroundThreadsForIO,
                    Ssl = options.Ssl is null
                        ? new SslOption()
                        : new SslOption(options.Ssl.ServerName, options.Ssl.CertificatePath, options.Ssl.Enabled),
                };

                var connection = connectionFactory.CreateConnection(options.ConnectionName);

                if (options.Exchange?.DeclareExchange == true)
                {
                    var nuarOptions = optionsProvider.Get<NuarOptions>();
                    var exchanges = nuarOptions.Modules
                        .SelectMany(m => m.Value.Routes)
                        .Where(m => m.Use.Equals(Name, StringComparison.InvariantCultureIgnoreCase))
                        .SelectMany(r => r.Config)
                        .Where(c => c.Key.Equals("exchange", StringComparison.InvariantCultureIgnoreCase))
                        .Distinct()
                        .ToList();

                    if (exchanges.Any())
                    {
                        var logger = sp.GetService<ILogger<IConnection>>();
                        var loggerEnabled = options.Logger?.Enabled == true;

                        using (var channel = connection.CreateModel())
                        {
                            foreach (var exchange in exchanges)
                            {
                                var exchangeName = exchange.Value;
                                var exchangeType = options.Exchange.Type;
                                if (loggerEnabled)
                                {
                                    logger.LogInformation($"Declaring exchange: '{exchangeName}', type: '{exchangeType}'.");
                                }

                                channel.ExchangeDeclare(exchangeName, exchangeType, options.Exchange.Durable, options.Exchange.AutoDelete);
                            }
                        }
                    }
                }

                return connection;
            });

            services.AddTransient<IRabbitMqClient, RabbitMqClient>();
            services.AddTransient<RabbitMqHandler>();

            services.AddSingleton<IContextBuilder, NullContextBuilder>();
            services.AddSingleton<ISpanContextBuilder, NullSpanContextBuilder>();
        }

        public void Use(IApplicationBuilder app, IOptionsProvider optionsProvider)
        {
            app.UseRequestHandler<RabbitMqHandler>(Name);
        }
    }
}
