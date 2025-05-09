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
    public class OrganizationService
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task<List<OrganizationResponse>> GetOrganizations()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ApiUrl.GetOrganizations()));
                ApiUrl.AddRequestHeaders(request);

                var responseMessage = await AppInitializer.Client.SendAsync(request);
                var organizationsResponse = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<List<OrganizationResponse>>(organizationsResponse);
                }
            }
            catch (Exception e)
            {
                Logger.Error($"GetOrganizations service method: {e.Message}\n {e.StackTrace}");
            }
            return new List<OrganizationResponse>();
        }
    }
}