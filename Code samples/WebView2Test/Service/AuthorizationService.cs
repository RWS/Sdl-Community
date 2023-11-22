using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebView2Test.Helpers;
using WebView2Test.Model;

namespace WebView2Test.Service
{
	public class AuthorizationService : IDisposable
	{
		private const string GetTokenEndpoint = "/oauth/token";
		private const string RevokeTokenEndpoint = "/oauth/revoke";
		private const string UrlTemplateNormal = "{0}?audience={1}&response_type=code&scope=openid%20email%20profile%20offline_access&redirect_uri={2}&client_id={3}&allowsignup=false&state={4}&code_challenge={5}&code_challenge_method={6}";
		private const string UserInfoEndpoint = "/userinfo";
		private readonly string _authorizationUrl = "/authorize";
		private readonly string _codeChallenge;
		private readonly string _codeChallengeMethod = "S256";
		private readonly CredentialRepository _credentialRepository;
		private readonly AuthorizationSettings _authorizationSettings;
		private readonly string _redirectUrl = "https://www.rws.com";
		private readonly string _state;
		private Credential _credentials;
		private Timer _refreshAccessTokenTimer;

		public AuthorizationService(LoginGeneratorsHelper loginGeneratorsHelper, CredentialRepository credentialRepository, AuthorizationSettings authorizationSettings)
		{
			_credentialRepository = credentialRepository;
			_authorizationSettings = authorizationSettings;
			_state = loginGeneratorsHelper.RandomDataBase64url(32);
			CodeVerifier = loginGeneratorsHelper.RandomDataBase64url(32);
			_codeChallenge = loginGeneratorsHelper.Base64urlencodeNoPadding(loginGeneratorsHelper.Sha256(CodeVerifier));
		}

		private enum Operation
		{
			Login,
			Logout,
			Refresh
		}

		private string CodeVerifier { get; }

		public Credential Credentials
		{
			get => _credentials ??= LoadCredentials();
			set => _credentials = value;
		}

		public void Dispose()
		{
			_refreshAccessTokenTimer?.Dispose();
		}

		public string GenerateAuthorizationRequest()
		{
			return GenerateRequestUrl(UrlTemplateNormal);
		}

		public string GenerateLogoutUrl()
		{
			return $"{_authorizationSettings.Auth0Url}/v2/logout?client_id={_authorizationSettings.ClientId}&returnTo={_redirectUrl}&federated";
		}

		public async Task<bool> EnsureLoggedIn()
		{
			if (Credentials is null) return false;

			var response = await HttpHelper.SendAsync(HttpMethod.Get, new Uri($"{_authorizationSettings.Auth0Url}{UserInfoEndpoint}"), null,
				Credentials.Token);

			if (response.IsSuccessStatusCode)
			{
				return true;
			}

			return await RefreshAccessToken();
		}

		public async Task<bool> Login(string query)
		{
			var parameters = HttpDecoder.ParseQuery(query);
			GetParameters(Operation.Login, parameters);

			var endpoint = new Uri($"{_authorizationSettings.Auth0Url}{GetTokenEndpoint}");
			var response = await HttpHelper.SendAsync(HttpMethod.Post, endpoint, parameters);

			if (!response.IsSuccessStatusCode) return false;

			SetCredential(await response.Content.ReadAsStringAsync());

			return true;
		}

		public async Task Logout()
		{
			var endpoint = new Uri($"{_authorizationSettings.Auth0Url}{RevokeTokenEndpoint}");
			await HttpHelper.SendAsync(HttpMethod.Post, endpoint,
				GetParameters(Operation.Logout));

			ClearCredentials();
		}

		private void ClearCredentials()
		{
			Credentials = null;
			_credentialRepository.ClearCredentials();
		}

		private string GenerateRequestUrl(string urlTemplate)
		{
			return string.Format(urlTemplate,
				_authorizationSettings.Auth0Url + _authorizationUrl,
				Uri.EscapeDataString(_authorizationSettings.Audience),
				Uri.EscapeDataString(_redirectUrl),
				_authorizationSettings.ClientId,
				_state,
				_codeChallenge,
				_codeChallengeMethod);
		}

		private Dictionary<string, string> GetParameters(Operation operation, Dictionary<string, string> parameters = null)
		{
			parameters ??= new Dictionary<string, string>();

			parameters.Add("client_id", _authorizationSettings.ClientId);

			switch (operation)
			{
				case Operation.Login:
					parameters.Add("redirect_uri", _redirectUrl);
					parameters.Add("code_verifier", CodeVerifier);
					parameters.Add("grant_type", "authorization_code");
					break;

				case Operation.Logout:
					parameters.Add("token", Credentials.RefreshToken);
					break;

				case Operation.Refresh:
					parameters.Add("grant_type", "refresh_token");
					parameters.Add("refresh_token", Credentials.RefreshToken);
					break;
			}

			return parameters;
		}

		private Credential LoadCredentials()
		{
			return _credentialRepository.LoadCredentials();
		}

		private async Task<bool> RefreshAccessToken()
		{
			var parameters = GetParameters(Operation.Refresh);

			var endpoint = $"{_authorizationSettings.Auth0Url}{GetTokenEndpoint}";
			var response = await HttpHelper.SendAsync(HttpMethod.Post, new Uri(endpoint), parameters);

			if (response.IsSuccessStatusCode)
			{
				SetCredential(await response.Content.ReadAsStringAsync());
				return true;
			}

			ClearCredentials();
			return false;
		}

		private void SetCredential(string response)
		{
			_refreshAccessTokenTimer?.Dispose();

			var responseJson = HttpDecoder.ParseJson(response);

			Credentials = new Credential(responseJson["access_token"].ToString(),
				responseJson["refresh_token"].ToString());

			SetRefreshAccessTokenTimer();

			_credentialRepository.SaveCredentials(Credentials);
		}

		private void SetRefreshAccessTokenTimer()
		{
			var handler = new JwtSecurityTokenHandler();
			var accessTokenValidTo = handler.ReadJwtToken(Credentials.Token).ValidTo;

			var timeUntilExpiry = accessTokenValidTo.Subtract(DateTime.Now).Subtract(new TimeSpan(0, 5, 0));
			_refreshAccessTokenTimer = new Timer(async _ => await RefreshAccessToken(), null, timeUntilExpiry,
				Timeout.InfiniteTimeSpan);
		}
	}
}