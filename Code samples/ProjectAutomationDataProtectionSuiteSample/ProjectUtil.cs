using System;
using System.Collections.Generic;
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
		public string ProjectName { get; set; }
		public string InputFolder { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
		public string OutputFolder { get; set; }
	}

	internal static class ProjectUtil
	{
		public static LocalProjectSettings DefaultSettings()
		{
			var uniqueId = DateTime.Now.Ticks.ToString();
			var inputFolder = "C:\\john\\buff\\test-data-protection";	
			var outputFolder = "c:\\john\\buff\\test-" + uniqueId;
			return new LocalProjectSettings
			{
				ProjectName = "Test-" + uniqueId, // unique name
				InputFolder = inputFolder,
				SourceLanguage = "en-US",
				TargetLanguage = "de-DE",
				OutputFolder = outputFolder,
			};
		}

		public static FileBasedProject CreateProject(LocalProjectSettings settings)
		{
			try
			{
				// Create new project object
				var createdProject = new FileBasedProject(
					GetProjectInfo(
						settings.ProjectName,
						new Language(settings.SourceLanguage),
						new Language[] { new Language(settings.TargetLanguage) }, settings.OutputFolder));

				// Add files from selected folder
				createdProject.AddFolderWithFiles(settings.InputFolder, true);

				// Start the tasks
				RunTasks(createdProject, settings);

				createdProject.Save();
				// project is saved but not listed in Studio, this is by design.
				return createdProject;
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



		private static void CheckEvents(List<TaskStatusEventArgs> taskStatusEventArgsList, List<MessageEventArgs> messageEventArgsList)
		{
			// task statuses and messages can be iterated and any problems can be reported
			foreach (var item in taskStatusEventArgsList)
			{
				switch (item.Status)
				{
					case TaskStatus.Assigned:
						break;
					case TaskStatus.Cancelled:
						break;
					case TaskStatus.Cancelling:
						break;
					case TaskStatus.Completed:
						break;
					case TaskStatus.Created:
						break;
					case TaskStatus.Failed:
						break;
					case TaskStatus.Invalid:
						break;
					case TaskStatus.Rejected:
						break;
					case TaskStatus.Started:
						break;
					default:
						break;
				}
			}

			// at the end clear task statuses and messages
			taskStatusEventArgsList.Clear();
			messageEventArgsList.Clear();
		}

		private static AutomaticTask RunTasks(
			FileBasedProject createdProject,
			ProjectFile[] projectFiles,
			string taskIDToRun,
			List<TaskStatusEventArgs> taskStatusEventArgsList,
			List<MessageEventArgs> messageEventArgsList)
		{
			AutomaticTask task = createdProject.RunAutomaticTask(
				projectFiles.GetIds(),
				taskIDToRun,
				(sender, taskStatusArgs) =>
				{
					taskStatusEventArgsList.Add(taskStatusArgs);
				},
				(sender, messageArgs) =>
				{
					messageEventArgsList.Add(messageArgs);
				});

			return task;
		}

		private static void RunTasks(FileBasedProject createdProject, LocalProjectSettings settings)
		{
			Language targetLanguage = new Language(settings.TargetLanguage);
			List<TaskStatusEventArgs> taskStatusEventArgsList = new List<TaskStatusEventArgs>();
			List<MessageEventArgs> messageEventArgsList = new List<MessageEventArgs>();

			// set up  perfect match
			ProjectFile[] projectFiles = createdProject.GetSourceLanguageFiles();
			createdProject.AddBilingualReferenceFiles(GetBilingualFileMappings(new Language[] { targetLanguage }, projectFiles, settings.OutputFolder));

			// scan files
			AutomaticTask automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.Scan,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);

			// convert files
			automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);

			// copy files to target languages
			automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.CopyToTargetLanguages,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);

			// from now on use target language files
			projectFiles = createdProject.GetTargetLanguageFiles(targetLanguage);

			// Apply Perfect Match
			automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.PerfectMatch,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);

			// analyze files
			automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.AnalyzeFiles,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);

			// translate files
			automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.PreTranslateFiles,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);

			// populate project TM
			automaticTask = RunTasks(
				createdProject,
				projectFiles,
				AutomaticTaskTemplateIds.PopulateProjectTranslationMemories,
				taskStatusEventArgsList,
				messageEventArgsList);

			CheckEvents(taskStatusEventArgsList, messageEventArgsList);
		}

	}
}
