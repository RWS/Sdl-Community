using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sdl.Community.AdaptiveMT.Service.Helpers;
using Sdl.Community.AdaptiveMT.Service.Model;

namespace Sdl.Community.AdaptiveMT.Service.Clients
{
	public static class ApiClient
	{
		public static async Task<UserResponse> Login(string userName, string password)
		{
			var user = new UserRequest
			{
				Email = userName,
				Password = password
			};
			var response = await SendRequest(user, string.Empty, HttpMethod.Post, ApiUrls.Login());
			var userResponse = JsonConvert.DeserializeObject<UserResponse>(response);

			return userResponse;
		}

		public static async Task OosSession(UserRequest user,string sid)
		{
			var response = await SendRequest(null, sid, HttpMethod.Put, ApiUrls.Session(user.SelectedAccountId));
			JsonConvert.DeserializeObject<Session>(response);
		}
		public static async Task<FeedbackResponse> Feedback(string sid,FeedbackRequest feedback)
		{
			var response = await SendRequest(feedback, sid, HttpMethod.Post, ApiUrls.Feedback());
			var feedbackMessage = JsonConvert.DeserializeObject<FeedbackResponse>(response);
			return feedbackMessage;

		}
		public static async Task<string> SendRequest(object contentBody,string sid, HttpMethod method, Uri url)
		{
			var jsonHelper = new JsonSerializerHelper();
			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
				var traking = string.Format("applicationKey={0}", @"CZqvCdx8JnoKV7JiAuvGOQ%3D%3D");
				var request = new HttpRequestMessage(method, url);

				//for Login call we don't have sid
				if (sid != string.Empty)
				{
					var authHeader = $"SID={sid}";
					request.Headers.Authorization = new AuthenticationHeaderValue("LC", authHeader);
					
					request.Headers.Add("Tracking", traking);
				}
				if (contentBody != null)
				{
					var content = JsonConvert.SerializeObject(contentBody, jsonHelper.SerializerSettings());
					request.Content = new StringContent(content, new UTF8Encoding(), "application/json");
				}
				var responseMessage = await client.SendAsync(request);
				var response = await responseMessage.Content.ReadAsStringAsync();
				return response;
			}
		}


		public static async Task<TranslateResponse> Translate(string sid, TranslateRequest translateRequest)
		{
			var response = await SendRequest(translateRequest, sid, HttpMethod.Post, ApiUrls.Translate());
			var translateResponse = JsonConvert.DeserializeObject<TranslateResponse>(response);
			return translateResponse;
		}
	}
}
