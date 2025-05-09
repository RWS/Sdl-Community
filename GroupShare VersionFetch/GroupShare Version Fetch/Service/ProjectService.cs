using Newtonsoft.Json;
using NLog;
using Sdl.Community.GSVersionFetch.Helpers;
using Sdl.Community.GSVersionFetch.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sdl.Community.GSVersionFetch.Service
{
    public class ProjectService
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<byte[]> DownloadFileVersion(string projectId, string languageFileId, int version)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.DownloadFileVersion(projectId, languageFileId, version)));
                ApiUrl.AddRequestHeaders(request);

                var responseMessage = await AppInitializer.Client.SendAsync(request);
                var fileResponse = await responseMessage.Content.ReadAsByteArrayAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return fileResponse;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"DownloadFileVersion service method: {e.Message}\n {e.StackTrace}");
            }
            return new byte[0];
        }

        public async Task<List<GsFileVersion>> GetFileVersion(string languageFileId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetFileVersions(languageFileId)));
                ApiUrl.AddRequestHeaders(request);

                var responseMessage = await AppInitializer.Client.SendAsync(request);
                var filesResponse = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<List<GsFileVersion>>(filesResponse);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetFileVersion service method: {e.Message}\n {e.StackTrace}");
            }
            return new List<GsFileVersion>();
        }

        public async Task<ProjectResponse> GetGsProjects(ProjectFilter projectFilter)
        {
            try
            {
                var queryString = ApiUrl.GetProjectsUri(projectFilter);
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(queryString));
                ApiUrl.AddRequestHeaders(request);

                var responseMessage = await AppInitializer.Client.SendAsync(request);
                var projectsResponse = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<ProjectResponse>(projectsResponse);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetGsProjects service method: {e.Message}\n {e.StackTrace}");
            }
            return new ProjectResponse();
        }

        public async Task<List<GsFile>> GetProjectFiles(string projectId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetProjectFiles(projectId)));
                ApiUrl.AddRequestHeaders(request);

                var responseMessage = await AppInitializer.Client.SendAsync(request);
                var filesResponse = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<List<GsFile>>(filesResponse);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetProjectFiles service method: {e.Message}\n {e.StackTrace}");
            }
            return new List<GsFile>();
        }
    }
}