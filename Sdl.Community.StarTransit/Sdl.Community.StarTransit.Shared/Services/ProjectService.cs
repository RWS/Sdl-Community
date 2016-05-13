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
using System.Threading.Tasks;
using Sdl.Community.StarTransit.Shared.Import;
using Sdl.Desktop.IntegrationApi;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Community.StarTransit.Shared.Utils;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class ProjectService : AbstractViewControllerAction<ProjectsController>
    {

        public void CreateProject(PackageModel package)
        {
            var importer = new TransitTmImporter();
            foreach (var pair in package.LanguagePairs)
            {
                var selectedTm = pair.TmPath;
                foreach (var tm in pair.StarTranslationMemoryMetadatas)
                {
                    importer.CreateSdlXliffFile(tm.TargetFile, selectedTm);
                }

            }
            
            var target = GetTargetLanguages(package.LanguagePairs);
           
            var projectInfo = new ProjectInfo
            {
                Name = package.Name,
                LocalProjectFolder = package.Location,
                 SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
                 TargetLanguages = target,
                DueDate = package.DueDate,

            };

            var newProject = new FileBasedProject(projectInfo,
                new ProjectTemplateReference(package.ProjectTemplate.Uri));
            if (package.Customer != null)
            {
                newProject.SetCustomer(package.Customer);
            }

           
            ProjectFile[] sourceProjectFiles = newProject.AddFiles(package.LanguagePairs[0].SourceFile.ToArray());
            var targetProjectFiles = newProject.AddFiles(package.LanguagePairs[0].TargetFile.ToArray());

            newProject.RunAutomaticTask(targetProjectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            var taskSequence = newProject.RunAutomaticTasks(targetProjectFiles.GetIds(), new string[]
            {
                    AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
                    AutomaticTaskTemplateIds.CopyToTargetLanguages,
                    AutomaticTaskTemplateIds.PerfectMatch,
                    AutomaticTaskTemplateIds.PreTranslateFiles,
                    AutomaticTaskTemplateIds.AnalyzeFiles
            });

            newProject.Save();
            var controller = Controller;
            controller.RefreshProjects();


        }

        private Language[] GetTargetLanguages(List<LanguagePair> languagePairs)
        {
            var targetCultureInfoList = new List<CultureInfo>();
            foreach (var pair in languagePairs)
            {
                var targetLanguage = pair.TargetLanguage;
                targetCultureInfoList.Add(targetLanguage);
            }
            var targetLanguageList = new List<Language>();
            foreach (var target in targetCultureInfoList)
            {
                var language = new Language(target);
                targetLanguageList.Add(language);
            }

            var targetArray = targetLanguageList.ToArray();

            return targetArray;
        }



        protected override void Execute()
        {
            
        }
    }
}
