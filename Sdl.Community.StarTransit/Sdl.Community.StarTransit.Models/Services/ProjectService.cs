using System;
using System.CodeDom;
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
            var target = GetTargetLanguages(package.TargetLanguage);

            var projectInfo = new ProjectInfo
            {
                Name = package.Name,
                LocalProjectFolder = package.Location,
                SourceLanguage = new Language(package.SourceLanguage),
                TargetLanguages = target
            };

            var newProject = new FileBasedProject(projectInfo, new ProjectTemplateReference(package.ProjectTemplate.Uri));

          
            ProjectFile[] sourceProjectFiles = newProject.AddFiles(package.SourceFiles);
            var targetProjectFiles = newProject.AddFiles(package.TargetFiles);
            
            newProject.RunAutomaticTask(targetProjectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            var taskSequence = newProject.RunAutomaticTasks(targetProjectFiles.GetIds(), new string[]
            {
                AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
                AutomaticTaskTemplateIds.CopyToTargetLanguages,
                AutomaticTaskTemplateIds.PerfectMatch,
                AutomaticTaskTemplateIds.PreTranslateFiles,
                AutomaticTaskTemplateIds.AnalyzeFiles
            }, StatusHandler, MessageHandler);
            
            newProject.Save();

            DeleteFilesFromTemp(package.SourceFiles);
            DeleteFilesFromTemp(package.TargetFiles);
        }

        private void MessageHandler(object sender, TaskMessageEventArgs e)
        {
            var x = e.Message;
        }

        private void StatusHandler(object sender, TaskStatusEventArgs e)
        {
            var x = e.Status;
        }

       
        private Language[] GetTargetLanguages(List<CultureInfo> languages)
        {
            var targetLanguageList = new List<Language>();
            foreach (var target in languages)
            {
                var language = new Language(target);
                targetLanguageList.Add(language);
            }

            var targetLanguages = new Language[targetLanguageList.Count];
            for (var i = 0; i < targetLanguageList.Count; i++)
            {
                targetLanguages[i] = targetLanguageList[i];
            }

            return targetLanguages;
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
