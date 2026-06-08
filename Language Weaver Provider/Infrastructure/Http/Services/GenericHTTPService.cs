using LanguageWeaverProvider.Infrastructure.Http.Interfaces;
using LanguageWeaverProvider.Infrastructure.Http.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LanguageWeaverProvider.Infrastructure.Http.Services
{
    public class GenericHTTPService<TRequest, TResponse> : IGenericHTTPService<TRequest, TResponse>
    {
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, string> _httpHeaders;
        private readonly HttpMethod _httpMethod;
        private readonly string _httpUri;

        public GenericHTTPService(
            HttpClient httpClient,
            Dictionary<string, string> httpHeaders,
            HttpMethod httpMethod,
            string httpUri)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpHeaders = httpHeaders ?? new Dictionary<string, string>();
            _httpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            _httpUri = httpUri ?? throw new ArgumentNullException(nameof(httpUri));
        }

        public async Task<IGenericHTTPResponse<TResponse>> SendRequest(TRequest request)
        {

            try
            {
                using var httpRequest = new HttpRequestMessage(_httpMethod, _httpUri);

                // Add headers
                foreach (var header in _httpHeaders)
                {
                    httpRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                // Serialize request body if it exists
                if (request != null)
                {
                    string json = JsonConvert.SerializeObject(request);
                    httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                // Send HTTP request
                using var response = await _httpClient.SendAsync(httpRequest);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new GenericHTTPRespose<TResponse>
                    {
                        Response = default,
                        Success = false,
                        Errors = [$"HTTP {(int)response.StatusCode}: {responseContent}"]
                    };
                }

                // Deserialize response
                var responseInstance = JsonConvert.DeserializeObject<TResponse>(
                responseContent
                );

                return new GenericHTTPRespose<TResponse>
                {
                    Response = responseInstance,
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                return new GenericHTTPRespose<TResponse>()
                {
                    Response = default,
                    Success = false,
                    Errors = [ex.Message]
                };
            }
        }
    }
}
