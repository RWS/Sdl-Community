using System.Net.Http;
using System.Threading.Tasks;
using LanguageWeaverProvider.Model;

namespace LanguageWeaverProvider.Services
{
	internal class Service
    {
		public static async Task<HttpResponseMessage> SendRequest(AccessToken accessToken, HttpMethod httpMethod, string requestUri, HttpContent content = null)
		{
			var request = new HttpRequestMessage(httpMethod, requestUri);
			request.Headers.Add("Authorization", $"{accessToken.TokenType} {accessToken.Token}");

			if (content is not null)
			{
				request.Content = content;
			}

			var response = await new HttpClient().SendAsync(request);
			return response;
		}
	}
}