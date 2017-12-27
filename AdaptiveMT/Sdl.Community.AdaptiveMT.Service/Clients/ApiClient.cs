using System;
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
				//var test = JsonConvert.DeserializeObject<UserResponse>(userDetails);
				return userResponse;
			}
		}
	}
}
