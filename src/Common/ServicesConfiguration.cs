using System;

namespace Common
{
	public class Identity
	{
		public string Url { get; set; } = string.Empty;
	}
	public class CoreDomainAPI
	{
		public string Url { get; set; } = string.Empty;
	}
	public class PublicWebApp
	{
		public const string ClientId = "publicwebapp";
		public string Url { get; set; } = string.Empty;
		public string RedirectURI { get; set; } = string.Empty;
		public string PostLogoutRedirectUris { get; set; } = string.Empty;
		public string Secret { get; set; } = string.Empty;
	}
	public class ServicesConfiguration
	{
		public Identity Identity { get; set; } = default!;
		public CoreDomainAPI CoreDomainAPI { get; set; } = default!;
		public PublicWebApp PublicWebApp { get; set; } = default!;
	}
}
