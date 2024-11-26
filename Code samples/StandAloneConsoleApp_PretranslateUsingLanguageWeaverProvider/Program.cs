using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using System;
using System.IO;

namespace StandAloneConsoleApp_PretranslateUsingLanguageWeaverProvider
{
    public class Program
    {
        private static void AddFilesToProject(IProject project, string sourceFilesDirectory)
        {
            project.AddFolderWithFiles(sourceFilesDirectory, true);

            var projectFiles = project.GetSourceLanguageFiles();

            var scanResult = project.RunAutomaticTask(
                projectFiles.GetIds(),
                AutomaticTaskTemplateIds.Scan
            );

            Console.WriteLine("Scan: " + scanResult.Status);

            var files = project.GetSourceLanguageFiles();

            for (var i = 0; i < project.GetSourceLanguageFiles().Length; i++)
            {
                Guid[] currentFileId = { files[i].Id };

                var currentFileName = files[i].Name;
                project.SetFileRole(currentFileId, FileRole.Translatable);
            }
        }

        private static string GetDateTimeToString(DateTime dateTime)
        {
            var value = dateTime.Year +
                        dateTime.Month.ToString().PadLeft(2, '0') +
                        dateTime.Day.ToString().PadLeft(2, '0') +
                        "-" +
                        dateTime.Hour.ToString().PadLeft(2, '0') +
                        dateTime.Minute.ToString().PadLeft(2, '0') +
                        dateTime.Second.ToString().PadLeft(2, '0');
            return value;
        }

        private static string GetOutputProjectDirectory(string baseFolder)
        {
            var dateTimeString = GetDateTimeToString(DateTime.Now);
            var projectDirectory = Path.Combine(baseFolder, dateTimeString);

            if (!Directory.Exists(projectDirectory))
                Directory.CreateDirectory(projectDirectory);

            return projectDirectory;
        }


        private static void Main(string[] args)
        {
            var projectsDirectory = @"C:\Users\aflorescu\Code\Projects";
            var templatePath = @"C:\Users\aflorescu\Documents\Studio 2022\Project Templates\Studio22LWEdge.sdltpl";

            var sourceLanguage = "en-US";
            var targetLanguage = "de-DE";

            if (!Directory.Exists(projectsDirectory))
                Directory.CreateDirectory(projectsDirectory);
            var project = CreateProject(projectsDirectory, sourceLanguage, targetLanguage, templatePath);

            UpdateTranslationConfiguration(project);

            //var tpUriString = "languageweavercloud:///";
            var tpUriString = "languageweaveredge:///";
            if (string.IsNullOrWhiteSpace(templatePath)) AddTranslationProviderToProject(templatePath, project, tpUriString);

            var cloudCredentials = new
            {
                AccountRegion = "eu",
                ClientID = "",
                ClientSecret = ""
            };

            var edgeCredentials = new
            {
                AccountRegion = "eu",
                Host = "https://mt01.edge.languageweaver.com",
                ApiKey = "u_aflorescu@sdl.com_6wFrCx71NVm6"
            };


            project.Credentials.AddCredential(new Uri(tpUriString), JsonConvert.SerializeObject(edgeCredentials));

            project.Save();

            AddFilesToProject(project, @"C:\Users\aflorescu\Code\Samples\SampleFiles\Translatable");

            RunTasks(project, targetLanguage);

            project.Save();
        }

        private static void AddTranslationProviderToProject(string templatePath, FileBasedProject project,
            string tpUriString)
        {
            var tpConfig = project.GetTranslationProviderConfiguration();

            var tpReference = new TranslationProviderReference(new Uri(tpUriString), null, true);
            var tpCascadeEntry = new TranslationProviderCascadeEntry(tpReference, true, true, false);
            tpConfig.Entries.Add(tpCascadeEntry);
            project.UpdateTranslationProviderConfiguration(tpConfig);


        }

        private static FileBasedProject CreateProject(string projectsDirectory, string sourceLanguage, string targetLanguage,
            string templatePath)
        {
            var projectDirectory = GetOutputProjectDirectory(projectsDirectory);

            var projectName = "TestProject-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" +
                              DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;

            var projectInfo = new ProjectInfo
            {
                Name = projectName,
                SourceLanguage = new Language(sourceLanguage),
                TargetLanguages = new[] { new Language(targetLanguage) },
                LocalProjectFolder = projectDirectory
            };

            FileBasedProject project;
            if (!string.IsNullOrEmpty(templatePath))
            {
                var projectTemplateReference = new ProjectTemplateReference(templatePath);
                project = new FileBasedProject(projectInfo, projectTemplateReference);
            }
            else project = new FileBasedProject(projectInfo);

            return project;
        }

        private static void RunAnalyzeTaskFiles(IProject project, string targetLanguage)
        {
            var targetFiles = project.GetTargetLanguageFiles(Sdl.Core.Globalization.LanguageRegistry.LanguageRegistryApi.Instance.GetLanguage(targetLanguage));

            var task = project.RunAutomaticTask(
                targetFiles.GetIds(),
                AutomaticTaskTemplateIds.AnalyzeFiles
            );

            Console.WriteLine("RunAnalyzeTaskFiles: " + task.Status);
        }

        private static void RunPreTranslateFiles(IProject project, string targetLanguage)
        {
            var targetFiles = project.GetTargetLanguageFiles(Sdl.Core.Globalization.LanguageRegistry.LanguageRegistryApi.Instance.GetLanguage(targetLanguage));

            var task = project.RunAutomaticTask(
                targetFiles.GetIds(),
                AutomaticTaskTemplateIds.PreTranslateFiles);

            Console.WriteLine("RunPreTranslateFiles: " + task.Status);
        }

        private static void RunScanTaskFiles(IProject project)
        {
            var sourceFiles = project.GetSourceLanguageFiles();
            var sourceFilesIds = sourceFiles.GetIds();

            var scanTask = project.RunAutomaticTask(
                sourceFiles.GetIds(),
                AutomaticTaskTemplateIds.Scan);

            Console.WriteLine("RunScanTaskFiles: " + scanTask.Status);

            var convertTask = project.RunAutomaticTask(
                sourceFilesIds,
                AutomaticTaskTemplateIds.ConvertToTranslatableFormat);

            Console.WriteLine("RunConvertTaskFiles: " + convertTask.Status);

            var splitTask = project.RunAutomaticTask(
                sourceFilesIds,
                AutomaticTaskTemplateIds.CopyToTargetLanguages);

            Console.WriteLine("RunSplitTaskFiles: " + splitTask.Status);
        }

        private static void RunTasks(FileBasedProject project, string targetLanguage)
        {
            RunScanTaskFiles(project);
            RunAnalyzeTaskFiles(project, targetLanguage);
            RunPreTranslateFiles(project, targetLanguage);
        }

        private static void UpdateTranslationConfiguration(FileBasedProject project)
        {
            var settings = project.GetSettings();
            var preTranslateSettings = settings.GetSettingsGroup<TranslateTaskSettings>();
            preTranslateSettings.NoTranslationMemoryMatchFoundAction.Value = NoTranslationMemoryMatchFoundAction.ApplyAutomatedTranslation;
            preTranslateSettings.MinimumMatchScore.Value = 75;

            project.UpdateSettings(settings);
            project.Save();
        }
    }
}