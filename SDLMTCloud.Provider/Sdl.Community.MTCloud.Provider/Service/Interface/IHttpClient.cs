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
		Task<string> GetResponseAsString(HttpResponseMessage responseMessage, [CallerMemberName] string callerMemberName = null);
		Task<T> GetResult<T>(HttpResponseMessage responseMessage, [CallerMemberName] string callerMemberName = null);
		Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, [CallerMemberName] string callerMemberName = null);
		void SetLogger(ILogger logger);
	}
}