using System;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	[ApplicationInitializer]
	public class SettingsService : IApplicationInitializer
	{
		private const string BatchProcessing = "batch processing";
		private const string CreateNewProject = "create a new project";
		private static ProjectsController _projectsController;
		public static CurrentViewDetector CurrentViewDetector { get; set; }

		private static ProjectsController ProjectsController
					=> _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

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

		public static SettingsModel GetSettingsForCurrentProject()
		{
			try
			{
				var serializedSettings = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					$@"RWS AppStore\IATETerminologyProvider\Settings\{GetProjectInProcessing().GetProjectInfo().Id}",
					"IATESettings.json"));

				return string.IsNullOrEmpty(serializedSettings)
					? null
					: JsonConvert.DeserializeObject<SettingsModel>(serializedSettings);
			}
			catch { }

			return null;
		}

		public static void SaveSettingsForCurrentProject(SettingsModel settings)
		{
			var serializedSettings = JsonConvert.SerializeObject(settings);

			var settingsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				$@"RWS AppStore\IATETerminologyProvider\Settings\{GetProjectInProcessing().GetProjectInfo().Id}");

			Directory.CreateDirectory(settingsFolderPath);

			File.WriteAllText(
				$@"{settingsFolderPath}\IATESettings.json",
				serializedSettings);
		}

		public void Execute()
		{
			CurrentViewDetector = new CurrentViewDetector();
		}
	}
}