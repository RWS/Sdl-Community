using Sdl.Community.MTCloud.Provider.Studio;
using Sdl.Core.Settings;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.MTCloud.Provider.Service
{
	public class LanguageMappingsService
	{		
		private static ISettingsGroup _sdlMTCloudSettingsGroup;

		/// <summary>
		/// Get the saved language mapping settings from current project
		/// </summary>
		public LanguageMappingSettings GetLanguageMappingSettings()
		{
			var currentProject = GetCurrentProject() ?? AppInitializer.GetFileController()?.CurrentProject;
			var projectSettings = currentProject?.GetSettings();

			if (projectSettings != null)
			{
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(Constants.SettingsGrId);

				_sdlMTCloudSettingsGroup = projectSettings.GetSettingsGroup(Constants.SettingsGrId);

				if (!containsSettingsGroup)
				{
					projectSettings.AddSettingsGroup(_sdlMTCloudSettingsGroup);
				}
				var savedMappingSettings = projectSettings.GetSettingsGroup<LanguageMappingSettings>(Constants.SettingsGrId);
				return savedMappingSettings;
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
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(Constants.SettingsGrId);

				if (containsSettingsGroup)
				{
					projectSettings.RemoveSettingsGroup(Constants.SettingsGrId);
					currentProject.UpdateSettings(projectSettings);
				}
			}
		}

		/// <summary>
		/// Update the current project with the settings for Language Mappings options
		/// </summary>
		public void SaveLanguageMappingSettings(LanguageMappingSettings settings)
		{
			GetCurrentProject()?.UpdateSettings(_sdlMTCloudSettingsGroup.SettingsBundle);
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