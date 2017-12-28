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
		public  static async Task<UserResponse> Login(string userName, string password)
		{
			var jsonHelper = new JsonSerializerHelper();
			using (var client = new HttpClient())
			{
				//client.BaseAddress = new Uri(ApiUrls.BaseUri);
				client.DefaultRequestHeaders
					.Accept
					.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var user = new UserRequest
				{
					Email = userName,
					Password = password
				};

				var request = new HttpRequestMessage(HttpMethod.Post, ApiUrls.Login());
				var content = JsonConvert.SerializeObject(user,jsonHelper.SerializerSettings());
				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

				var response = await client.SendAsync(request);
				var userDetails = await response.Content.ReadAsStringAsync();
				var userResponse = jsonHelper.Deserialize<UserResponse>(userDetails);

				return userResponse;
			}
		}

		public static async Task Feedback(string sid)
		{
			var jsonHelper = new JsonSerializerHelper();
			using (var client = new HttpClient())
			{
				var feedback = new FeedbackRequest
				{
					LanguagePair = new LanguagePair
					{
						Source = "en-US",
						Target = "de-DE"
					},
					OriginalOutput = "Dies ist A Test.",
					PostEdited = "Das ist ein Katzensprung.",
					Definition =new Definition
					{
						Resources = new List<Resource>
						{
							new Resource
							{
								ResourceId = "5a3b9b630cf26707d2cf1863",
								Type = "MT"
							}
						}
					},
					Source = "This is a test."
				};

			var authHeader = $"SID={sid}";
				var request = new HttpRequestMessage(HttpMethod.Post, ApiUrls.Feedback());
				request.Headers.Authorization = new AuthenticationHeaderValue("LC", authHeader);
				var trackingHeader = string.Format("applicationKey={0} applicationInstance={1}", "n3JUV2wjQeKOVB0muXqL0Q%3D%3D", "55486a6a-cbfa-43de-b632-71c3b2e0da44.14.1");
				request.Headers.Add("Tracking", trackingHeader);
				var content = JsonConvert.SerializeObject(feedback, jsonHelper.SerializerSettings());

				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

				var response = await client.SendAsync(request);
				var userDetails = await response.Content.ReadAsStringAsync();
				
			}
		}
	}
}
