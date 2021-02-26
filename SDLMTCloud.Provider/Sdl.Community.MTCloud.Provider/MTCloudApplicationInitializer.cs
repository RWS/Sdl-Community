using System.Net.Http.Headers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider
{
	[ApplicationInitializer]
	internal class MtCloudApplicationInitializer : IApplicationInitializer
	{
		private static EditorController _editorController;
		public static IHttpClient Client { get; private set; }

		public static EditorController EditorController
			=> _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();

		public static MetadataSupervisor MetadataSupervisor
			=> new(new SegmentMetadataCreator(), EditorController);

		public static TranslationService TranslationService { get; private set; }

		public static void SetTranslationService(IConnectionService connectionService)
		{
			TranslationService = new TranslationService(connectionService, Client);
			MetadataSupervisor.StartSupervising(TranslationService);
		}

		public void Execute()
		{
			Client = new HttpClient();
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}