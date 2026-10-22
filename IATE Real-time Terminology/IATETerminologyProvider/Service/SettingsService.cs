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
using TradosStudio.API.ProjectManagement;

namespace Sdl.Community.IATETerminologyProvider.Service
{
    public class SettingsService
    {
        private const string BatchProcessing = "batch processing";
        private const string CreateNewProject = "create a new project";
        private static CurrentViewDetector currentViewDetector;

        private static CurrentViewDetector CurrentViewDetector
        {
            get => currentViewDetector ??= new CurrentViewDetector();
            set => currentViewDetector = value;
        }

        private static bool IsCreatingNewProjectWindowOpen()
        {
            var application = Application.Current;
            if (application == null)
            {
                return false;
            }

            Func<bool> findWindow = () =>
            {
                var currentWindow = application.Windows.Cast<Window>().FirstOrDefault(window =>
                    string.Equals(window.Title, BatchProcessing, StringComparison.OrdinalIgnoreCase) ||
                    (!string.IsNullOrEmpty(window.Title) &&
                     window.Title.IndexOf(CreateNewProject, StringComparison.OrdinalIgnoreCase) >= 0));

                return currentWindow != null &&
                       !string.Equals(currentWindow.Title, BatchProcessing, StringComparison.OrdinalIgnoreCase);
            };

            return application.Dispatcher.CheckAccess()
                ? findWindow()
                : application.Dispatcher.Invoke(findWindow);
        }

        private static string GetProjectInProcessingId(IProjectsRegistry projectsRegistry)
        {
            if (SdlTradosStudio.Application is null)
            {
                return null;
            }

            if (IsCreatingNewProjectWindowOpen())
            {
                return null;
            }

            var projectId = CurrentViewDetector.View
                switch
            {
                CurrentViewDetector.CurrentView.ProjectsView => projectsRegistry.GetSelectedProjects()?.FirstOrDefault()?.Id ?? projectsRegistry.GetActiveProject()?.Id,
                CurrentViewDetector.CurrentView.FilesView => projectsRegistry.GetActiveProject()?.Id,
                CurrentViewDetector.CurrentView.EditorView => projectsRegistry.GetActiveProject()?.Id,
                _ => null
            };
            return projectId?.ToString();
        }

        public static SettingsModel GetSettingsForCurrentProject(IProjectsRegistry projectsRegistry)
        {
            try
            {
                var projectId = GetProjectInProcessingId(projectsRegistry);
                if (string.IsNullOrEmpty(projectId))
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


        public static async Task SaveSettingsForCurrentProject(SettingsModel settings, IProjectsRegistry projectsRegistry, string path = null)
        {
            var serializedSettings = JsonConvert.SerializeObject(settings);

            var settingsFolderPath = path;
            if (string.IsNullOrWhiteSpace(settingsFolderPath))
            {
                var projectId = GetProjectInProcessingId(projectsRegistry);

                if (string.IsNullOrEmpty(projectId))
                {
                    throw new InvalidOperationException("Cannot save settings because no current Trados project could be determined.");
                }

                settingsFolderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Trados AppStore",
                    "IATETerminologyProvider",
                    "Settings",
                    projectId);
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