using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
                LocalProjectFolder = package.Location,
                SourceLanguage = new Language(package.SourceLanguage),
                TargetLanguages = new Language[] {new Language(package.TargetLanguage)}
            };

            var newProject = new FileBasedProject(projectInfo);
            // ProjectFile[] projectFiles =newProject.AddFiles(new[] {@"C:\Users\aghisa\Desktop\Utils.txt"});
            ProjectFile[] projectFiles = newProject.AddFiles(package.Files);
            newProject.RunAutomaticTask(projectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            TaskSequence taskSequence = newProject.RunDefaultTaskSequence(projectFiles.GetIds());
            newProject.Save();

            DeleteFilesFromTemp(package.Files);
        }

        private static void DeleteFilesFromTemp(IEnumerable<string> files)
        {
            try
            {
                foreach (var file in files)
                {
                    File.Delete(file);
                }
                
            }catch(Exception e) { }
        }
    }
}
