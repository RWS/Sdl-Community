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
		private static bool? _isStudioRunning;

		public static IHttpClient Client { get; } = new HttpClient();
		public static CurrentViewDetector CurrentViewDetector { get; set; }
		public static EditorController EditorController { get; set; }

		public static bool IsStudioRunning { get => _isStudioRunning ?? false; set => _isStudioRunning = value; }
		public static IMessageBoxService MessageService { get; } = new MessageBoxService();
		public static MetadataSupervisor MetadataSupervisor { get; set; }

		public static ProjectsController ProjectsController { get; private set; }
		public static TranslationService TranslationService { get; private set; }

		public static Window GetCurrentWindow() => Application.Current.Windows.Cast<Window>().FirstOrDefault(
			window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));

		public static FileBasedProject GetProjectInProcessing()
		{
			if (SdlTradosStudio.Application is null) return null;
			if (GetCurrentWindow()?.Title.ToLower().Contains(CreateNewProject) ?? false) return null;

			FileBasedProject projectInProcessing;
			switch (CurrentViewDetector.View)
			{
				case CurrentViewDetector.CurrentView.ProjectsView:
					projectInProcessing = ProjectsController.SelectedProjects.FirstOrDefault() ?? ProjectsController.CurrentProject;
					break;

				case CurrentViewDetector.CurrentView.FilesView:
				case CurrentViewDetector.CurrentView.EditorView:
					projectInProcessing = ProjectsController.CurrentProject;
					break;

				default:
					projectInProcessing = null;
					break;
			}
			return projectInProcessing;
		}

		/// <summary>
		/// Since SdlTradosStudio is a static class, the moment we access .Application on it without Studio running we get an exception
		/// and the class itself cannot be referred to, therefore we cannot check if it has been initialized
		/// </summary>
		public static void SetIsStudioRunning()
		{
			if (_isStudioRunning.HasValue) return;
			try
			{
				IsStudioRunning = SdlTradosStudio.Application != null;
			}
			catch { }
		}

		public static void SetTranslationService(IConnectionService connectionService)
		{
			TranslationService = new TranslationService(connectionService, Client, MessageService);

			//TODO: start supervising when a QE enabled model has been chosen

			if (!IsStudioRunning) return;
			MetadataSupervisor.StartSupervising(TranslationService);
		}

		public void Execute()
		{
			SetIsStudioRunning();
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			if (!IsStudioRunning) return;
			ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
			CurrentViewDetector = new CurrentViewDetector();
			EditorController = SdlTradosStudio.Application.GetController<EditorController>();
			MetadataSupervisor = new MetadataSupervisor(new SegmentMetadataCreator(), EditorController);
		}
	}
}