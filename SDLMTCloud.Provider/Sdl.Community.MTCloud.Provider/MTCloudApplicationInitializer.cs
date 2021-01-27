using System.Net.Http.Headers;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;

namespace Sdl.Community.MTCloud.Provider
{
	[ApplicationInitializer]
	internal class MTCloudApplicationInitializer : IApplicationInitializer
	{
		public static IHttpClient Client { get; private set; }

		public void Execute()
		{
			Client = new HttpClient();
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}