using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetJSON;

namespace Nuar.Requests
{
    internal sealed class PayloadBuilder : IPayloadBuilder
    {
        public PayloadBuilder()
        {
            // NetJSON configuration
            NetJSON.NetJSON.DateFormat = NetJSON.NetJSONDateFormat.ISO;
            NetJSON.NetJSON.SkipDefaultValue = false;
            NetJSON.NetJSON.TimeZoneFormat = NetJSON.NetJSONTimeZoneFormat.Utc;
            NetJSON.NetJSON.UseEnumString = true; // Ensures enums are serialized as strings if used
        }

        public async Task<string> BuildRawAsync(HttpRequest request)
        {
            if (request.Body == null)
            {
                return string.Empty;
            }

            // Reading the raw body of the request
            using (var reader = new StreamReader(request.Body))
            {
                var content = await reader.ReadToEndAsync();
                Console.WriteLine($"Incoming Payload: {content}");
                return content;
            }
        }

        public async Task<T> BuildJsonAsync<T>(HttpRequest request) where T : class, new()
        {
            var payload = await BuildRawAsync(request);

            if (string.IsNullOrWhiteSpace(payload))
            {
                return new T(); // Return empty instance if the payload is empty
            }

            // Handling payload deserialization and catching any errors
            try
            {
                var deserializedPayload = NetJSON.NetJSON.Deserialize<T>(payload);
                Console.WriteLine($"Deserialized Payload: {NetJSON.NetJSON.Serialize(deserializedPayload)}");
                return deserializedPayload;
            }
            catch (NetJSON.NetJSONInvalidJSONException ex)
            {
                Console.WriteLine($"Error Deserializing Payload: {ex.Message}");
                throw; // Let the error propagate so you can handle it appropriately in the service
            }
        }
    }
}
