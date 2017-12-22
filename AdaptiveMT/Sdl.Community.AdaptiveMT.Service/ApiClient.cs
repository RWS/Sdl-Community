using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Sdl.Community.AdaptiveMT.Service.Model;

namespace Sdl.Community.AdaptiveMT.Service
{
	public static class ApiClient
	{
		public  static async Task<string> Login(string userName, string password)
		{
			var serializerSettings = new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			};
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri(Constants.BaseUri);
				client.DefaultRequestHeaders
					.Accept
					.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var user = new User
				{
					Email = userName,
					Password = password
				};

				var request = new HttpRequestMessage(HttpMethod.Post, Constants.Login);
				var content = JsonConvert.SerializeObject(user,serializerSettings);

				request.Content = new StringContent(content, new UTF8Encoding(), "application/json");

				var response = await client.SendAsync(request);

				var userDetails = await response.Content.ReadAsStringAsync();

				return userDetails;
			}
		}
	}
}
