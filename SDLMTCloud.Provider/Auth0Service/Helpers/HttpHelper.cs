using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Auth0Service.Helpers
{
	public static class HttpHelper
	{
		public static HttpResponseMessage Send(HttpMethod method, Uri uri, Dictionary<string, string> parameters = null, string token = null)
		{
			using var httpRequest = new HttpRequestMessage
			{
				Method = method,
				RequestUri = uri
			};

			if (parameters is not null) httpRequest.Content = new FormUrlEncodedContent(parameters);

			using var httpClient = new HttpClient();

			if (token is not null)
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			return httpClient.SendAsync(httpRequest).Result;
		}
	}
}