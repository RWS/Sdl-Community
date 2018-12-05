using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using System;
using System.IO;

namespace FileBasedProjectPublish
{
    [ApplicationInitializer]
    public class Project : IApplicationInitializer
    {
		// application will thorw an error when trying to publishing the project to Group Share using Windows credentials,
		// because of an exiting bug in Studio API(the issue was raised to Studio team)
        public void Execute()
        {	  
			var groupShareServer = "GS server address";

			var projectFolder = Path.Combine(Path.GetTempPath(), "ProjectTemp");
			if (!Directory.Exists(projectFolder))
			{
				Directory.CreateDirectory(projectFolder);
			}

			var projectInfo = new ProjectInfo
			{
				Name = "GSProjectName",
				SourceLanguage = new Language("en-US"),
				LocalProjectFolder = projectFolder,
				TargetLanguages = new[] { new Language("fr-FR") }
			};

			// create new project and add a translatable file to it from the local machine
			var fileBasedProject = new FileBasedProject(projectInfo);
			var projectFiles = fileBasedProject.AddFiles(new[] { Path.Combine(Path.GetTempPath(), "TestFile.docx") });
			fileBasedProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
			var taskSequence = fileBasedProject.RunDefaultTaskSequence(projectFiles.GetIds());

			if (taskSequence.Status == TaskStatus.Completed)
			{
				//project publishing on GS will fail if GetProjectStatistics is called
				//this bug was raised to Studio team
				//fileBasedProject.GetProjectStatistics();

				fileBasedProject.Save();

				// publish project to GS using Windows credentials
				var result = fileBasedProject.PublishProject(
					new Uri(groupShareServer),
					true,
					"windows userName",
					"windows password",
					"GroupShare organization",
					(sender, args) => { });
			}
			else
			{
				//taskSequence status can be used to see the errors in case the project was not created successfully
			}
		}	  
	}
}