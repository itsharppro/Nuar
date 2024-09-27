using System.Collections.Generic;

namespace Nuar.RabbitMq
{
    public class RabbitMqOptions : IOptions
    {
        public string ConnectionName { get; set; }
        public IEnumerable<string> Hostnames { get; set; }
        public int Port { get; set; } = 5672; 
        public string VirtualHost { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public int RequestedConnectionTimeout { get; set; } = 30000;
        public int SocketReadTimeout { get; set; } = 30000;
        public int SocketWriteTimeout { get; set; } = 30000;
        public ushort RequestedChannelMax { get; set; } = 0;
        public uint RequestedFrameMax { get; set; } = 0;
        public ushort RequestedHeartbeat { get; set; } = 60; 
        public bool UseBackgroundThreadsForIO { get; set; } = false;

        public ExchangeOptions Exchange { get; set; } = new ExchangeOptions();
        
        public SslOptions Ssl { get; set; } = new SslOptions();

        public MessageContextOptions MessageContext { get; set; } = new MessageContextOptions();

        public LoggerOptions Logger { get; set; } = new LoggerOptions();

        public IDictionary<string, object> Headers { get; set; } = new Dictionary<string, object>();

        public string SpanContextHeader { get; set; }

        public class SslOptions
        {
            public bool Enabled { get; set; } = false;
            public string ServerName { get; set; }
            public string CertificatePath { get; set; }
        }

        public class ExchangeOptions
        {
            public bool DeclareExchange { get; set; } = true;
            public bool Durable { get; set; } = true;
            public bool AutoDelete { get; set; } = false;
            public string Type { get; set; } = "direct"; 
        }

        public class MessageContextOptions
        {
            public bool Enabled { get; set; } = false;
            public string Header { get; set; }
        }

        public class LoggerOptions
        {
            public bool Enabled { get; set; } = true;
            public string Level { get; set; } = "Info";
        }
    }
}
