using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Policy;
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
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
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
		public static IFileTypeManager FileTypeManager { get; set; }
		public static MetadataSupervisor MetadataSupervisor { get; set; }
		public static string ProjectInCreationFilePath { get; set; }
		public static ProjectsController ProjectsController { get; private set; }
		public static ISegmentSupervisor SegmentSupervisor { get; set; }
		public static ITranslationService TranslationService { get; private set; }

		private static IStudioEventAggregator EventAggregator { get; } = _eventAggregator ??
																		 (IsStudioRunning()
																			 ? _eventAggregator = SdlTradosStudio.Application
																				 .GetService<IStudioEventAggregator>()
																			 : null);


		public static string EnsureValidPath(string filePath, string targetLanguage)
		{
			if (File.Exists(filePath))
				return filePath;

			const string filenameExtension = ".sdlxliff";
			var separatorTokens = new string[] { $@"{targetLanguage.ToLower()}\" };
			var fileName = filePath.ToLower()
								   .Split(separatorTokens, StringSplitOptions.RemoveEmptyEntries)
								   .LastOrDefault();
			fileName += !fileName.EndsWith(filenameExtension) ? filenameExtension : string.Empty;
			var projectPath = Path.GetDirectoryName(ProjectInCreationFilePath) ??
							  Path.GetDirectoryName(GetProjectInProcessing()?.FilePath);
			var processedPath = $@"{projectPath}\{targetLanguage}\{fileName}";

			if (File.Exists(processedPath))
				return processedPath;

			if (string.IsNullOrEmpty(projectPath))
				return null;

			var targetLanguageFiles = Directory.GetFiles(projectPath);
			processedPath = targetLanguageFiles.FirstOrDefault(
					file =>
						Path.GetFileName(file).Contains(Path.GetFileNameWithoutExtension(filePath)) &&
						Path.GetExtension(file) == filenameExtension);

			return File.Exists(processedPath) ? processedPath : null;
		}

		public static Window GetCurrentWindow() => Application.Current.Windows.Cast<Window>().FirstOrDefault(
					window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));

		public static FileBasedProject GetProjectInProcessing()
		{
			if (!IsStudioRunning())
				return null;

			if (Application.Current.Dispatcher.Invoke(() =>
					GetCurrentWindow()?.Title.ToLower().Contains(CreateNewProject) ?? false))
				return null;

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
			if (_isStudioRunning is not null)
				return _isStudioRunning.Value;

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

		public static IDisposable Subscribe<T>(Action<T> action) => EventAggregator?.GetEvent<T>().Subscribe(action);

		public void Execute()
		{
			ConnectionService = new ConnectionService(StudioInstance.GetActiveForm(), new VersionService(), Client);
			Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			if (IsStudioRunning())
			{
				FileTypeManager = DefaultFileTypeManager.CreateInstance(true);
				ProjectsController = SdlTradosStudio.Application.GetController<ProjectsController>();
				CurrentViewDetector = new CurrentViewDetector();
				EditorController = SdlTradosStudio.Application.GetController<EditorController>();
				MetadataSupervisor = new MetadataSupervisor(new SegmentMetadataCreator(), EditorController);
				SegmentSupervisor = new SegmentSupervisor(EditorController);
			}
		}

		public static ConnectionService ConnectionService { get; set; }
	}
}