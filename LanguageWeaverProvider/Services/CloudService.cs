using System.Net.Http;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.NewFolder
{
	public static class CloudService
	{
		private const string UserCredentialsEndpoint = "https://api.languageweaver.com/v4/token/user";
		private const string ClientSecretsEndpoint = "https://api.languageweaver.com/v4/token";
		private const string ContentType = "application/json";

		public async static Task<string> AuthenticateUser(CloudCredentials cloudCredentials, bool useUserCredentials)
		{
			var client = new HttpClient();
			var endpoint = useUserCredentials ? UserCredentialsEndpoint : ClientSecretsEndpoint;
			var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
			var content = GetAuthenticationContent(cloudCredentials, useUserCredentials);
			var stringContent = new StringContent(content, null, ContentType);
			request.Content = stringContent;
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		private static string GetAuthenticationContent(CloudCredentials cloudCredentials, bool useUserCredentials)
		{
			var contentString = "\r\n{{\r\n    \"{0}\": \"{1}\",\r\n    \"{2}\": \"{3}\"\r\n}}";
			var output = string.Format(
				contentString,
				useUserCredentials ? "username" : "clientId",
				useUserCredentials ? cloudCredentials.UserID : cloudCredentials.ClientID,
				useUserCredentials ? "password" : "clientSecret",
				useUserCredentials ? cloudCredentials.UserPassword : cloudCredentials.ClientSecret);
			return output;
		}
	}
}