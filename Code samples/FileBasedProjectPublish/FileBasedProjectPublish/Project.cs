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
        public void Execute()
        {
            var groupShareServer = "http://gs2017dev.sdl.com ";

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
                TargetLanguages = new Language[] { new Language("fr-FR") }
            };

            // create new project and add a translatable file to it from the local machine
            var fileBasedProject = new FileBasedProject(projectInfo);
            var projectFiles = fileBasedProject.AddFiles(new string[] { Path.Combine(Path.GetTempPath(), "TestFile.docx") });
            fileBasedProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            var taskSequence = fileBasedProject.RunDefaultTaskSequence(projectFiles.GetIds());

            if (taskSequence.Status == TaskStatus.Completed)
            {
                fileBasedProject.Save();

                //// publish project to GS using Windows credentials
                //var result = fileBasedProject.PublishProject(
                //    new Uri(groupShareServer),
                //    true,
                //    "windows userName",
                //    "windows password",
                //    "GroupShare organization",
                //    (sender, args) => { });
                // publish project to GS using Windows credentials
                var result = fileBasedProject.PublishProject(
                    new Uri(groupShareServer),
                    true,
                    "SDLCommunity",
                    "Commun1tyRocks",
                    "/TestAPI",
                    (sender, args) => { });
            }
            else
            {
                //taskSequence status can be used to see the errors in case the project was not created successfully
            }
        }
    }
}