using Duende.IdentityServer.Models;

namespace App.Infrastructure.Identity.IdentityServerConfig;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
        ];

    public static IEnumerable<ApiScope> ApiScopes =>
        [
                new ApiScope("api.read", "Read Access to API"),
                new ApiScope("api.write", "Write Access to API")
        ];

    public static IEnumerable<Client> Clients =>
        [
                new Client
                {
                    ClientId = "postman_client",
                    ClientName = "Postman Client for OAuth2 Demo",
                    ClientSecrets = { new Secret("supersecret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://www.getpostman.com/oauth2-callback" },
                    AllowedCorsOrigins = { "https://www.getpostman.com" },

                    AllowedScopes = {
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.OpenId,
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.Profile,
                        Duende.IdentityServer.IdentityServerConstants.StandardScopes.Email,
                        "api.read",
                        "api.write"
                    },
                    AllowOfflineAccess = true
                }
        ];
}
