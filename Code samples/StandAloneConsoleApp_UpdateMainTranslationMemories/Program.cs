using System;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace StandAloneConsoleApp_UpdateMainTranslationMemories
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var project = new FileBasedProject(@"<projectFilePath>");

			UpdateProjectProviderSettings(project);

			var tpConfig = project.GetTranslationProviderConfiguration();

			var tpUriString = @"sdltm.file:///<TMPath>";
			var tpReference = new TranslationProviderReference(new Uri(tpUriString), null, true);
			var tpCascadeEntry = new TranslationProviderCascadeEntry(tpReference, true, true, false);
			tpConfig.Entries.Add(tpCascadeEntry);
			project.UpdateTranslationProviderConfiguration(tpConfig);

			project.RunAutomaticTasks(project.GetTargetLanguageFiles().GetIds(), new[]
			{
				AutomaticTaskTemplateIds.UpdateMainTranslationMemories
			});

			project.Save();
		}

		private static void UpdateProjectProviderSettings(FileBasedProject project)
		{
			var settings = project.GetSettings();
			var preTranslateSettings = settings.GetSettingsGroup<TranslateTaskSettings>();
			preTranslateSettings.NoTranslationMemoryMatchFoundAction.Value = NoTranslationMemoryMatchFoundAction.ApplyAutomatedTranslation;
			preTranslateSettings.MinimumMatchScore.Value = 75;

			project.UpdateSettings(settings);
			project.Save();
		}
	}
}