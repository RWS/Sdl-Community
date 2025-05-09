using Newtonsoft.Json;
using Sdl.Community.GSVersionFetch.Model;
using Sdl.Community.GSVersionFetch.Service;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Sdl.Community.GSVersionFetch.Helpers
{
    public static class ApiUrl
    {
        public static string BaseUrl;
        public static string Scopes = "ManagementRestApi ProjectServerRestApi MultiTermRestApi TMServerRestApi";
        private static readonly string CurrentManagementServerUrl = "api/management/v2";
        private static readonly string CurrentProjectServerUrl = "api/projectserver/v2";
        public static string Login => $"{BaseUrl}api/authentication/token";

        public static void AddRequestHeaders(HttpRequestMessage request, string content = null, bool withToken = true)
        {
            request.Headers.Connection.Add("Keep-Alive");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            if (withToken)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Authentication.Token);
            }

            if (request.Method == HttpMethod.Post)
            {
                request.Content = new StringContent(content, Encoding.UTF8,
                    "application/x-www-form-urlencoded");
            }
        }

        public static string DownloadFileVersion(string projectId, string languageFileId, int version)
        {
            return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/{projectId}/fileversions/download/{languageFileId}/{version}";
        }

        public static string GetFileVersions(string languageFileId)
        {
            return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/fileversions/{languageFileId}";
        }

        public static string GetOrganizations()
        {
            return $"{BaseUrl}/{CurrentManagementServerUrl}/organizations";
        }

        public static string GetProjectFiles(string projectId)
        {
            return $"{BaseUrl}/{CurrentProjectServerUrl}/projects/{projectId}/files";
        }

        public static string GetProjects()
        {
            return $"{BaseUrl}/{CurrentProjectServerUrl}/projects";
        }

        public static string GetProjectsUri(ProjectFilter projectFilter)
        {
            var baseUrl = GetProjects();
            var uri = $"{baseUrl}?page={projectFilter.Page}" +
                      "&start=0" +
                      $"&limit={projectFilter.PageSize}";

            if (projectFilter.Filter != null)
            {
                uri += $"&filter={JsonConvert.SerializeObject(projectFilter.Filter)}";
            }

            return uri;
        }

        public static string PlusEncodeSpaces(string toEncode) => toEncode.Replace(' ', '+');
    }
}