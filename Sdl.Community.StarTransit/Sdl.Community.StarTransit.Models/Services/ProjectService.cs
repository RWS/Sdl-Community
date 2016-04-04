using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.ProjectAutomation.Core;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.FileBased;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class ProjectService
    {
        public void CreateProject(PackageModel package)
        {
            var projectInfo = new ProjectInfo
            {
                Name = package.Name,
                LocalProjectFolder = @"C:\Users\aghisa\Desktop\folder7",
                SourceLanguage = new Language(CultureInfo.GetCultureInfo("en-US")),
                TargetLanguages = new Language[] { new Language(CultureInfo.GetCultureInfo("de-DE")), new Language(CultureInfo.GetCultureInfo("fr-FR")) }

            };

            var newProject = new FileBasedProject(projectInfo);
            ProjectFile[] projectFiles = newProject.AddFiles(new[] { @"C:\Users\aghisa\Desktop\Utils.txt" });
            newProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            TaskSequence taskSequence = newProject.RunDefaultTaskSequence(projectFiles.GetIds());
            newProject.Save();
          
        }
    }
}
