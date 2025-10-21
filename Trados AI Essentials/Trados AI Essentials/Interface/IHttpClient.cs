using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Trados_AI_Essentials.Interface
{
    public interface IHttpClient
    {
        HttpRequestHeaders DefaultHeaders { get; }

        Task<string> SendAsync(HttpMethod request, object translationRequest, string uri);
    }
}