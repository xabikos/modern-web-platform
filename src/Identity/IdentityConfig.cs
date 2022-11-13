using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Identity
{
	public static class IdentityConfig
	{
		public static IEnumerable<IdentityResource> IdentityResources =>
		new IdentityResource[]
		{
			new IdentityResources.OpenId(),
			new IdentityResources.Profile(),

		};

		public static IEnumerable<ApiScope> ApiScopes =>
			new ApiScope[]
				{
					new ApiScope("core_domain_API", "The core domain API that holds all the business logic")
				};

		public static IEnumerable<Client> Clients =>
			new Client[]
				{
					new Client {
						ClientId = "publicwebapp",
						ClientSecrets = { new Secret("secret".Sha256()) },

						AllowedGrantTypes = GrantTypes.Code,
                
						// where to redirect to after login
						RedirectUris = { "https://localhost:7002/signin-oidc" },

						// where to redirect to after logout
						PostLogoutRedirectUris = { "https://localhost:7002/signout-callback-oidc" },
						
						AllowOfflineAccess= true,

						AllowedScopes = new List<string>
						{
							IdentityServerConstants.StandardScopes.OpenId,
							IdentityServerConstants.StandardScopes.Profile,
                            "core_domain_API"
                        }
					}
				};
	}
}
