using System.Collections.Generic;

namespace Nuar.Extensions.Jwt
{
    internal class JwtOptions : IOptions
    {
        // Specifies the key used to sign the JWT
        public string IssuerSigningKey { get; set; }

        // Specifies the URL of the authority that issued the token
        public string Authority { get; set; }

        // Specifies the expected audience of the token
        public string Audience { get; set; }

        // The challenge that will be used in WWW-Authenticate header
        public string Challenge { get; set; } = "Bearer";

        // Specifies the metadata address for fetching the signing keys, typically from the discovery endpoint
        public string MetadataAddress { get; set; }

        // Specifies if the token should be saved once received
        public bool SaveToken { get; set; } = true;

        // Indicates whether the signin token should be saved
        public bool SaveSigninToken { get; set; }

        // Specifies if the audience is required in the token
        public bool RequireAudience { get; set; } = true;

        // Indicates whether HTTPS metadata is required
        public bool RequireHttpsMetadata { get; set; } = true;

        // Specifies if the token must include an expiration time
        public bool RequireExpirationTime { get; set; } = true;

        // Indicates whether signed tokens are required
        public bool RequireSignedTokens { get; set; } = true;

        // Specifies the valid audience (used for validation)
        public string ValidAudience { get; set; }

        // Specifies a collection of valid audiences (for multiple audiences)
        public IEnumerable<string> ValidAudiences { get; set; }

        // Specifies the valid issuer (used for validation)
        public string ValidIssuer { get; set; }

        // Specifies a collection of valid issuers (for multiple issuers)
        public IEnumerable<string> ValidIssuers { get; set; }

        // Specifies whether to validate the actor claim
        public bool ValidateActor { get; set; }

        // Indicates whether the audience should be validated
        public bool ValidateAudience { get; set; } = true;

        // Indicates whether the issuer should be validated
        public bool ValidateIssuer { get; set; } = true;

        // Specifies if the token's lifetime should be validated
        public bool ValidateLifetime { get; set; } = true;

        // Specifies if token replay attacks should be validated
        public bool ValidateTokenReplay { get; set; }

        // Specifies if the issuer signing key should be validated
        public bool ValidateIssuerSigningKey { get; set; }

        // Indicates if the middleware should refresh the keys if the issuer key is not found
        public bool RefreshOnIssuerKeyNotFound { get; set; } = true;

        // Specifies whether detailed error information should be included in the response
        public bool IncludeErrorDetails { get; set; } = true;

        // Specifies the authentication type to be used (typically "Bearer")
        public string AuthenticationType { get; set; }

        // Specifies the claim type for the user's name
        public string NameClaimType { get; set; }

        // Specifies the claim type for the user's role
        public string RoleClaimType { get; set; }
    }
}
