using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NLog;

namespace Sdl.Community.MTCloud.Provider.Service.Interface
{
	public interface IHttpClient
	{
		HttpRequestHeaders DefaultRequestHeaders { get; }

		TimeSpan Timeout { get; set; }

		Task<string> GetResponseAsString(HttpResponseMessage responseMessage, [CallerMemberName] string callerMemberName = null);
		Task<T> GetResult<T>(HttpResponseMessage responseMessage, [CallerMemberName] string callerMemberName = null);
		Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);

		Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, HttpCompletionOption httpCompletionOption);
		Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, [CallerMemberName] string callerMemberName = null);
		void SetLogger(ILogger logger);
	}
}