using System.Text;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Nuar.Formatters
{
    public class NetJsonInputFormatter : TextInputFormatter
    {
        public NetJsonInputFormatter()
        {
            SupportedMediaTypes.Add("application/json");
            SupportedEncodings.Add(Encoding.UTF8);
        }

        protected override bool CanReadType(Type type)
        {
            return type != null;
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var request = context.HttpContext.Request;
            using (var reader = new StreamReader(request.Body, encoding))
            {
                var body = await reader.ReadToEndAsync();
                var result = NetJSON.NetJSON.Deserialize(context.ModelType, body);
                return await InputFormatterResult.SuccessAsync(result);
            }
        }
    }
}