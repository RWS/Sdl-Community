using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface IHttpClient
	{
		HttpRequestHeaders DefaultRequestHeaders { get; }

		TimeSpan Timeout { get; set; }

		Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);

		Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, HttpCompletionOption httpCompletionOption);
	}
}