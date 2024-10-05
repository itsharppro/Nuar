using System.Text;
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

        public override Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var json = NetJSON.NetJSON.Serialize(context.Object);
            return response.WriteAsync(json);
        }
    }
}