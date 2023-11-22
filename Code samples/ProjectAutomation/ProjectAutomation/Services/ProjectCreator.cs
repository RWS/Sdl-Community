using System;
using System.Globalization;
using System.IO;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;

namespace ProjectAutomation.Services
{
	public class ProjectCreator : IDisposable
	{
		public bool Create(string sourceFilesDirectory, string projectDirectory, string sourceLanguage, string targetLanguage, MemoryResource memory)
		{
			if (string.IsNullOrEmpty(sourceLanguage) || string.IsNullOrEmpty(targetLanguage))
			{
				return false;
			}

			try
			{
				var projectInfo = GetProjectInformation(sourceLanguage, targetLanguage, projectDirectory);
				var project = new FileBasedProject(projectInfo);

				if (memory != null)
				{
					AddTranslationProvider(project, memory);
				}

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
		
		private static void AddTranslationProvider(FileBasedProject project, MemoryResource memory)
		{
		
			var provider = GetTranslationProviderReference(memory);
			if (provider == null)
			{
				return;
			}

			var entry = new TranslationProviderCascadeEntry(provider, false, true, false, 0);

			var tpConfig = project.GetTranslationProviderConfiguration();

			// remove any existing entries, if needed!
			tpConfig.Entries.Clear();

			tpConfig.Entries.Add(entry);
			project.UpdateTranslationProviderConfiguration(tpConfig);

			if (memory.Uri != null && !string.IsNullOrEmpty(memory.UserNameOrClientId) &&
			    !string.IsNullOrEmpty(memory.UserPasswordOrClientSecret))
			{
				project.Credentials.AddCredential(memory.Uri, false,
					memory.UserNameOrClientId, memory.UserPasswordOrClientSecret);
			}

			UpdateTranslateSettings(project);

			project.Save();
		}

		private static void UpdateTranslateSettings(FileBasedProject project)
		{
			var settings = project.GetSettings();
			var translateSettings = settings.GetSettingsGroup<TranslateTaskSettings>();
			translateSettings.NoTranslationMemoryMatchFoundAction.Value =
				NoTranslationMemoryMatchFoundAction.ApplyAutomatedTranslation;
			translateSettings.TranslationOverwriteMode.Value = TranslationUpdateMode.OverwriteExistingTranslation;
			project.UpdateSettings(settings);
		}

		private static TranslationProviderReference GetTranslationProviderReference(MemoryResource memory)
		{
			TranslationProviderReference provider = null;
			if (!string.IsNullOrEmpty(memory.Path) && File.Exists(memory.Path))
			{
				provider = new TranslationProviderReference(memory.Path);
			}
			else if (memory.Uri != null && !string.IsNullOrEmpty(memory.UserNameOrClientId) &&
			         !string.IsNullOrEmpty(memory.UserPasswordOrClientSecret))
			{
				provider = new TranslationProviderReference(memory.Uri, null, true);
			}

			return provider;
		}

		private void RunAnalyzeTaskFiles(IProject project, string targetLanguage)
		{
			var targetFiles = project.GetTargetLanguageFiles(new Language(CultureInfo.GetCultureInfo(targetLanguage)));

			var task = project.RunAutomaticTask(
				targetFiles.GetIds(),
				AutomaticTaskTemplateIds.AnalyzeFiles
			);

			Console.WriteLine("RunAnalyzeTaskFiles: " + task.Status);
		}

		private void RunPreTranslateFiles(IProject project, string targetLanguage)
		{
			var targetFiles = project.GetTargetLanguageFiles(new Language(CultureInfo.GetCultureInfo(targetLanguage)));

			var task = project.RunAutomaticTask(
				targetFiles.GetIds(),
				AutomaticTaskTemplateIds.PreTranslateFiles);

			Console.WriteLine("RunPreTranslateFiles: " + task.Status);
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

					Console.WriteLine("ConvertToTranslatableFormatTask: " + convertToTranslatableFormatTask.Status);

					var copyToTargetLanguagesTask = project.RunAutomaticTask(
						currentFileId,
						AutomaticTaskTemplateIds.CopyToTargetLanguages
					);

					Console.WriteLine("CopyToTargetLanguagesTask: " + copyToTargetLanguagesTask.Status);
				}
			}
		}

		
		public void Dispose()
		{
		}
	}
}
