using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;

namespace Sdl.Community.GSVersionFetch.Service
{
	public class ProjectService
	{
		public static readonly Log Log = Log.Instance;

		public async Task<ProjectResponse> GetGsProjects(ProjectFilter projectFilter)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var baseUrl = ApiUrl.GetProjects();
					var builder = new UriBuilder(baseUrl);
					var query = HttpUtility.ParseQueryString(builder.Query);
					query["page"] = projectFilter.Page.ToString();
					query["start"] = "0";
					query["limit"] = projectFilter.PageSize.ToString();
					builder.Query = query.ToString();
					var request = new HttpRequestMessage(HttpMethod.Get, new Uri(builder.ToString()));
					ApiUrl.AddRequestHeaders(httpClient, request);

					var responseMessage = await httpClient.SendAsync(request);
					var projectsResponse = await responseMessage.Content.ReadAsStringAsync();
					if (responseMessage.StatusCode == HttpStatusCode.OK)
					{
						return JsonConvert.DeserializeObject<ProjectResponse>(projectsResponse);
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"GetGsProjects service method: {e.Message}\n {e.StackTrace}");
			}
			return new ProjectResponse();
		}

		public async Task<List<GsFile>> GetProjectFiles(string projectId)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetProjectFiles(projectId)));
					ApiUrl.AddRequestHeaders(httpClient, request);

					var responseMessage = await httpClient.SendAsync(request);
					var filesResponse = await responseMessage.Content.ReadAsStringAsync();
					if (responseMessage.StatusCode == HttpStatusCode.OK)
					{
						return JsonConvert.DeserializeObject<List<GsFile>>(filesResponse);
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"GetProjectFiles service method: {e.Message}\n {e.StackTrace}");
			}
			return new List<GsFile>();
		}

		public async Task<List<GsFileVersion>> GetFileVersion(string languageFileId)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetFileVersions(languageFileId)));
					ApiUrl.AddRequestHeaders(httpClient, request);

					var responseMessage = await httpClient.SendAsync(request);
					var filesResponse = await responseMessage.Content.ReadAsStringAsync();
					if (responseMessage.StatusCode == HttpStatusCode.OK)
					{
						return JsonConvert.DeserializeObject<List<GsFileVersion>>(filesResponse);
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"GetFileVersion service method: {e.Message}\n {e.StackTrace}");

			}
			return new List<GsFileVersion>();
		}

		public async Task<byte[]> DownloadFileVersion(string projectId, string languageFileId, int version)
		{
			try
			{
				using (var httpClient = new HttpClient())
				{
					var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.DownloadFileVersion(projectId, languageFileId, version)));
					ApiUrl.AddRequestHeaders(httpClient, request);

					var responseMessage = await httpClient.SendAsync(request);
					var fileResponse = await responseMessage.Content.ReadAsByteArrayAsync();
					if (responseMessage.StatusCode == HttpStatusCode.OK)
					{
						return fileResponse;
					}
				}
			}
			catch (Exception e)
			{
				Log.Logger.Error($"DownloadFileVersion service method: {e.Message}\n {e.StackTrace}");
			}
			return new byte[0];
		}
	}
}
