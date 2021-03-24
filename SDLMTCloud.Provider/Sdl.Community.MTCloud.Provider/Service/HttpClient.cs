using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Sdl.Community.MTCloud.Provider.Service.Interface;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class HttpClient : IHttpClient
	{
		private readonly System.Net.Http.HttpClient _httpClient;
		private ILogger _logger;

		public HttpClient()
		{
			_httpClient = new System.Net.Http.HttpClient();
		}

		public HttpRequestHeaders DefaultRequestHeaders { get => _httpClient.DefaultRequestHeaders; }

		public async Task<string> GetResponseAsString(HttpResponseMessage responseMessage, [CallerMemberName] string callerMemberName = null)
		{
			string response = null;

			if (responseMessage?.Content == null) return null;
			try
			{
				response = await responseMessage.Content.ReadAsStringAsync();
			}
			catch (Exception e)
			{
				_logger.Error($"{nameof(GetResponseAsString)} for {callerMemberName}: {e}");
			}

			return response;
		}

		public async Task<T> GetResult<T>(HttpResponseMessage responseMessage, [CallerMemberName] string callerMemberName = null)
		{
			if (responseMessage is null) return default;
			var response = await GetResponseAsString(responseMessage, callerMemberName);
			responseMessage.ReasonPhrase = GetErrorDescription(response);
			if (responseMessage.IsSuccessStatusCode)
			{
				T result = default;
				try
				{
					result = JsonConvert.DeserializeObject<T>(response);
				}
				catch (Exception e)
				{
					_logger.Error($"{nameof(GetResult)} for {callerMemberName}: {e}");
				}
				return result;
			}

			_logger.Error($"{nameof(GetResult)} for {callerMemberName} " + $"{responseMessage?.StatusCode}\n {responseMessage?.RequestMessage}");
			return default;
		}

		public async Task<HttpResponseMessage> SendRequest(HttpRequestMessage request, [CallerMemberName] string callerMemberName = null)
		{
			HttpResponseMessage responseMessage = null;
			try
			{
				responseMessage = await _httpClient.SendAsync(request);
			}
			catch (Exception e)
			{
				_logger.Error($"{nameof(SendRequest)} for {callerMemberName}" + e);
			}

			return responseMessage;
		}

		public void SetLogger(ILogger logger)
		{
			_logger = logger;
		}

		private string GetErrorDescription(string value)
		{
			var message = "";
			try
			{
				var jObj = JToken.Parse(value);
				message = jObj["status"]["description"].ToString();
			}
			catch
			{
				// ignored
			}
			return message;
		}
	}
}