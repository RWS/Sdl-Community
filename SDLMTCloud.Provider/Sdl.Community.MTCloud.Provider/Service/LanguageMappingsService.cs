using System.Linq;
using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Core.Settings;
using Sdl.Desktop.IntegrationApi;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Internal;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class LanguageMappingsService
	{		
		private static ISettingsGroup _settingsGroup;
		private readonly FileBasedProject _currentProject;

		public LanguageMappingsService()
		{
			var app = ApplicationHost<SdlTradosStudioApplication>.Application;
			var projectsController = app.GetController<ProjectsController>();
			_currentProject = projectsController.CurrentProject ?? projectsController.SelectedProjects.FirstOrDefault();
		}

		public LanguageMappingSettings GetLanguageMappingSettings()
		{			
			var projectSettings = _currentProject?.GetSettings();
			if (projectSettings != null)
			{
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(Constants.SettingsGroupId);
				_settingsGroup = projectSettings.GetSettingsGroup(Constants.SettingsGroupId);
				if (!containsSettingsGroup)
				{
					projectSettings.AddSettingsGroup(_settingsGroup);					
				}

				_settingsGroup = projectSettings.GetSettingsGroup(Constants.SettingsGroupId);
				var settings = projectSettings.GetSettingsGroup<LanguageMappingSettings>(Constants.SettingsGroupId);
				
				return settings;
			}

			return null;
		}
	
		public void RemoveLanguageMappingSettings()
		{			
			var projectSettings = _currentProject?.GetSettings();
			if (projectSettings != null)
			{
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(Constants.SettingsGroupId);
				if (containsSettingsGroup)
				{
					projectSettings.RemoveSettingsGroup(Constants.SettingsGroupId);
					_currentProject.UpdateSettings(projectSettings);
				}
			}
		}

		public void SaveLanguageMappingSettings(LanguageMappingSettings settings)
		{
			_currentProject?.UpdateSettings(settings.SettingsBundle);
			_currentProject?.Save();			
		}		
	}
}