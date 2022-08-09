using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Auth0Service.Helpers
{
	public class HttpHelper : IDisposable
	{
		private HttpClient _httpClient;

		public HttpHelper()
		{
			_httpClient = new HttpClient();
		}

		public void Dispose()
		{
			_httpClient?.Dispose();
		}

		public HttpResponseMessage Send(HttpMethod method, Uri uri, Dictionary<string, string> parameters = null, string token = null)
		{
			using var httpRequest = new HttpRequestMessage
			{
				Method = method,
				RequestUri = uri
			};

			if (parameters is not null) httpRequest.Content = new FormUrlEncodedContent(parameters);

			if (token is not null)
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

			return _httpClient.SendAsync(httpRequest).Result;
		}
	}
}