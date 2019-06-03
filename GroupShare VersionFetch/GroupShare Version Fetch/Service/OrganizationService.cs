using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;

namespace Sdl.Community.GSVersionFetch.Service
{
	public class OrganizationService
	{
		public static readonly Log Log = Log.Instance;

		public async Task<List<OrganizationResponse>> GetOrganizations()
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetOrganizations()));
					ApiUrl.AddRequestHeaders(httpClient, request);

					var responseMessage = await httpClient.SendAsync(request);
					var organizationsResponse = await responseMessage.Content.ReadAsStringAsync();
					if (responseMessage.StatusCode == HttpStatusCode.OK)
					{
						return JsonConvert.DeserializeObject<List<OrganizationResponse>>(organizationsResponse);
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"GetOrganizations service method: {e.Message}\n {e.StackTrace}");
			}
			return new List<OrganizationResponse>();
		}
	}
}
