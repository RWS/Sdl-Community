using System.Net.Http;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.NewFolder
{
	public static class CloudService
	{
		public async static Task<string> Authenticate(CloudCredentials cloudCredentials)
		{
			var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://api.languageweaver.com/v4/token/user");
			var content = new StringContent($"\r\n{{\r\n    \"username\": \"{cloudCredentials.UserID}\",\r\n    \"password\": \"{cloudCredentials.UserPassword}\"\r\n}}", null, "application/json");
			request.Content = content;
			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			return await response.Content.ReadAsStringAsync();
		}
	}
}