using Newtonsoft.Json;
using NLog;
using Sdl.Community.IATETerminologyProvider.Helpers;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class SettingsService
	{
		private const string BatchProcessing = "batch processing";
		private const string CreateNewProject = "create a new project";
		private static ProjectsController _projectsController;
		private static CurrentViewDetector currentViewDetector;

		private static CurrentViewDetector CurrentViewDetector
		{
			get => currentViewDetector ??= new CurrentViewDetector();
			set => currentViewDetector = value;
		}

		private static ProjectsController ProjectsController
					=> _projectsController ??= SdlTradosStudio.Application?.GetController<ProjectsController>();

		public static Window GetCurrentWindow() => Application.Current.Windows.Cast<Window>().FirstOrDefault(
			window => window.Title.ToLower() == BatchProcessing || window.Title.ToLower().Contains(CreateNewProject));

		private static FileBasedProject GetProjectInProcessing()
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
                var projectId = GetProjectInProcessing()?.GetProjectInfo()?.Id;
                if (projectId == null)
                {
                    return new SettingsModel();
                }

                var serializedSettings = File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					$@"Trados AppStore\IATETerminologyProvider\Settings\{projectId}",
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

			var settingsFolderPath = path;
			if (string.IsNullOrWhiteSpace(settingsFolderPath))
			{
				var projectId = GetProjectInProcessing()?.GetProjectInfo()?.Id;
				if (projectId == null)
				{
					throw new InvalidOperationException("Cannot save settings because no current Trados project could be determined.");
				}

				settingsFolderPath = Path.Combine(
					Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					"Trados AppStore",
					"IATETerminologyProvider",
					"Settings",
					projectId.ToString());
			}


			await Task.Run(() =>
			{
				Directory.CreateDirectory(settingsFolderPath);

				File.WriteAllText(
					$@"{settingsFolderPath}\IATESettings.json",
					serializedSettings);
			});
		}
	}
}