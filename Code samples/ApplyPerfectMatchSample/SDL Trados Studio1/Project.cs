using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace ApplyPerfectMatch
{
	[ApplicationInitializer]
	public class Project : IApplicationInitializer
	{
		public void Execute()
		{
			var projectInfo = new ProjectInfo
			{
				Name = "TestProject",
				SourceLanguage = new Language(CultureInfo.GetCultureInfo("en-US")), //import Sdl.core.globalization.dll
				TargetLanguages = new[] {new Language(CultureInfo.GetCultureInfo("de-DE")),},
				//file path where you want to save the project
				LocalProjectFolder = $@""
			};

			//import Sdl.ProjectAutomation.FileBased
			var fileBasedProject = new FileBasedProject(projectInfo);

			//HERE YOU NEED TO ADD THE PATH FOR FILES YOU WANT TO INCLUDE IN YOUR PROJECT
			var filesPath = new[] {@"C"};

			//add files to project
			var projectFiles = fileBasedProject.AddFiles(filesPath);
			//Translation provider configuration
			var translationProviderConfig = new TranslationProviderConfiguration();
			var entry = new TranslationProviderCascadeEntry(
				@"En-DeTM.sdltm", true, true, true);
			translationProviderConfig.Entries.Add(entry);
			fileBasedProject.UpdateTranslationProviderConfiguration(translationProviderConfig);
			//we need to run automatic task to create the project in studio
			fileBasedProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
			var taskSequence = fileBasedProject.RunAutomaticTasks(projectFiles.GetIds(),
				new[]
				{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages, AutomaticTaskTemplateIds.PerfectMatch,
					AutomaticTaskTemplateIds.PreTranslateFiles,
				});
			if (taskSequence.Status == TaskStatus.Completed)
			{
				//project was created succesfully
			}
			else
			{
				if (taskSequence.Status.Equals(TaskStatus.Failed))
				{
					foreach (var subTask in taskSequence.SubTasks)
					{
					}
				}
			}
			fileBasedProject.Save();
		}
	}
}
