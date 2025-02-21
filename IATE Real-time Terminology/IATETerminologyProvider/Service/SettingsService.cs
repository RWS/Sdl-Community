using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
		private static CurrentViewDetector currentViewDetector;

		public static CurrentViewDetector CurrentViewDetector
		{
			get => currentViewDetector ??= new CurrentViewDetector();
			set => currentViewDetector = value;
		}

		private static ProjectsController ProjectsController
					=> _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

		public static Window GetCurrentWindow() => Application.Current.Windows.Cast<Window>().FirstOrDefault(
			window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));

		public static FileBasedProject GetProjectInProcessing()
		{
			if (SdlTradosStudio.Application is null)
				return null;
			if (GetCurrentWindow()?.Title.ToLower().Contains(CreateNewProject) ?? false)
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

		public static SettingsModel GetSettingsForCurrentProject()
		{
			try
			{
				var serializedSettings = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					$@"Trados AppStore\IATETerminologyProvider\Settings\{GetProjectInProcessing().GetProjectInfo().Id}",
					"IATESettings.json"));

				return string.IsNullOrEmpty(serializedSettings)
					? null
					: JsonConvert.DeserializeObject<SettingsModel>(serializedSettings);
			}
			catch { }

			return null;
		}

		public static async Task<SettingsModel> GetSettingsFromTemplate(string path)
		{
			try
			{
				var settingsJson = await Task.Run(() => File.ReadAllText(path));
				var settings = JsonConvert.DeserializeObject<SettingsModel>(settingsJson);

				return settings;
			}
			catch { }

			return null;
		}

		public static async Task SaveSettingsAtChosenLocation(SettingsModel settingsModel, string path)
		{
			var availableFilePath = GetAvailableFileName(path);
			await Task.Run(() => File.WriteAllText(availableFilePath, JsonConvert.SerializeObject(settingsModel)));
		}

		private static string GetAvailableFileName(string filePath)
		{
			try
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			catch
			{
				return GetAvailableFileName(filePath.Insert(filePath.IndexOf(".xlsx", StringComparison.Ordinal), "(new)"));
			}

			return filePath;
		}


		public static async Task SaveSettingsForCurrentProject(SettingsModel settings, string path = null)
		{
			var serializedSettings = JsonConvert.SerializeObject(settings);

			var settingsFolderPath = path ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
				$@"Trados AppStore\IATETerminologyProvider\Settings\{GetProjectInProcessing().GetProjectInfo().Id}");


			await Task.Run(() =>
			{
				Directory.CreateDirectory(settingsFolderPath);

				File.WriteAllText(
					$@"{settingsFolderPath}\IATESettings.json",
					serializedSettings);
			});
		}

		public void Execute()
		{
		}
	}
}