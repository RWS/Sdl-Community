using Sdl.Community.BeGlobalV4.Provider.Helpers;
using Sdl.Community.BeGlobalV4.Provider.Studio;
using Sdl.Core.Settings;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.BeGlobalV4.Provider.Service
{
	public class LanguageMappingsService
	{
		private Constants _constants = new Constants();
		private static ISettingsGroup _sdlMTCloudSettingsGroup;

		/// <summary>
		/// Get the saved language mapping settings from current project
		/// </summary>
		public LanguageMappingSettings GetLanguageMappingSettings()
		{
			var savedMappingSettings = new LanguageMappingSettings();
			var currentProject = GetCurrentProject() ?? AppInitializer.GetFileController()?.CurrentProject;
			var projectSettings = currentProject?.GetSettings();

			if (projectSettings != null)
			{
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(_constants.SettingsGrId);

				_sdlMTCloudSettingsGroup = projectSettings.GetSettingsGroup(_constants.SettingsGrId);

				if (!containsSettingsGroup)
				{
					projectSettings.AddSettingsGroup(_sdlMTCloudSettingsGroup);
				}
				savedMappingSettings = projectSettings.GetSettingsGroup<LanguageMappingSettings>(_constants.SettingsGrId);
			}
			return savedMappingSettings;
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
				var containsSettingsGroup = projectSettings.ContainsSettingsGroup(_constants.SettingsGrId);

				if (containsSettingsGroup)
				{
					projectSettings.RemoveSettingsGroup(_constants.SettingsGrId);
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