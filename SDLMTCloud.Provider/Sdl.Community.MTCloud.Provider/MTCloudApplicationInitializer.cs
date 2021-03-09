using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Provider
{
	[ApplicationInitializer]
	internal class MtCloudApplicationInitializer : IApplicationInitializer
	{
		private const string BatchProcessing = "batch processing";
		private const string CreateNewProject = "create a new project";
		private static EditorController _editorController;
		public static IHttpClient Client { get; private set; }

		public static CurrentViewDetector CurrentViewDetector { get; set; } = new();

		public static EditorController EditorController
					=> _editorController ??= SdlTradosStudio.Application.GetController<EditorController>();

		public static MetadataSupervisor MetadataSupervisor
			=> new(new SegmentMetadataCreator(), EditorController);

		public static ProjectsController ProjectsController { get; private set; }
		public static TranslationService TranslationService { get; private set; }

		public static void CloseOpenedDocuments()
		{
			var activeDocs = _editorController.GetDocuments().ToList();

			foreach (var activeDoc in activeDocs)
			{
				_editorController.Close(activeDoc);
			}
		}

		public static Window GetCurrentWindow() => Application.Current.Windows.Cast<Window>().FirstOrDefault(
			window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));

		public static FileBasedProject GetProjectInProcessing()
		{
			if (GetCurrentWindow()?.Title.ToLower().Contains(CreateNewProject) ?? false) return null;

			var projectInProcessing = CurrentViewDetector.View
				switch
			{
				CurrentViewDetector.CurrentView.ProjectsView => ProjectsController.SelectedProjects.FirstOrDefault() ?? ProjectsController.CurrentProject,
				CurrentViewDetector.CurrentView.FilesView => ProjectsController.CurrentProject,
				CurrentViewDetector.CurrentView.EditorView => ProjectsController.CurrentProject,
				_ => null
			};
			return projectInProcessing;
		}

		public static void SetTranslationService(IConnectionService connectionService)
		{
			TranslationService = new TranslationService(connectionService, Client, new MessageBoxService());

			//TODO: start supervising when a QE enabled model has been chosen
			MetadataSupervisor.StartSupervising(TranslationService);
		}

		public void Execute()
		{
			ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			Client = new HttpClient();
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}
	}
}