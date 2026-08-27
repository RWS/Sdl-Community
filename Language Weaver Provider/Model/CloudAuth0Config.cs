using System;
using LanguageWeaverProvider.Extensions;

namespace LanguageWeaverProvider.Model
{
	public class CloudAuth0Config
	{
		private const string UrlTemplateNormal = "{5}?audience={0}&response_type=code&scope=openid%20email%20profile%20offline_access&redirect_uri={1}&client_id={2}&allowsignup=false&state={3}&code_challenge={4}&code_challenge_method=S256";

		public CloudAuth0Config(string connectionCode, string portalRegion)
		{
			var environment = CloudEnvironment.Current;
			RedirectUri = environment.Auth0RedirectUri;
			ClientId = environment.Auth0ClientId;
			State = LoginGeneratorsHelper.RandomDataBase64url(32);
			CodeVerifier = LoginGeneratorsHelper.RandomDataBase64url(32);
			var codeChallenge = LoginGeneratorsHelper.Base64urlencodeNoPadding(LoginGeneratorsHelper.Sha256(CodeVerifier));

			var uriString = string.Format(UrlTemplateNormal,
							   Uri.EscapeDataString(environment.Auth0Audience),
							   Uri.EscapeDataString(RedirectUri),
							   ClientId,
							   State,
							   codeChallenge,
							   environment.Auth0AuthorizeUrl);
			uriString += string.IsNullOrEmpty(connectionCode) ? string.Empty : $"&connection={connectionCode}";
			LoginUri = new Uri(uriString);
			PortalRegion = portalRegion;
		}

		public string PortalRegion { get; private set; }

		public string CodeVerifier { get; private set; }

		public string RedirectUri { get; private set; }

		public string ClientId { get; private set; }

		public string State { get; private set; }

		public Uri LoginUri { get; private set; }
	}
}