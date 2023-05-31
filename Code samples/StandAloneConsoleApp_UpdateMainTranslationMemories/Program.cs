using System;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace StandAloneConsoleApp_UpdateMainTranslationMemories
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var project = new FileBasedProject(@"<projectFilePath>");
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