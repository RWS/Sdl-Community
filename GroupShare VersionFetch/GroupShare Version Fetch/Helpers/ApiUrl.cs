using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public class ApiUrl
	{
		public static string BaseUrl;
		public  static List<string> Scopes= new List<string> {"ManagementRestApi", "ProjectServerRestApi", "MultiTermRestApi", "TMServerRestApi"};
		private static readonly string CurrentProjectServerUrl = "api/projectserver/v2";
		private static readonly string CurrentManagementServerUrl = "api/management/v2";

		public static string Login()
		{
			return $"{BaseUrl}/authentication/api/1.0/login";
		}

		public static string GetProjects()
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects";
		}

		public static string GetProjectFiles(string projectId)
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/{projectId}/files";
		}

		public static string GetFileVersions(string languageFileId)
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/fileversions/{languageFileId}";
		}

		public static string DownloadFileVersion(string projectId, string languageFileId, int version)
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/{projectId}/fileversions/download/{languageFileId}/{version}";
		}

		public static void AddRequestHeaders(HttpClient httpClient, HttpRequestMessage request)
		{
			httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Authentication.Token);
		}

		public static string GetQuerryString(ProjectFilter projectFilter)
		{
			var baseUrl = GetProjects();
			var builder = new UriBuilder(baseUrl);
			var query = HttpUtility.ParseQueryString(builder.Query);
			query["page"] = projectFilter.Page.ToString();
			query["start"] = "0";
			query["limit"] = projectFilter.PageSize.ToString();

			if (projectFilter.Filter!=null)
			{
				query["filter"] = JsonConvert.SerializeObject(projectFilter.Filter); 
			}
			builder.Query = query.ToString();

			return builder.ToString();
		}

		public static string GetOrganizations()
		{
			return $"{BaseUrl}/{CurrentManagementServerUrl}/organizations";
		}
	}
}
