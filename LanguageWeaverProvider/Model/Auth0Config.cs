using System;
using LanguageWeaverProvider.Extensions;

namespace LanguageWeaverProvider.Model
{
	public class Auth0Config
	{
		private const string UrlTemplateNormal = "https://sdl-prod.eu.auth0.com/authorize?audience={0}&response_type=code&scope=openid%20email%20profile%20offline_access&redirect_uri={1}&client_id={2}&allowsignup=false&state={3}&code_challenge={4}&code_challenge_method=S256";
		private const string Audience = "https://api.sdl.com";

		public Auth0Config()
		{
			RedirectUri = "https://www.rws.com";
			ClientId = "F4NpOGG1sBaEzk379M6ZxX3gGa0iH1Ff";
			State = LoginGeneratorsHelper.RandomDataBase64url(32);
			CodeVerifier = LoginGeneratorsHelper.RandomDataBase64url(32);
			var codeChallenge = LoginGeneratorsHelper.Base64urlencodeNoPadding(LoginGeneratorsHelper.Sha256(CodeVerifier));

			LoginUri = new Uri(string.Format(UrlTemplateNormal,
							   Uri.EscapeDataString(Audience),
							   Uri.EscapeDataString(RedirectUri),
							   ClientId,
							   State,
							   codeChallenge));
		}

		public string CodeVerifier { get; private set; }

		public string RedirectUri { get; private set; }

		public string ClientId { get; private set; }

		public string State { get; private set; }

		public Uri LoginUri { get; private set; }
	}
}