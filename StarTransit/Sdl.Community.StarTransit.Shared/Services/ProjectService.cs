using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Import;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class ProjectService
	{
		private readonly List<StarTranslationMemoryMetadata> _penaltiesTmsList;
		private readonly List<StarTranslationMemoryMetadata> _machineTransList;
		private TranslationProviderConfiguration _tmConfig;
		private MessageModel _messageModel;
		private readonly IFileTypeManager _fileTypeManager;
		private readonly ProjectsController _projectsController;
		private readonly List<ProjectFile> _targetProjectFiles;
		
		public ProjectService(IFileTypeManager fileTypeManager)
		{
			_fileTypeManager = fileTypeManager;

			var helpers = new Helpers();
			_projectsController = helpers.GetProjectsController();

			_messageModel = new MessageModel();
			_penaltiesTmsList = new List<StarTranslationMemoryMetadata>();
			_machineTransList = new List<StarTranslationMemoryMetadata>();
			_tmConfig = new TranslationProviderConfiguration();
			_targetProjectFiles = new List<ProjectFile>();
		}

		#region Public Methods

		public virtual IProject CreateNewProject(ProjectInfo projectInfo, ProjectTemplateReference projectTemplateReference)
		{
			if(projectInfo is null)
			{
				return null;
			}
			var newProject = new FileBasedProject(projectInfo, projectTemplateReference);
			return newProject;
		}

		public virtual MessageModel UpdateProjectSettings(IProject project)
		{
			var fileBasedProject = ((FileBasedProject)project);
			UpdateTmSettings(project);
			fileBasedProject.UpdateTranslationProviderConfiguration(_tmConfig);

			project.RunAutomaticTask(_targetProjectFiles?.GetIds(), AutomaticTaskTemplateIds.Scan);
			var taskSequence = fileBasedProject.RunAutomaticTasks(_targetProjectFiles?.GetIds(), new[]
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
				_messageModel.IsProjectCreated = false;
				_messageModel.Message = "Project could not be created.Error occured while running automatic tasks!";
				_messageModel.Title = "Informative message";
				return _messageModel;
			}
			fileBasedProject.Save();
			return _messageModel;
		}

		public MessageModel CreateProject(PackageModel package)
		{
			try
			{
				var target = GetTargetLanguages(package.LanguagePairs);

				var projectInfo = new ProjectInfo
				{
					Name = package.Name,
					LocalProjectFolder = package.Location,
					SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
					TargetLanguages = target,
					DueDate = package.DueDate
				};

				var newProject = CreateNewProject(projectInfo, new ProjectTemplateReference(package.ProjectTemplate.Uri));
				
				if (package.Customer != null)
				{
					((FileBasedProject)newProject).SetCustomer(package.Customer);
				}

				//Add StarTransit package source files. The same on all language pairs
				newProject.AddFiles(package.LanguagePairs[0].SourceFile.ToArray());

				//set the file role(user to display project details in Studio view)
				var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
				newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);

				_tmConfig = newProject.GetTranslationProviderConfiguration();

				_messageModel = SetLanguagePairInformation(newProject, package);

				if (_messageModel is null || _messageModel.Message is null)
				{
					if (Directory.Exists(newProject?.GetProjectInfo()?.LocalProjectFolder))
					{
						CreateMetadataFolder(package.Location, package.PathToPrjFile);
						_projectsController?.RefreshProjects();
					}

					if (_messageModel != null)
					{
						_messageModel.IsProjectCreated = true;
						_messageModel.Message = "Project was successfully created!";
						_messageModel.Title = "Informative message";
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateModel method: {ex.Message}\n {ex.StackTrace}");
				return null;
			}
			return _messageModel;
		}
		#endregion

		#region Private Methods

		private MessageModel SetLanguagePairInformation(
			IProject newProject,
			PackageModel package)
		{
			foreach (var pair in package.LanguagePairs)
			{				
				foreach (var starTmMetadata in pair.StarTranslationMemoryMetadatas)
				{
					AddTmPenalties(package, starTmMetadata);
					AddMtMemories(package, starTmMetadata);					
				}

				// Remove found items from pair.StarTranslationMemoryMetadatas (the remained ones are those which does not have penalties set on them)
				foreach (var item in _penaltiesTmsList)
				{
					pair.StarTranslationMemoryMetadatas.Remove(item);
				}

				// Remove Machine Translation memories from pair.StarTranslationMemoryMetadatas, if the user requests them, they will be imported separately, but never in the main TM
				pair.StarTranslationMemoryMetadatas.RemoveAll(item => Path.GetFileName(item?.TargetFile ?? "").Contains("_AEXTR_MT_"));

				_targetProjectFiles?.Clear();

				// Import language pair TM if any
				ImportLanguagePairTm(pair, newProject);

				if (!pair.TargetFile.Any() || pair.TargetFile.Count == 0)
				{
					_messageModel.IsProjectCreated = false;
					_messageModel.Message = "Project was not created correctly because no target files were found in the package!";
					_messageModel.Title = "Informative message";
					return _messageModel;
				}

				_targetProjectFiles?.AddRange(newProject.AddFiles(pair.TargetFile.ToArray()));
				_messageModel = UpdateProjectSettings(newProject);				
			}
			_projectsController?.RefreshProjects();
			return _messageModel;
		}
		
		private void ImportLanguagePairTm(LanguagePair pair, IProject project)
		{
			if (pair.HasTm && !string.IsNullOrEmpty(pair.TmPath))
			{
				if (pair.StarTranslationMemoryMetadatas.Count > 0)
				{
					var newTmPath = Path.Combine(project?.GetProjectInfo()?.LocalProjectFolder, Path.GetFileName(pair.TmPath));
					var importer = new TransitTmImporter(pair, _fileTypeManager, newTmPath);

					foreach (var tm in pair.StarTranslationMemoryMetadatas)
					{
						importer.ImportStarTransitTm(tm.TargetFile);
					}
					_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(importer.GetTranslationProviderReference(), true, true, true));
				}

				// Create separate TM for each TM file on which user set penalty. The penalty is applied on top of any penalty that might be applied by the translation provider itself.
				// (the name of the new TM will be the same with the one from StarTransit package)
				foreach (var item in _penaltiesTmsList)
				{
					var tpReference = CreateTpReference(item, pair, project);
					_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(tpReference, true, true, true, item.TMPenalty));
				}

				//If the user requests it, create a separate TM for the Machine Translation coming from Transit.
				foreach (var item in _machineTransList)
				{
					var tpReference = CreateTpReference(item, pair, project);

					//It should have a penalty set by default, otherwise it will be used for pretranslation and later added to the main TM when updating main TM, and we want to avoid that.
					_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(tpReference, true, true,true,1));
				}
			}
		}

		// Create translation provider reference
		private TranslationProviderReference CreateTpReference(StarTranslationMemoryMetadata item, LanguagePair pair, IProject project)
		{
			var importer = CreateTmImporter(item, pair, project);
			var tpReference = new TranslationProviderReference(importer.TMFilePath);
			return tpReference;
		}

		// Create the translation memory importer
		private TransitTmImporter CreateTmImporter(StarTranslationMemoryMetadata item, LanguagePair pair, IProject project)
		{
			var importer = new TransitTmImporter(_fileTypeManager, pair, project?.GetProjectInfo()?.LocalProjectFolder, Path.GetFileName(item.TargetFile));
			importer.ImportStarTransitTm(item.TargetFile);
			return importer;
		}

		// Separate all items from package.TMPenalties(files that are having penalties set), that are found in pair.StarTranslationMemoryMetadatas
		private void AddTmPenalties(PackageModel package, StarTranslationMemoryMetadata starTmMetadata)
		{
			if (package.TMPenalties != null)
			{
				if (package.TMPenalties.Any(t => t.Key.Equals(starTmMetadata.TargetFile)))
				{
					starTmMetadata.TMPenalty = package.TMPenalties.FirstOrDefault(t => t.Key.Equals(starTmMetadata.TargetFile)).Value;
					_penaltiesTmsList.Add(starTmMetadata);
				}
			}
		}

		//Separate all items from package.MachineTransMem (files that contain Machine Translation)
		private void AddMtMemories(PackageModel package, StarTranslationMemoryMetadata starTmMetadata)
		{
			if (package.MTMemories != null)
			{
				var hasMtMemories = package.MTMemories.Any(t => t.Equals(starTmMetadata.TargetFile));
				if (hasMtMemories)
				{
					_machineTransList.Add(starTmMetadata);
				}
			}
		}

		private Language[] GetTargetLanguages(List<LanguagePair> languagePairs)
		{
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"GetTargetLanguages method: {ex.Message}\n {ex.StackTrace}");
			}
			return new Language[] { };
		}

		// Update the translation memory settings
		private void UpdateTmSettings(IProject project)
		{
			try
			{
				var settings = project.GetSettings();
				var updateTmSettings = settings.GetSettingsGroup<TranslationMemoryUpdateTaskSettings>();
				if (updateTmSettings != null)
				{
					updateTmSettings.TmImportOptions.Value = TmImportOption.AlwaysAddNewTranslation;
					updateTmSettings.UpdateWithApprovedSignOffSegments.Value = true;
					updateTmSettings.UpdateWithApprovedTranslationSegments.Value = true;
					updateTmSettings.UpdateWithTranslatedSegments.Value = true;
					project.UpdateSettings(settings);
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"UpdateTmSettings method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Creates a folder named "StarTransitMetadata"and save the PRJ file in it.
		/// </summary>
		/// <param name="studioProjectPath"></param>
		/// <param name="prjFilePath"></param>
		private void CreateMetadataFolder(string studioProjectPath, string prjFilePath)
		{
			try
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
			catch (Exception ex)
			{
				Log.Logger.Error($"CreateMetadataFolder method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		#endregion
	}
}