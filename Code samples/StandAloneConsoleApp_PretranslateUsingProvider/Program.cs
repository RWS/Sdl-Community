using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using System;
using System.IO;

namespace StandAloneConsoleApp_PretranslateUsingProvider
{
    public class Program
    {
        private static string GetOutputProjectDirectory(string baseFolder)
        {
            var dateTimeString = GetDateTimeToString(DateTime.Now);
            var projectDirectory = Path.Combine(baseFolder, dateTimeString);

            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            return projectDirectory;
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

        private static void Main(string[] args)
        {
            // Change the path with the actual directory where projects should be saved...
            var projectsDirectory = @"";

            // Specify the path to the project template that contains the Language Cloud Translation Engine...
            var templatePath = @"";

            if (!Directory.Exists(projectsDirectory))
            {
                Directory.CreateDirectory(projectsDirectory);
            }

            var sourceLanguage = "en-US";
            var targetLanguage = "de-DE";

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
                project = new FileBasedProject(projectInfo, new ProjectTemplateReference(templatePath));                
            }
            else
            {
                project = new FileBasedProject(projectInfo);
            }

            UpdateProjectProviderSettings(project);

            // Update this variable with the actual URI of the translation provider if available.
            // This URI can be retrieved from an existing project or project template (.sdlproj/.sdltpl).
            // To locate it, open the project/template file in a text editor and search for the <MainTranslationProviderItem> XML element.
            // Copy the value of its "uri" property, as this contains the URI for the translation provider.
            //
            // Note:
            // - If a project template is provided, this parameter can be left as an empty string.
            // - URIs starting with "languagecloud.translation.https://" indicate a Translation Engine.
            //
            // Copy the complete URI from the project/template file and assign it to the variable below.
            // Remember: When using the project automation API, credentials must be provided explicitly 
            //           since the API operates outside the context of Trados Studio.
            var tpUriString = @""; // Paste URI here

            if (string.IsNullOrEmpty(templatePath))
            {
                var tpConfig = project.GetTranslationProviderConfiguration();

                var tpReference = new TranslationProviderReference(new Uri(tpUriString), null, true);
                var tpCascadeEntry = new TranslationProviderCascadeEntry(tpReference, true, true, false);
                tpConfig.Entries.Add(tpCascadeEntry);
                project.UpdateTranslationProviderConfiguration(tpConfig);
            }

            project.Save();

            // Update the second parameter with the actual path to the source files.
            // Example: @"C:\Path\To\SourceFiles"
            AddFilesToProject(project, @""); // Paste the source files path

            RunScanTaskFiles(project);
            RunAnalyzeTaskFiles(project, targetLanguage);
            RunPreTranslateFiles(project, targetLanguage);

            project.Save();
        }

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

        private static void UpdateProjectProviderSettings(FileBasedProject project)
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