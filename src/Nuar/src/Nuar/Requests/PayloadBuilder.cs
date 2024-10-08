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
            NetJSON.NetJSON.DateFormat = NetJSON.NetJSONDateFormat.ISO;
            NetJSON.NetJSON.SkipDefaultValue = false;
            NetJSON.NetJSON.TimeZoneFormat = NetJSON.NetJSONTimeZoneFormat.Utc;
            NetJSON.NetJSON.UseEnumString = true; 
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

            try
            {
                var deserializedPayload = NetJSON.NetJSON.Deserialize<T>(payload);
                return deserializedPayload;
            }
            catch (NetJSON.NetJSONInvalidJSONException ex)
            {
                throw; 
            }
        }
    }
}
