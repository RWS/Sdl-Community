using System;
using LanguageWeaverProvider.Extensions;

namespace LanguageWeaverProvider.Model
{
	public class CloudAuth0Config
	{
		private const string UrlTemplateNormal = "https://sdl-preprod.eu.auth0.com/authorize?audience={0}&response_type=code&scope=openid%20email%20profile%20offline_access&redirect_uri={1}&client_id={2}&allowsignup=false&state={3}&code_challenge={4}&code_challenge_method=S256";
		private const string Audience = "https://api-preprod.sdl.com";

		public CloudAuth0Config(string connectionCode, string portalRegion)
		{
			RedirectUri = "https://www.rws.com";
			ClientId = "OltQlVmK6N9Y04bNMmFxoXmGleLMmdxB";
			State = LoginGeneratorsHelper.RandomDataBase64url(32);
			CodeVerifier = LoginGeneratorsHelper.RandomDataBase64url(32);
			var codeChallenge = LoginGeneratorsHelper.Base64urlencodeNoPadding(LoginGeneratorsHelper.Sha256(CodeVerifier));

			var uriString = string.Format(UrlTemplateNormal,
							   Uri.EscapeDataString(Audience),
							   Uri.EscapeDataString(RedirectUri),
							   ClientId,
							   State,
							   codeChallenge);
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