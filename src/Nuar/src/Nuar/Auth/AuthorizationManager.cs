using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Nuar.Auth
{
    internal sealed class AuthorizationManager : IAuthorizationManager
    {
        private readonly IPolicyManager _policyManager;

        public AuthorizationManager(IPolicyManager policyManager)
        {
            _policyManager = policyManager;
        }

        public bool IsAuthorized(ClaimsPrincipal user, RouteConfig routeConfig)
        {
            if (user == null)
            {
                return false;
            }

            if (routeConfig.Route == null)
            {
                return true;
            }

            return HasPolicies(user, routeConfig.Route.Policies) && HasClaims(user, routeConfig.Route.Claims);
        }

        private bool HasPolicies(ClaimsPrincipal user, IEnumerable<string> policies)
            => policies == null || policies.All(p => HasPolicy(user, p));

        private bool HasPolicy(ClaimsPrincipal user, string policy)
            => HasClaims(user, _policyManager.GetClaims(policy));

        private static bool HasClaims(ClaimsPrincipal user, IDictionary<string, string> claims)
            => claims == null || claims.All(claim => user.HasClaim(claim.Key, claim.Value));
    }
}
