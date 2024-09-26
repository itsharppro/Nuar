using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Nuar.Requests
{
    internal sealed class ValueProvider : IValueProvider
    {
        private static readonly string[] AvailableTokens = { "user_id" };

        public IEnumerable<string> Tokens => AvailableTokens;

        public string GetValue(string value, HttpRequest request, RouteData data)
        {
            switch ($"{value?.ToLowerInvariant()}")
            {
                case "@user_id": return request.HttpContext?.User?.Identity?.Name;
                default: return value;
            }
        }
    }
}
