using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Sdl.Community.GSVersionFetch.Service;

namespace Sdl.Community.GSVersionFetch.Helpers
{
	public class ApiUrl
	{
		public static string BaseUrl;
		public  static List<string> Scopes= new List<string> {"ManagementRestApi", "ProjectServerRestApi", "MultiTermRestApi", "TMServerRestApi"};
		private static string CurrentProjectServerUrl = "api/projectserver/v2";

		public static string Login()
		{
			return $"{BaseUrl}/authentication/api/1.0/login";
		}

		public static string GetProjects()
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects";
		}

		public static void AddRequestHeaders(HttpClient httpClient,HttpRequestMessage request)
		{
			httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
			httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Authentication.Token);
		}

		public static string GetProjectFiles(string projectId)
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/{projectId}/files";
		}

		public static string GetFileVersions(string languageFileId)
		{
			return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/fileversions/{languageFileId}";
		}
	}
}
