using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.Desktop.IntegrationApi.Extensions;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using System;
using System.IO;
using Sdl.TranslationStudioAutomation.IntegrationApi.Presentation.DefaultLocations;

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

	// same thing done as a button in project context menu; and with a template used for the project creation
	// code comented as the two do almost the same thing and should be used separately
	//[Action("CreateAndPublishGSProject")]
 //   [ActionLayout(typeof(TranslationStudioDefaultContextMenus.ProjectsContextMenuLocation))]
 //   public class CreateGsProjectUsingTemplate : AbstractAction
 //   {
 //       protected override void Execute()
 //       {
 //           //plugin which uses project template to create a new server
 //           var groupShareServer = "[serverUri]";

 //           var projectFolder = Path.Combine(Path.GetTempPath(), "ProjectTemp");
 //           if (!Directory.Exists(projectFolder))
 //           {
 //               Directory.CreateDirectory(projectFolder);
 //           }

 //           var projectInfo = new ProjectInfo
 //           {
 //               Name = "",
 //               LocalProjectFolder = projectFolder,
 //           };

 //           // create new project and add a translatable file to it from the local machine
 //           //using following template
 //           var projectTemplate = new ProjectTemplateReference(
 //               @"[pathToTemplate]");
 //           var fileBasedProject = new FileBasedProject(projectInfo, projectTemplate);
 //           var projectFiles = fileBasedProject.AddFiles(new[] {@"[pathToATestFile]"});

 //           fileBasedProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
 //           var taskSequence = fileBasedProject.RunDefaultTaskSequence(projectFiles.GetIds());

 //           if (taskSequence.Status == TaskStatus.Completed)
 //           {
 //               fileBasedProject.Save();

 //               // publish project to GS using Windows credentials
 //               var result = fileBasedProject.PublishProject(
 //                   new Uri(groupShareServer),
 //                   false,
 //                   "",
 //                   "",
 //                   "",
 //                   null);
 //           }
 //           else
 //           {
 //               //taskSequence status can be used to see the errors in case the project was not created successfully
 //           }
 //       }
 //   }
}