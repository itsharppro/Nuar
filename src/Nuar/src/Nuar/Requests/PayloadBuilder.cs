using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NetJSON;

namespace Nuar.Requests
{
    internal sealed class PayloadBuilder : IPayloadBuilder
    {
        private readonly ILogger<PayloadBuilder> _logger;

        public PayloadBuilder(ILogger<PayloadBuilder> logger)
        {
            _logger = logger;

            // Configure NetJSON options globally
            NetJSON.NetJSON.DateFormat = NetJSON.NetJSONDateFormat.ISO;
            NetJSON.NetJSON.SkipDefaultValue = false;
            NetJSON.NetJSON.TimeZoneFormat = NetJSON.NetJSONTimeZoneFormat.Utc;
        }

        public async Task<string> BuildRawAsync(HttpRequest request)
        {
            if (request.Body == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(request.Body))
            {
                var content = await reader.ReadToEndAsync();

                // Log the incoming payload
                _logger.LogInformation("Incoming Payload: {Payload}", content);

                return content;
            }
        }

        public async Task<T> BuildJsonAsync<T>(HttpRequest request) where T : class, new()
        {
            var payload = await BuildRawAsync(request);
            
            if (string.IsNullOrWhiteSpace(payload))
            {
                return new T();
            }

            // Deserialize payload and log it
            var deserializedPayload = NetJSON.NetJSON.Deserialize<T>(payload);
            _logger.LogInformation("Deserialized Payload: {Payload}", NetJSON.NetJSON.Serialize(deserializedPayload));
            
            return deserializedPayload;
        }
    }
}
