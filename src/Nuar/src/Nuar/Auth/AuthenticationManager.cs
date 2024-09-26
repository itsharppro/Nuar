using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Nuar.Options;

namespace Nuar.Auth
{
    internal sealed class AuthenticationManager : IAuthenticationManager
    {
        private readonly NuarOptions _options;

        public AuthenticationManager(NuarOptions options)
        {
            _options = options;
        }

        public async Task<bool> TryAuthenticateAsync(HttpRequest request, RouteConfig routeConfig)
        {
            if (_options.Auth == null || !_options.Auth.Enabled || 
               (_options.Auth?.Global != true && routeConfig.Route?.Auth != true))
            {
                return true;
            }

            var result = await request.HttpContext.AuthenticateAsync();
            return result.Succeeded;
        }
    }
}
