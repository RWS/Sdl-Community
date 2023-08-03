using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.NewFolder
{
	public static class CloudService
	{
		private static readonly HttpClient _httpClient = new();

		private const string UserCredentialsEndpoint = "https://api.languageweaver.com/v4/token/user";
		private const string ClientSecretsEndpoint = "https://api.languageweaver.com/v4/token";
		private const string MediaType = "application/json";

		public async static Task<(string Response, bool Success)> AuthenticateUser(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			var endpoint = authenticationType switch
			{
				AuthenticationType.Credentials => UserCredentialsEndpoint,
				AuthenticationType.Secret => ClientSecretsEndpoint,
				_ => throw new ArgumentException("Invalid authentication type")
			};

			var content = GetAuthenticationContent(cloudCredentials, authenticationType);
			var stringContent = new StringContent(content, null, MediaType);

			try
			{
				var response = await _httpClient.PostAsync(endpoint, stringContent);
				response.EnsureSuccessStatusCode();
				return (await response.Content.ReadAsStringAsync(), true);
			}
			catch
			{
				return (null, false);
			}
		}

		private static string GetAuthenticationContent(CloudCredentials cloudCredentials, AuthenticationType authenticationType)
		{
			var isCredentialsSelected = authenticationType == AuthenticationType.Credentials;
			var idKey = isCredentialsSelected ? "username" : "clientId";
			var idValue = isCredentialsSelected ? cloudCredentials.UserID : cloudCredentials.ClientID;
			var secretKey = isCredentialsSelected ? "password" : "clientSecret";
			var secretValue = isCredentialsSelected ? cloudCredentials.UserPassword : cloudCredentials.ClientSecret;

			return $@"
{{
    ""{idKey}"": ""{idValue}"",
    ""{secretKey}"": ""{secretValue}""
}}";
		}
	}
}