using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Sdl.Community.MTCloud.Provider.Service.Interface;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class HttpClient : IHttpClient
	{
		private readonly System.Net.Http.HttpClient _httpClient;

		public HttpClient()
		{
			_httpClient = new System.Net.Http.HttpClient();
		}

		public HttpRequestHeaders DefaultRequestHeaders { get => _httpClient.DefaultRequestHeaders; }

		public TimeSpan Timeout
		{
			get => _httpClient.Timeout;
			set => _httpClient.Timeout = value;
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
		{
			return _httpClient.SendAsync(httpRequestMessage);
		}

		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, HttpCompletionOption httpCompletionOption)
		{
			return _httpClient.SendAsync(httpRequestMessage, httpCompletionOption);
		}
	}
}