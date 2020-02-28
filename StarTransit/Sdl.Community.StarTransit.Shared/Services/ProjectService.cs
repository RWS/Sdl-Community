using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Import;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.Desktop.IntegrationApi;
using Sdl.FileTypeSupport.Framework.Core.Utilities.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class ProjectService : AbstractViewControllerAction<ProjectsController>
	{
		#region Public Methods
		public MessageModel CreateProject(PackageModel package)
		{
			var penaltiesTmsList = new List<StarTranslationMemoryMetadata>();
			var machineTransList = new List<StarTranslationMemoryMetadata>();
			var target = GetTargetLanguages(package.LanguagePairs);

			var projectInfo = new ProjectInfo
			{
				Name = package.Name,
				LocalProjectFolder = package.Location,
				SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
				TargetLanguages = target,
				DueDate = package.DueDate
			};

			var newProject = new FileBasedProject(projectInfo,
				new ProjectTemplateReference(package.ProjectTemplate.Uri));
			if (package.Customer != null)
			{
				newProject.SetCustomer(package.Customer);
			}

			//Add StarTransit package source files. The same on all language pairs
			ProjectFile[] sourceProjectFiles = newProject.AddFiles(package.LanguagePairs[0].SourceFile.ToArray());

			//set the file role(user to display project details in Studio view)
			var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
			newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);

			var tmConfig = newProject.GetTranslationProviderConfiguration();
			var fileTypeManager = DefaultFileTypeManager.CreateInstance(true);

			var targetProjectFiles = new List<ProjectFile>();
			foreach (var pair in package.LanguagePairs)
			{
				// Separate all items from package.TMPenalties(files that are having penalties set)
				// which are found in pair.StarTranslationMemoryMetadatas
				foreach (var starTMMetadata in pair.StarTranslationMemoryMetadatas)
				{
					if (package.TMPenalties != null)
					{
						if (package.TMPenalties.Any(t => t.Key.Equals(starTMMetadata.TargetFile)))
						{
							starTMMetadata.TMPenalty = package.TMPenalties.FirstOrDefault(t => t.Key.Equals(starTMMetadata.TargetFile)).Value;
							penaltiesTmsList.Add(starTMMetadata);
						}
					}
					//Separate all items from package.MachineTransMem (files that contain Machine Translation)
					if (package.MachineTransMem != null)
					{
						if (package.MachineTransMem.Any(t => t.Equals(starTMMetadata.TargetFile)))
						{
							machineTransList.Add(starTMMetadata);
						}
					}
				}

				// Remove found items from pair.StarTranslationMemoryMetadatas (the remained ones are those which does not have penalties set on them)
				foreach (var item in penaltiesTmsList)
				{
					pair.StarTranslationMemoryMetadatas.Remove(item);
				}

				// Remove Machine Translation memories from pair.StarTranslationMemoryMetadatas, if the user requests them, they will be imported separately, but never in the main TM
				pair.StarTranslationMemoryMetadatas.RemoveAll(item => Path.GetFileName(item.TargetFile).Contains("_MT_"));

				// Create one TM (that has the name equals with the project name) 
				// with the TM files from pair.StarTranslationMemoryMetadatas (the ones without penalties set by the user)
				targetProjectFiles.Clear();

				// Import language pair TM if any
				if (pair.HasTm)
				{
					if (pair.StarTranslationMemoryMetadatas.Count > 0)
					{
						var importer = new TransitTmImporter(pair.SourceLanguage,
							pair.TargetLanguage,
							pair.CreateNewTm,
							fileTypeManager,
							Path.Combine(Path.GetDirectoryName(newProject.FilePath), Path.GetFileName(pair.TmPath)));
						foreach (var tm in pair.StarTranslationMemoryMetadatas)
						{
							importer.ImportStarTransitTm(tm.TargetFile);
						}

						tmConfig.Entries.Add(new TranslationProviderCascadeEntry(importer.GetTranslationProviderReference(),
							true,
							true,
							true));
					}

					// Create separate TM for each TM file on which user set penalty
					// (the name of the new TM will be the same with the one from StarTransit package)
					foreach (var item in penaltiesTmsList)
					{
						var tmWithPenaltyImporter = new TransitTmImporter(
							Path.GetDirectoryName(newProject.FilePath),
							pair.SourceLanguage,
							pair.TargetLanguage,
							Path.GetFileName(item.TargetFile),
							fileTypeManager);

						tmWithPenaltyImporter.ImportStarTransitTm(item.TargetFile);

						tmConfig.Entries.Add(new TranslationProviderCascadeEntry(
							new TranslationProviderReference(tmWithPenaltyImporter.TMFilePath),
							true,
							true,
							true,
							item.TMPenalty));
					}

					//If the user requests it, create a separate TM for the Machine Translation coming from Transit.
					foreach (var item in machineTransList)
					{
						var MachineTranslationImporter = new TransitTmImporter(
							Path.GetDirectoryName(newProject.FilePath),
							pair.SourceLanguage,
							pair.TargetLanguage,
							Path.GetFileName(item.TargetFile),
							fileTypeManager);

						MachineTranslationImporter.ImportStarTransitTm(item.TargetFile);

						//It should have a penalty set by default, otherwise it will be used for pretranslation and later added to the main TM when updating main TM, and we want to avoid that.
						tmConfig.Entries.Add(new TranslationProviderCascadeEntry(
							new TranslationProviderReference(MachineTranslationImporter.TMFilePath),
							true,
							true,
							true,
							1));
					}
				}
				if (!pair.TargetFile.Any() || pair.TargetFile.Count == 0)
				{
					var messageModel = new MessageModel
					{
						IsProjectCreated = false,
						Message = "Project was not created correctly because no target files were found in the package!",
						Title = "Informative message"
					};
					return messageModel;
				}
				targetProjectFiles.AddRange(newProject.AddFiles(pair.TargetFile.ToArray()));

				//update tm settings
				UpdateTmSettings(newProject);
				newProject.UpdateTranslationProviderConfiguration(tmConfig);

				newProject.RunAutomaticTask(targetProjectFiles.GetIds(), AutomaticTaskTemplateIds.Scan);
				var taskSequence = newProject.RunAutomaticTasks(targetProjectFiles.GetIds(), new[]
				{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages,
					AutomaticTaskTemplateIds.PerfectMatch,
					AutomaticTaskTemplateIds.PreTranslateFiles,
					AutomaticTaskTemplateIds.AnalyzeFiles,
					AutomaticTaskTemplateIds.UpdateMainTranslationMemories

				});

				if (taskSequence.Status.Equals(TaskStatus.Failed))
				{
					var messageModel = new MessageModel
					{
						IsProjectCreated = false,
						Message = "Project could not be created.Error occured while running automatic tasks!",
						Title = "Informative message"
					};
					return messageModel;
				}

				newProject.Save();
			}
			Controller.RefreshProjects();
			if (Directory.Exists(Path.GetDirectoryName(newProject.FilePath)))
			{
				CreateMetadataFolder(package.Location, package.PathToPrjFile);

				Controller.RefreshProjects();
			}
			return null;
		}
		#endregion

		#region Private Methods
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

		private void UpdateTmSettings(FileBasedProject project)
		{
			var settings = project.GetSettings();
			var updateTmSettings = settings.GetSettingsGroup<TranslationMemoryUpdateTaskSettings>();
			updateTmSettings.TmImportOptions.Value = TmImportOption.AlwaysAddNewTranslation;
			updateTmSettings.UpdateWithApprovedSignOffSegments.Value = true;
			updateTmSettings.UpdateWithApprovedTranslationSegments.Value = true;
			updateTmSettings.UpdateWithTranslatedSegments.Value = true;

			project.UpdateSettings(settings);
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
		#endregion
	}
}