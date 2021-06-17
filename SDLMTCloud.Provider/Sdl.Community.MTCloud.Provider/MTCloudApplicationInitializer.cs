using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Community.MTCloud.Provider.Studio;
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
		private const string BatchProcessing = "batch processing";
		private const string CreateNewProject = "create a new project";
		private static IStudioEventAggregator _eventAggregator;
		private static bool? _isStudioRunning;
		public static IHttpClient Client { get; } = new HttpClient();

		public static CurrentViewDetector CurrentViewDetector { get; set; }

		public static EditorController EditorController { get; set; }

		public static MetadataSupervisor MetadataSupervisor { get; set; }

		public static ProjectsController ProjectsController { get; private set; }

		public static TranslationService TranslationService { get; private set; }

		private static IStudioEventAggregator EventAggregator { get; } = _eventAggregator ??
																		 (IsStudioRunning()
																			 ? SdlTradosStudio.Application
																				 .GetService<IStudioEventAggregator>()
																			 : null);

		public static RateItController RateItController => IsStudioRunning() ? SdlTradosStudio.Application.GetController<RateItController>() : null;

		public static Window GetCurrentWindow() => Application.Current.Windows.Cast<Window>().FirstOrDefault(
			window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));

		public static FileBasedProject GetProjectInProcessing()
		{
			if (SdlTradosStudio.Application is null) return null;
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

		public static bool IsStudioRunning()
		{
			if (_isStudioRunning is not null) return _isStudioRunning.Value;

			try
			{
				_isStudioRunning ??= SdlTradosStudio.Application is not null;
				return _isStudioRunning.Value;
			}
			catch { }

			return _isStudioRunning ??= false;
		}

		public static void PublishEvent<TEvent>(TEvent sampleEvent)
		{
			EventAggregator?.Publish(sampleEvent);
		}

		public static void RefreshQeStatus()
		{
			if (TranslationService?.IsActiveModelQeEnabled ?? false)
				PublishEvent(new RefreshQeStatus());
		}

		public static void SetTranslationService(IConnectionService connectionService)
		{
			TranslationService = new TranslationService(connectionService, Client, new MessageBoxService());

			//TODO: start supervising when a QE enabled model has been chosen

			MetadataSupervisor?.StartSupervising(TranslationService);
		}

		public static void Subscribe<T>(Action<T> action)
		{
			EventAggregator?.GetEvent<T>().Subscribe(action);
		}

		public void Execute()
		{
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			if (IsStudioRunning())
			{
				ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
				CurrentViewDetector = new CurrentViewDetector();
				EditorController = SdlTradosStudio.Application.GetController<EditorController>();
				MetadataSupervisor = new MetadataSupervisor(new SegmentMetadataCreator(), EditorController);
			}
		}
	}
}