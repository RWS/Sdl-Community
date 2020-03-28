using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Core.Settings;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class LanguageMappingsService
	{		
		private static ISettingsGroup _settingsGroup;

		public LanguageMappingsService()
		{
			
		}

		/// <summary>
		/// Get the saved language mapping settings from current project
		/// </summary>
		public LanguageMappingSettings GetLanguageMappingSettings()
		{
			var currentProject = GetCurrentProject() ?? AppInitializer.GetFileController()?.CurrentProject;
			var projectSettings = currentProject?.GetSettings();

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

		/// <summary>
		/// Remove the settings from .sdlproj's SettingsGroup 
		/// </summary>
		public void RemoveLanguageMappingSettings()
		{
			var currentProject = GetCurrentProject();
			var projectSettings = currentProject?.GetSettings();

			if (projectSettings != null)
			{
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(Constants.SettingsGroupId);

				if (containsSettingsGroup)
				{
					projectSettings.RemoveSettingsGroup(Constants.SettingsGroupId);
					currentProject.UpdateSettings(projectSettings);
				}
			}
		}

		/// <summary>
		/// Update the current project with the settings for Language Mappings options
		/// </summary>
		public void SaveLanguageMappingSettings(LanguageMappingSettings settings)
		{
			GetCurrentProject()?.UpdateSettings(_settingsGroup.SettingsBundle);
		}

		/// <summary>
		/// Get the current project
		/// </summary>
		/// <returns>Current project</returns>
		private static FileBasedProject GetCurrentProject()
		{
			return AppInitializer.GetProjectController()?.CurrentProject;
		}
	}
}