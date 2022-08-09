using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Threading;
using Auth0Service.Helpers;
using Auth0Service.Model;

namespace Auth0Service.Service
{
	public class AuthorizationService
	{
		private const string GetTokenEndpoint = "/oauth/token";
		private const string RevokeTokenEndpoint = "/oauth/revoke";
		private const string UrlTemplateNormal = "{0}?audience={1}&response_type=code&scope=openid%20email%20profile%20offline_access&redirect_uri={2}&client_id={3}&allowsignup=false&state={4}&code_challenge={5}&code_challenge_method={6}";
		private const string UserInfoEndpoint = "/userinfo";
		private readonly AuthorizationSettings _authorizationSettings;
		private readonly string _authorizationUrl = "/authorize";
		private readonly string _codeChallenge;
		private readonly string _codeChallengeMethod = "S256";
		private readonly HttpHelper _httpHelper;
		private readonly string _redirectUrl = "https://www.rws.com";
		private readonly string _state;
		private Timer _refreshAccessTokenTimer;

		public AuthorizationService(LoginGeneratorsHelper loginGeneratorsHelper, AuthorizationSettings authorizationSettings, HttpHelper httpHelper)
		{
			_authorizationSettings = authorizationSettings;
			_state = loginGeneratorsHelper.RandomDataBase64url(32);
			CodeVerifier = loginGeneratorsHelper.RandomDataBase64url(32);
			_codeChallenge = loginGeneratorsHelper.Base64urlencodeNoPadding(loginGeneratorsHelper.Sha256(CodeVerifier));
			_httpHelper = httpHelper;
		}

		private enum Operation
		{
			Login,
			Logout,
			Refresh
		}

		public Credential Credentials { get; set; }
		private string CodeVerifier { get; }

		public AuthenticationResult AreCredentialsValid()
		{
			if (Credentials is null) return new AuthenticationResult(AuthenticationResult.CredentialsValidity.NotPresent);
			var response = GetUserProfile(Credentials.Token);

			if (response.IsSuccessStatusCode)
			{
				return new AuthenticationResult(AuthenticationResult.CredentialsValidity.Valid);
			}

			return RefreshAccessToken();
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
			return $"{_authorizationSettings.Auth0Url}/v2/logout?federated&client_id={_authorizationSettings.ClientId}&returnTo={_redirectUrl}";
		}

		public AuthenticationResult Login(string query)
		{
			var parameters = HttpDecoder.ParseQuery(query);
			GetParameters(Operation.Login, parameters);

			var endpoint = new Uri($"{_authorizationSettings.Auth0Url}{GetTokenEndpoint}");
			var response = _httpHelper.Send(HttpMethod.Post, endpoint, parameters);

			if (!response.IsSuccessStatusCode) return new AuthenticationResult(AuthenticationResult.CredentialsValidity.Invalid);

			SetCredential(response.Content.ReadAsStringAsync().Result);

			return new AuthenticationResult(AuthenticationResult.CredentialsValidity.Valid);
		}

		public void Logout()
		{
			var logoutEndpoint = new Uri(GenerateLogoutUrl());
			_httpHelper.Send(HttpMethod.Get, logoutEndpoint);

			var revokeEndpoint = new Uri($"{_authorizationSettings.Auth0Url}{RevokeTokenEndpoint}");
			_httpHelper.Send(HttpMethod.Post, revokeEndpoint,
				GetParameters(Operation.Logout));

			ClearCredentials();
		}

		private void ClearCredentials()
		{
			Credentials = null;
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
					parameters.Add("token", Credentials?.RefreshToken);
					break;

				case Operation.Refresh:
					parameters.Add("grant_type", "refresh_token");
					parameters.Add("refresh_token", Credentials.RefreshToken);
					break;
			}

			return parameters;
		}

		private HttpResponseMessage GetUserProfile(string token)
		{
			return _httpHelper.Send(HttpMethod.Get, new Uri($"{_authorizationSettings.Auth0Url}{UserInfoEndpoint}"), null,
							token);
		}

		private AuthenticationResult RefreshAccessToken()
		{
			var parameters = GetParameters(Operation.Refresh);

			var endpoint = $"{_authorizationSettings.Auth0Url}{GetTokenEndpoint}";
			var response = _httpHelper.Send(HttpMethod.Post, new Uri(endpoint), parameters);

			if (response.IsSuccessStatusCode)
			{
				SetCredential(response.Content.ReadAsStringAsync().Result);
				return new AuthenticationResult(AuthenticationResult.CredentialsValidity.Valid);
			}

			ClearCredentials();
			return new AuthenticationResult(AuthenticationResult.CredentialsValidity.Invalid);
		}

		private void SetCredential(string response)
		{
			_refreshAccessTokenTimer?.Dispose();

			var responseJson = HttpDecoder.ParseJson(response);
			var accessToken = responseJson["access_token"].ToString();

			Credentials = new Credential(accessToken, responseJson["refresh_token"]?.ToString());

			SetRefreshAccessTokenTimer();
		}

		private void SetRefreshAccessTokenTimer()
		{
			var handler = new JwtSecurityTokenHandler();
			var accessTokenValidTo = handler.ReadJwtToken(Credentials.Token).ValidTo;

			var timeUntilExpiry = accessTokenValidTo.Subtract(DateTime.Now).Subtract(new TimeSpan(0, 5, 0));
			_refreshAccessTokenTimer = new Timer(_ => RefreshAccessToken(), null, timeUntilExpiry,
				Timeout.InfiniteTimeSpan);
		}
	}
}