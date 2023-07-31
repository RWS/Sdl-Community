using System;
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
			return useUserCredentials ? await AuthenticateUsingCretentials(cloudCredentials) : await AuthenticateUsingSecrets(cloudCredentials);
		}

		private async static Task<string> AuthenticateUsingCretentials(CloudCredentials cloudCredentials)
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://api.languageweaver.com/v4/token/user");
			var content = new StringContent($"\r\n{{\r\n    \"username\": \"{cloudCredentials.UserID}\",\r\n    \"password\": \"{cloudCredentials.UserPassword}\"\r\n}}", null, ContentType);
			request.Content = content;
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		private async static Task<string> AuthenticateUsingSecrets(CloudCredentials cloudCredentials)
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://api.languageweaver.com/v4/token");
			var content = new StringContent("\r\n{\r\n    \"clientId\": \"\",\r\n    \"clientSecret\": \"\"\r\n}", null, "application/json");
			request.Content = content;
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}

		private static string GetRequestMessage()
		{
			return null;
		}
	}
}