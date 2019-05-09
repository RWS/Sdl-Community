using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;

namespace Sdl.Community.GSVersionFetch.Service
{
	public class ProjectService
	{
		public async Task<ProjectResponse> GetGsProjects()
		{
			using (var httpClient = new HttpClient())
			{
				var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetProjects()));
				ApiUrl.AddRequestHeaders(httpClient, request);

				var responseMessage = await httpClient.SendAsync(request);
				var projectsResponse = await responseMessage.Content.ReadAsStringAsync();
				if (responseMessage.StatusCode == HttpStatusCode.OK)
				{
					return JsonConvert.DeserializeObject<ProjectResponse>(projectsResponse);
				}

				// if the respsonse is anything other than 200 -> OK, then throw and exception with the response message
				throw new Exception(projectsResponse);
			}
		}
	}
}
