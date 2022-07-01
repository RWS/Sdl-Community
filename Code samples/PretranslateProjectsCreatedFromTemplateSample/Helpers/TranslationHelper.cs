using Pretranslate_Template2;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace PretranslateProjectsCreatedFromTemplateSample.Helpers
{
	public static class TranslationHelper
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="projectPath"></param>
		/// <param name="projectCopyPath"></param>
		/// <param name="credentials"></param>
		/// <returns></returns>
		public static AutomaticTask RunPretranslation(string projectPath, string projectCopyPath = null, Credentials credentials = null)
		{
			var project = new FileBasedProject(projectPath);

			if (credentials is not null) UpdateProjectProviderSettings(project, credentials);

			var projectFiles = project.GetTargetLanguageFiles();
			var task = project.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.PreTranslateFiles);

			project.Save();

			return task;
		}

		private static void UpdateProjectProviderSettings(FileBasedProject project, Credentials credentials)
		{
			var settings = project.GetSettings();
			var preTranslateSettings = settings.GetSettingsGroup<TranslateTaskSettings>();
			preTranslateSettings.NoTranslationMemoryMatchFoundAction.Value = NoTranslationMemoryMatchFoundAction.ApplyAutomatedTranslation;
			preTranslateSettings.MinimumMatchScore.Value = 75;

			project.UpdateSettings(settings);
			project.Credentials.AddCredential(credentials.Uri, credentials.ApiKey);

			project.Save();
		}
	}
}