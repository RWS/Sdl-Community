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
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
    public class ProjectService : AbstractViewControllerAction<ProjectsController>
    {

        public void CreateProject(PackageModel package)
        {
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

           //Add StarTransit package source files. The same on all language pairs
            ProjectFile[] sourceProjectFiles = newProject.AddFiles(package.LanguagePairs[0].SourceFile.ToArray());

            var tmConfig = newProject.GetTranslationProviderConfiguration();
            var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);

            List<ProjectFile> targetProjectFiles = new List<ProjectFile>();
          foreach (var pair in package.LanguagePairs)
            {
                //import language pair TM if any
                if (pair.HasTm)
                {
                    var importer = new TransitTmImporter(pair.SourceLanguage,
                           pair.TargetLanguage,
                           pair.CreateNewTm,
                           fileTypeManager,
                           pair.TmPath);
                    foreach (var tm in pair.StarTranslationMemoryMetadatas)
                    {
                       
                         importer.ImportStarTransitTm(tm.TargetFile);

                       
                    }
                    tmConfig.Entries.Add(new TranslationProviderCascadeEntry(importer.GeTranslationProviderReference(),
                           true,
                           true,
                           true));
                }
               targetProjectFiles.AddRange(newProject.AddFiles(pair.TargetFile.ToArray()));
            }

            newProject.RunAutomaticTask(targetProjectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
            var taskSequence = newProject.RunAutomaticTasks(targetProjectFiles.GetIds(), new string[]
            {
                    AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
                    AutomaticTaskTemplateIds.CopyToTargetLanguages,
                    AutomaticTaskTemplateIds.PerfectMatch,
                    AutomaticTaskTemplateIds.PreTranslateFiles,
                    AutomaticTaskTemplateIds.AnalyzeFiles,
                   
            });

            
            newProject.UpdateTranslationProviderConfiguration(tmConfig);
            newProject.Save();
           // var controller = Controller;
            Controller.RefreshProjects();
            CreateMetadataFolder(package.Location, package.PathToPrjFile);

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

        /// <summary>
        /// Creates a folder named "StarTransitMetadata"and save the PRJ file in it.
        /// </summary>
        /// <param name="studioProjectPath"></param>
        /// <param name="prjFilePath"></param>
        private void CreateMetadataFolder(string studioProjectPath, string prjFilePath)
        {
            var starTransitMetadataFolderPath = Path.Combine(studioProjectPath, "StarTransitMetadata");

            if (!Directory.Exists(starTransitMetadataFolderPath))
            {
                Directory.CreateDirectory(starTransitMetadataFolderPath);
            }

            var prjFileName = Path.GetFileName(prjFilePath);
            if (prjFileName != null)
            {
                File.Copy(prjFilePath, Path.Combine(starTransitMetadataFolderPath, prjFileName), true);
            }
            
        }

        protected override void Execute()
        {
            
        }
    }
}
