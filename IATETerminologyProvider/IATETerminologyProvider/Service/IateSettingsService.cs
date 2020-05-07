using System.Linq;
using Sdl.Community.IATETerminologyProvider.Interface;
using Sdl.Community.IATETerminologyProvider.Model;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace Sdl.Community.IATETerminologyProvider.Service
{
	public class IateSettingsService:IIateSettingsService
	{
		private static ISettingsGroup _settingsGroup;
		private readonly FileBasedProject _currentProject;

		public IateSettingsService()
		{
			var app = ApplicationHost<SdlTradosStudioApplication>.Application;
			var projectsController = app.GetController<ProjectsController>();
			_currentProject = projectsController.CurrentProject ?? projectsController.SelectedProjects.FirstOrDefault();
		}

		public ProviderSettings GetProviderSettings()
		{
			var projectSettings = _currentProject?.GetSettings();
			if (projectSettings is null) return null;

			var containsSettingsGroup = projectSettings.ContainsSettingsGroup(nameof(ProviderSettings));
			_settingsGroup = projectSettings.GetSettingsGroup(nameof(ProviderSettings));
			if (!containsSettingsGroup)
			{
				projectSettings.AddSettingsGroup(_settingsGroup);
			}

			_settingsGroup = projectSettings.GetSettingsGroup(nameof(ProviderSettings));
			var settings = projectSettings.GetSettingsGroup<ProviderSettings>(nameof(ProviderSettings));
			return settings;
		}

		public void RemoveProviderSettings()
		{
			var projectSettings = _currentProject?.GetSettings();
			if (projectSettings is null) return;
			var containsSettingsGroup = projectSettings.ContainsSettingsGroup(nameof(ProviderSettings));
			if (!containsSettingsGroup) return;
			projectSettings.RemoveSettingsGroup(nameof(ProviderSettings));
			_currentProject.UpdateSettings(projectSettings);
		}

		public void SaveProviderSettings(ProviderSettings settings)
		{
			if(_currentProject is null)return;
			_currentProject.UpdateSettings(settings.SettingsBundle);
			_currentProject.Save();
		}
	}
}