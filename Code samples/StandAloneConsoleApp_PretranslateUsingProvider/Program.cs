using System;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace StandAloneConsoleApp_PretranslateUsingProvider
{
    public class Program
    {
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
            var projectInfo = new ProjectInfo
            {
                Name = "",
                SourceLanguage = new Language(""),
                TargetLanguages = new[] { new Language("") },
                LocalProjectFolder = $@"PathWhereProjectsWillBeSaved\{GetDateTimeToString(DateTime.Now)}"
            };

            var project = new FileBasedProject(projectInfo);

            UpdateProjectProviderSettings(project);

            var tpConfig = project.GetTranslationProviderConfiguration();

            var tpUriString = "";
            var tpReference = new TranslationProviderReference(new Uri(tpUriString), null, true);
            var tpCascadeEntry = new TranslationProviderCascadeEntry(tpReference, true, true, false);
            tpConfig.Entries.Add(tpCascadeEntry);
            project.UpdateTranslationProviderConfiguration(tpConfig);

            var apiKey = "";
            project.Credentials.AddCredential(new Uri(tpUriString), apiKey);
            project.Save();

            var projFiles =
                project.AddFiles(
                    new[] { @"filepaths" });

	        //var projFiles = project.AddFolderWithFiles(@"", true);

            project.RunAutomaticTasks(projFiles.GetIds(), new[]
            {
                AutomaticTaskTemplateIds.Scan,
                AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
                AutomaticTaskTemplateIds.CopyToTargetLanguages,
                AutomaticTaskTemplateIds.PreTranslateFiles,
            });

            project.Save();
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