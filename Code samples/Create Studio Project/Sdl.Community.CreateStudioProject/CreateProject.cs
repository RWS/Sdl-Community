using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.DefaultLocations;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace Sdl.Community.CreateStudioProject
{
	[ApplicationInitializer]
	public class CreateProject : IApplicationInitializer
	{
		public void Execute()
		{
			//import Sdl.ProjectAutomation.Core.dll
			var projectInfo = new ProjectInfo
			{
				Name = "Project From Plugin",
				SourceLanguage = new Language(CultureInfo.GetCultureInfo("en-US")), //import Sdl.core.globalization.dll 
				TargetLanguages = new[]
				{
					new Language(CultureInfo.GetCultureInfo("de-DE")),
					new Language(CultureInfo.GetCultureInfo("fr-FR"))
				},
				//file path where you want to save the project
				LocalProjectFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) , @"Studio 2017\Projects\Project From Plugin")
			};

			//import Sdl.ProjectAutomation.FileBased
			var fileBasedProject = new FileBasedProject(projectInfo);

			//HERE YOU NEED TO ADD THE PATH FOR FILES YOU WANT TO INCLUDE IN YOUT PROJECT
			var filesPath = new[] {@""};

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

