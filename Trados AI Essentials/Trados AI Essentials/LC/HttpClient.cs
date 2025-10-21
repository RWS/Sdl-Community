using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Trados_AI_Essentials.Interface;

namespace Trados_AI_Essentials.LC
{
    public class HttpClient : IHttpClient
    {
        public HttpRequestHeaders DefaultHeaders => Client.DefaultRequestHeaders;
        private System.Net.Http.HttpClient Client { get; } = new System.Net.Http.HttpClient();

        public async Task<string> SendAsync(HttpMethod method, object translationRequest, string uri)
        {
            var response = await Client.SendAsync(new HttpRequestMessage(method, new Uri(uri))
            {
                Content = translationRequest is null
                    ? null
                    : new StringContent(JsonConvert.SerializeObject(translationRequest), Encoding.UTF8,
                        "application/json")
            });

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}