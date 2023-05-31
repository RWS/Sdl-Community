using System;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace StandAloneConsoleApp_UpdateMainTranslationMemories
{
	public class Program
	{
		public static void GetUpdateTmTaskSettings(FileBasedProject project)
		{
			var settings = project.GetSettings();
			var updateTmSettings = settings.GetSettingsGroup<TranslationMemoryUpdateTaskSettings>();

			updateTmSettings.TmImportOptions.Value = TmImportOption.MergeTranslation;

			updateTmSettings.UpdateWithApprovedSignOffSegments.Value = true;
			updateTmSettings.UpdateWithApprovedTranslationSegments.Value = true;
			updateTmSettings.UpdateWithTranslatedSegments.Value = true;

			updateTmSettings.UpdateWithDraftSegments.Value = false;
			updateTmSettings.UpdateWithRejectedSignOffSegments.Value = false;
			updateTmSettings.UpdateWithRejectedTranslationSegments.Value = false;
			updateTmSettings.UpdateWithUnspecifiedSegments.Value = false;

			project.UpdateSettings(settings);
		}

		private static void Main(string[] args)
		{
			var project = new FileBasedProject(@"<projectFilePath>");
			GetUpdateTmTaskSettings(project);

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
	}
}