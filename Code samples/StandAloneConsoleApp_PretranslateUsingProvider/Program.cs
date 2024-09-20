using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using System;
using StandAloneConsoleApp_PretranslateUsingProvider.LC;
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

            // Change the path with the actual project template path
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

            // Update this variable with the actual URI of the translation provider.
            // You can retrieve this value from an existing project or project template (.sdlproj/.sdltpl).
            // To do this, open the project file in a text editor of your choice.
            // Look for the XML element <CascadeEntryProvider> and its child elements.
            // These elements should contain the URI for the translation provider.
            //
            // For example:
            // - URIs starting with "languagecloud.translation.https://" indicate a Translation Engine.
            //
            // Copy the complete URI from the project/template file and assign it to the variable below.
            //
            // Example URI: 
            // If a project template is used, you would typically need to map the URI from the project template file to the actual URI.
            // For the sake of this example, change this value with the actual tpUri string.
            // This is because, when the project automation api is used, the credentials need to be provided explicitly as it works outside the context of Trados Studio.
            var tpUriString = @""; // Paste URI here

            // The API key can be obtained from the Language Cloud web interface.
            // Navigate to: Users -> Integrations -> API Keys.
            // Copy the API key from this section.
            var apiKey = ""; // Paste the API Key here

            if (string.IsNullOrEmpty(templatePath))
            {
                var tpConfig = project.GetTranslationProviderConfiguration();

                var tpReference = new TranslationProviderReference(new Uri(tpUriString), null, true);
                var tpCascadeEntry = new TranslationProviderCascadeEntry(tpReference, true, true, false);
                tpConfig.Entries.Add(tpCascadeEntry);
                project.UpdateTranslationProviderConfiguration(tpConfig);
            }

            project.Credentials.AddCredential(new Uri(tpUriString), apiKey);

            project.Save();

            // Update the second parameter with the actual path to the source files.
            // Example: @"C:\Path\To\SourceFiles"
            AddFilesToProject(project, @""); // Paste the source files path

            //Login to LC for projects with Translation Engine providers 
            var lcService = new LCService();
            lcService.LoginToLC();

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