using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.FileBased;
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

		public static ProjectsController ProjectsController { get; private set; }
		public static TranslationService TranslationService { get; private set; }

		public static CurrentViewDetector CurrentViewDetector { get; set; } = new();

		public static Window GetCurrentWindow()
		{
			return
				Application.Current.Windows.Cast<Window>().FirstOrDefault(
					window => window.Title == "Batch Processing" || window.Title.Contains("Create a New Project"));
		}

		public static void SetTranslationService(IConnectionService connectionService)
		{
			TranslationService = new TranslationService(connectionService, Client);

			//TODO: start supervising when a QE enabled model has been chosen
			MetadataSupervisor.StartSupervising(TranslationService);
		}

		public static void CloseOpenedDocuments()
		{
			var activeDocs = _editorController.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				_editorController.Close(activeDoc);
			}
		}

		public static FileBasedProject GetProjectInProcessing()
		{
			if (GetCurrentWindow()?.Title.Contains("Create a New Project") ?? false) return null;

			FileBasedProject currentProjectInProcessingPath = null;
			switch (CurrentViewDetector.View)
			{
				case CurrentViewDetector.CurrentView.ProjectsView:
					currentProjectInProcessingPath = ProjectsController.SelectedProjects.FirstOrDefault() ??
					                                 ProjectsController.CurrentProject;
					break;
				case CurrentViewDetector.CurrentView.FilesView:
				case CurrentViewDetector.CurrentView.EditorView:
					currentProjectInProcessingPath = ProjectsController.CurrentProject;
					break;
			}
			return currentProjectInProcessingPath;
		}

		public void Execute()
		{
			ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			Client = new HttpClient();
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}