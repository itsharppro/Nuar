using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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

            if (request.Body == null)
            {
                return await InputFormatterResult.NoValueAsync();
            }

            using (var reader = new StreamReader(request.Body, encoding))
            {
                try
                {
                    var body = await reader.ReadToEndAsync();
                    if (string.IsNullOrWhiteSpace(body))
                    {
                        return await InputFormatterResult.NoValueAsync();
                    }

                    var result = NetJSON.NetJSON.Deserialize(context.ModelType, body);
                    return await InputFormatterResult.SuccessAsync(result);
                }
                catch
                {
                    // Handle deserialization error by adding model state error and returning failure
                    context.ModelState.AddModelError("JSON", "Invalid JSON format.");
                    return await InputFormatterResult.FailureAsync();
                }
            }
        }
    }
}
