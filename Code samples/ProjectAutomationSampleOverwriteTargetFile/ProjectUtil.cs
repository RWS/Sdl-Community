using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace ProjectAutomationDataProtectionSuiteSample
{
	public class LocalProjectSettings
	{
		public string ProjectName { get; set; } = "";
        public string InputFolder { get; set; } = "";
        public string SourceLanguage { get; set; } = "";
        public IReadOnlyList<string> TargetLanguages { get; set; } = new List<string>();
        public string OutputFolder { get; set; } = "";
        public string TranslationMemory { get; set; } = "";
    }

	internal static class ProjectUtil
    {
        public const string TEMP_FOLDER_ROOT = "C:\\john\\buff\\";

        public static LocalProjectSettings ProjectSettings(string singleFile, string sourceLanguage, string targetLanguage)
        {
            return ProjectSettings(singleFile, sourceLanguage, new[] { targetLanguage });
        }
        public static LocalProjectSettings ProjectSettings(string singleFile, string sourceLanguage, IReadOnlyList<string> targetLanguages)
        {
            var uniqueId = DateTime.Now.Ticks.ToString();
            var inputFolder = $"{TEMP_FOLDER_ROOT}temp-{uniqueId}-in";
            var outputFolder = $"{TEMP_FOLDER_ROOT}temp-{uniqueId}-out";
            Directory.CreateDirectory(inputFolder);
            Directory.CreateDirectory(outputFolder);
			File.Copy(singleFile, $"{inputFolder}\\{Path.GetFileName(singleFile)}");
            return new LocalProjectSettings
            {
				ProjectName = $"project-{uniqueId}",
				InputFolder = inputFolder, 
                OutputFolder = outputFolder, 
                SourceLanguage = sourceLanguage, 
                TargetLanguages = targetLanguages,
            };
        }
        // the whole project is created in a temporary folder you can later delete
        public static LocalProjectSettings TempProjectSettings(string singleFile, string sourceLanguage, IReadOnlyList<string> targetLanguages)
        {
            var destPath = $"{Path.GetTempPath()}trados-temp-{DateTime.Now.Ticks}\\";
            Directory.CreateDirectory(destPath);

            var uniqueId = DateTime.Now.Ticks.ToString();
            var inputFolder = $"{destPath}temp-in";
            var outputFolder = $"{destPath}temp-out";

            Directory.CreateDirectory(inputFolder);
            Directory.CreateDirectory(outputFolder);
            File.Copy(singleFile, $"{inputFolder}\\{Path.GetFileName(singleFile)}");
            return new LocalProjectSettings
            {
                ProjectName = $"project-{uniqueId}",
                InputFolder = inputFolder,
                OutputFolder = outputFolder,
                SourceLanguage = sourceLanguage,
                TargetLanguages = targetLanguages,
            };
        }


        public static FileBasedProject CreateProject(LocalProjectSettings settings)
		{
			try
			{
				// Create new project object
				var project = new FileBasedProject(
					GetProjectInfo(
						settings.ProjectName,
						new Language(settings.SourceLanguage),
                        settings.TargetLanguages.Select(l => new Language(l)).ToArray(), 
                        settings.OutputFolder));

				if ( settings.TranslationMemory != "")
                    SetTranslationMemory(project, settings);

                // Add files from selected folder
                project.AddFolderWithFiles(settings.InputFolder, true);

				AddBilingualFileMappings(project, settings);

				project.Save();
				// project is saved but not listed in Studio, this is by design.
				return project;
			}
			catch (Exception ex)
			{
				throw new Exception("Problem during project creation", ex);
			}
		}

		private static ProjectInfo GetProjectInfo(string projectName, Language sourceLang, Language[] targetLangs, string path)
		{
			ProjectInfo newProjectInfo = new ProjectInfo()
			{
				Name = projectName,
				CreatedBy = "API automation",
				Description = "Project created by API",
				DueDate = DateTime.Now.AddDays(7),
				SourceLanguage = sourceLang,
				TargetLanguages = targetLangs,
				LocalProjectFolder = path
			};

			return newProjectInfo;
		}

		private static BilingualFileMapping[] GetBilingualFileMappings(Language[] targetLanguages, ProjectFile[] translatableFiles, string previousProjectPath)
		{
			List<BilingualFileMapping> mappings = new List<BilingualFileMapping>();
			foreach (Language language in targetLanguages)
			{
				string searchPath = Path.Combine(previousProjectPath, language.IsoAbbreviation);
				foreach (ProjectFile file in translatableFiles)
				{
					string previousFile = string.Concat(Path.Combine(searchPath, file.Name), (file.Name.EndsWith(".sdlxliff") ? "" : ".sdlxliff"));
					if (File.Exists(previousFile))
					{
						BilingualFileMapping mapping = new BilingualFileMapping()
						{
							BilingualFilePath = previousFile,
							Language = language,
							FileId = file.Id
						};
						mappings.Add(mapping);
					}
				}
			}
			return mappings.ToArray();
		}



        private static void AddBilingualFileMappings(FileBasedProject createdProject, LocalProjectSettings settings)
        {
            foreach (var language in settings.TargetLanguages)
            {
                Language targetLanguage = new Language(language);

                // set up  perfect match
                ProjectFile[] projectFiles = createdProject.GetSourceLanguageFiles();
                createdProject.AddBilingualReferenceFiles(GetBilingualFileMappings(new Language[] { targetLanguage }, projectFiles, settings.OutputFolder));
            }
        }


        public static void SetTranslationMemory(FileBasedProject project, LocalProjectSettings sett)
        {
			Debug.Assert(sett.TranslationMemory != "");
            var translationConfig = project.GetTranslationProviderConfiguration();
            translationConfig.Entries.Add(new TranslationProviderCascadeEntry(sett.TranslationMemory, true, true, true, 0));
            project.UpdateTranslationProviderConfiguration(translationConfig);
            project.Save();
        }

        public static bool VerifyAutomaticTask(AutomaticTask result)
        {
            foreach (var msg in result.Messages)
                Console.WriteLine($"*** ERROR: {result.Name} - {msg.Message} - {msg.Exception?.Message}");
            // the idea - any message means something went wrong
            return (result.Messages.Length == 0);
        }

        public static bool TryRunSourceTaskAndAnalyze(FileBasedProject project, string taskId)
        {
            var result = project.RunAutomaticTask(project.GetSourceLanguageFiles().Select(f => f.Id).ToArray(), taskId);
			foreach (var msg in result.Messages)
                Console.WriteLine($"*** ERROR: {taskId} - {msg.Message} - {msg.Exception?.Message}");
            // the idea - any message means something went wrong
            return (result.Messages.Length == 0);
        }

        public static bool TryRunTargetTaskAndAnalyze(FileBasedProject project, string taskId)
        {
            var result = project.RunAutomaticTask(project.GetTargetLanguageFiles().Select(f => f.Id).ToArray(), taskId);
            foreach (var msg in result.Messages)
                Console.WriteLine($"*** ERROR: {taskId} - {msg.Message} - {msg.Exception?.Message}");
            // the idea - any message means something went wrong
            return (result.Messages.Length == 0);
        }

        // just in case I want the file I'm adding to a project to have a desired name
        public static string GetDesiredTempFileName(string originalFileName, string desiredTempName)
        {
            var destPath = $"{Path.GetTempPath()}{DateTime.Now.Ticks}\\";
            Directory.CreateDirectory(destPath);
            var destName = $"{destPath}{Path.GetFileName(desiredTempName)}.sdlxliff";
            File.Copy(originalFileName, destName);
            return destName;
        }

    }
}
