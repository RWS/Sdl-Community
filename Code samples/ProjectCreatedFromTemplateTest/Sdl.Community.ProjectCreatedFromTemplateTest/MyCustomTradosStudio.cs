using System;
using System.Collections.Generic;
using System.Globalization;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi.Extensions;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

namespace Sdl.Community.ProjectCreatedFromTemplateTest
{
	[Action("ProjectCreatedFromTemplateTest", Name = "ProjectCreatedFromTemplateTest")]
	[ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
	public class MyCustomTradosStudio : AbstractAction
	{
		protected override void Execute()
		{
			var template = new ProjectTemplateReference(@"");

			var projectInfo = new ProjectInfo
			{
				Name = "ProjectCreatedFromTemplateTest",
				SourceLanguage = new Language(CultureInfo.GetCultureInfo("en-US")), //import Sdl.core.globalization.dll
				TargetLanguages = new[]
				{
					new Language(CultureInfo.GetCultureInfo("de-DE")),
					new Language(CultureInfo.GetCultureInfo("fr-FR"))
				},
				//file path where you want to save the project
				LocalProjectFolder = @""
			};

			//import Sdl.ProjectAutomation.FileBased
			var fileBasedProject = new FileBasedProject(projectInfo, template);

			//HERE YOU NEED TO ADD THE PATH FOR FILES YOU WANT TO INCLUDE IN YOUT PROJECT
			var filesPath = new[] { @"" };

			//add files to project
			var projectFiles = fileBasedProject.AddFiles(filesPath);
			//we need to run automatic task to create the project in studio
			fileBasedProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
			var taskSequence = fileBasedProject.RunDefaultTaskSequence(projectFiles.GetIds());
			if (taskSequence.Status == TaskStatus.Completed)
			{
				//project was created succesfully
			}
			else
			{
				//here we'll see the erors
			}

			fileBasedProject.Save();
		}
	}
}
