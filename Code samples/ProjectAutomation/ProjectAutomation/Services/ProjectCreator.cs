using System;
using System.Globalization;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace ProjectAutomation.Services
{
	public class ProjectCreator : IDisposable
	{
		public bool Create(string sourceFilesDirectory, string projectDirectory, string sourceLanguage, string targetLanguage, string tmPath)
		{
			if (string.IsNullOrEmpty(sourceLanguage) || string.IsNullOrEmpty(targetLanguage))
			{
				return false;
			}

			try
			{
				var projectInfo = GetProjectInformation(sourceLanguage, targetLanguage, projectDirectory);
				var project = new FileBasedProject(projectInfo);

				AddTranslationProvider(project, tmPath);

				AddFilesToProject(project, sourceFilesDirectory);

				ConvertFiles(project);

				RunAnalyzeTaskFiles(project, targetLanguage);
				RunPreTranslateFiles(project, targetLanguage);

				project.Save();

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return false;
		}


		private void RunAnalyzeTaskFiles(IProject project, string targetLanguage)
		{
			var targetFiles = project.GetTargetLanguageFiles(new Language(CultureInfo.GetCultureInfo(targetLanguage)));

			var task = project.RunAutomaticTask(
				targetFiles.GetIds(),
				AutomaticTaskTemplateIds.AnalyzeFiles
			);
		}

		private void RunPreTranslateFiles(IProject project, string targetLanguage)
		{
			var settings = project.GetSettings();
			var translateSettings = settings.GetSettingsGroup<TranslateTaskSettings>();
			translateSettings.NoTranslationMemoryMatchFoundAction.Value = NoTranslationMemoryMatchFoundAction.ApplyAutomatedTranslation;
			translateSettings.TranslationOverwriteMode.Value = TranslationUpdateMode.OverwriteExistingTranslation;

			project.UpdateSettings(settings);

			var targetFiles = project.GetTargetLanguageFiles(new Language(CultureInfo.GetCultureInfo(targetLanguage)));

			var task = project.RunAutomaticTask(
				targetFiles.GetIds(),
				AutomaticTaskTemplateIds.PreTranslateFiles);
		}

		private static ProjectInfo GetProjectInformation(string sourceLanguage, string targetLanguage, string projectDirectory)
		{
			var info = new ProjectInfo
			{
				Name = "TestProject-" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second,
				LocalProjectFolder = projectDirectory
			};

			var srcLang = new Language(CultureInfo.GetCultureInfo(sourceLanguage));
			info.SourceLanguage = srcLang;

			var trgLang = new[] { new Language(CultureInfo.GetCultureInfo(targetLanguage)) };
			info.TargetLanguages = trgLang;

			return info;
		}

		private void AddTranslationProvider(FileBasedProject project, string path)
		{
			var tmConfig = project.GetTranslationProviderConfiguration();
			var provider = new TranslationProviderReference(path);
			var entry = new TranslationProviderCascadeEntry(provider, false, true, true, 0);

			// remove any existing entries, if needed!
			tmConfig.Entries.Clear();

			tmConfig.Entries.Add(entry);
			project.UpdateTranslationProviderConfiguration(tmConfig);

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

			var files = project.GetSourceLanguageFiles();

			for (var i = 0; i < project.GetSourceLanguageFiles().Length; i++)
			{
				Guid[] currentFileId = { files[i].Id };

				var currentFileName = files[i].Name;
				project.SetFileRole(currentFileId, FileRole.Translatable);
			}
		}

		private static void ConvertFiles(IProject project)
		{
			var files = project.GetSourceLanguageFiles();

			for (var i = 0; i < project.GetSourceLanguageFiles().Length; i++)
			{
				if (files[i].Role == FileRole.Translatable)
				{
					Guid[] currentFileId = { files[i].Id };
					var convertToTranslatableFormatTask = project.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.ConvertToTranslatableFormat
					);

					var copyToTargetLanguagesTask = project.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.CopyToTargetLanguages
					);
				}
			}
		}

		
		public void Dispose()
		{
		}
	}
}
