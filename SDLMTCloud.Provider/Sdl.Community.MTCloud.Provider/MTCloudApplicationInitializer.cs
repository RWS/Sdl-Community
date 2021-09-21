using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Windows;
using Sdl.Community.MTCloud.Provider.Events;
using Sdl.Community.MTCloud.Provider.Helpers;
using Sdl.Community.MTCloud.Provider.Interfaces;
using Sdl.Community.MTCloud.Provider.Service;
using Sdl.Community.MTCloud.Provider.Service.Interface;
using Sdl.Community.MTCloud.Provider.Service.RateIt;
using Sdl.Community.MTCloud.Provider.Studio.TranslationProvider;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.Desktop.IntegrationApi.Interfaces;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.MTCloud.Providerw
{
	[ApplicationInitializer]
	internal class MtCloudApplicationInitializer : IApplicationInitializer
	{
		private const string BatchProcessing = "batch processing";
		private const string CreateNewProject = "create a new project";
		private const string ProjectInProcessing = "ProjectInProcessing";
		private static IStudioEventAggregator _eventAggregator;
		private static bool? _isStudioRunning;
		public static IHttpClient Client { get; } = new HttpClient();

		public static CurrentViewDetector CurrentViewDetector { get; set; }
		public static EditorController EditorController { get; set; }
		public static MetadataSupervisor MetadataSupervisor { get; set; }
		public static string ProjectInCreationFilePath { get; set; }
		public static ProjectsController ProjectsController { get; private set; }
		public static ITranslationService TranslationService { get; private set; }

		private static string CurrentProjectId
		{
			get
			{
				var currentProject = GetProjectInProcessing();
				return currentProject is null ? ProjectInProcessing : currentProject.GetProjectInfo().Id.ToString();
			}
		}

		private static IStudioEventAggregator EventAggregator { get; } = _eventAggregator ??
																		 (IsStudioRunning()
																			 ? _eventAggregator = SdlTradosStudio.Application
																				 .GetService<IStudioEventAggregator>()
																			 : null);

		private static Dictionary<string, SdlMTCloudTranslationProvider> Providers { get; set; } = new();

		public static void AddCurrentProjectProvider(SdlMTCloudTranslationProvider provider)
		{
			if (!IsStudioRunning()) return;
			if (IsProjectCreationTime())
			{
				AttachToProjectCreatedEvent();
			}

			if (string.IsNullOrEmpty(CurrentProjectId)) return;

			Providers[CurrentProjectId] = provider;
		}

		public static SdlMTCloudTranslationProvider GetCurrentProjectProvider()
		{
			return Providers.ContainsKey(ProjectInProcessing)
				? Providers[ProjectInProcessing]
				: string.IsNullOrEmpty(CurrentProjectId) ? null : Providers.ContainsKey(CurrentProjectId) ? Providers[CurrentProjectId] : null;
		}

		public static Window GetCurrentWindow()
		{
			return
				Application.Current.Windows.Cast<Window>().FirstOrDefault(
					window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));
		}

		public static FileBasedProject GetProjectInProcessing()
		{
			if (!IsStudioRunning()) return null;
			if (Application.Current.Dispatcher.Invoke(() => GetCurrentWindow()?.Title.ToLower().Contains(CreateNewProject) ?? false))
			{
				return null;
			}

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

		public static bool IsProjectCreationTime()
		{
			return
				Application.Current.Dispatcher.Invoke(
					() =>
						GetCurrentWindow()?.Title.ToLower().Contains(CreateNewProject) ?? false);
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

		public static void SetTranslationService(IConnectionService connectionService, ITranslationService translationService)
		{
			TranslationService = translationService ?? new TranslationService(connectionService, Client, new MessageBoxService());
			MetadataSupervisor?.StartSupervising(TranslationService);
			SegmentSupervisor?.StartSupervising(TranslationService);
		}

		public static IDisposable Subscribe<T>(Action<T> action)
		{
			return EventAggregator?.GetEvent<T>().Subscribe(action);
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
				SegmentSupervisor = new SegmentSupervisor(EditorController);
			}
		}

		public static ISegmentSupervisor SegmentSupervisor { get; set; }

		private static void AttachToProjectCreatedEvent()
		{
			ProjectsController.CurrentProjectChanged += ProjectsController_CurrentProjectChanged;
		}

		private static void ProjectsController_CurrentProjectChanged(object sender, EventArgs e)
		{
			ProjectsController.CurrentProjectChanged -= ProjectsController_CurrentProjectChanged;

			if (!Providers.ContainsKey(ProjectInProcessing)) return;

			var currentProvider = Providers[ProjectInProcessing];
			Providers.Remove(ProjectInProcessing);

			var projectInCreation = ProjectsController.CurrentProject;

			ProjectInCreationFilePath = projectInCreation.FilePath;
			Providers[projectInCreation.GetProjectInfo().Id.ToString()] = currentProvider;
		}
	}
}