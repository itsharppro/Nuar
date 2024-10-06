using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Nuar.Formatters
{
    public class NetJsonOutputFormatter : TextOutputFormatter
    {
        public NetJsonOutputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanWriteType(Type type)
        {
            return type != null;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;

            // Set Content-Type to application/json
            if (!response.Headers.ContainsKey("Content-Type"))
            {
                response.Headers.Add("Content-Type", "application/json");
            }

            try
            {
                var json = NetJSON.NetJSON.Serialize(context.Object);
                await response.WriteAsync(json, selectedEncoding);
            }
            catch
            {
                // Handle serialization error by returning a failure response or rethrowing the exception
                throw new InvalidOperationException("Failed to serialize the response to JSON.");
            }
        }
    }
}
