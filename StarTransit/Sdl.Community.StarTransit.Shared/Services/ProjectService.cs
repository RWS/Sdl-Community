using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using Sdl.Community.StarTransit.Shared.Import;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using Sdl.TranslationStudioAutomation.IntegrationApi;
using Sdl.Versioning;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class ProjectService:IProjectService
	{
		private readonly List<StarTranslationMemoryMetadata> _penaltiesTmsList;
		private readonly List<StarTranslationMemoryMetadata> _machineTransList;
		private TranslationProviderConfiguration _tmConfig;
		private MessageModel _messageModel;
		private readonly ProjectsController _projectsController;
		private readonly string _iconPath;
		private readonly IFileService _fileService;
		private readonly IProjectsControllerService _projectControllerService;
		private readonly Language[] _targetLanguages;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _initialFolderPath;

		public ProjectService(Helpers helpers):this()
		{
			if (helpers == null) return;
			_projectsController = helpers.GetProjectsController();
			_iconPath = string.IsNullOrEmpty(_iconPath) ? helpers.GetIconPath() : _iconPath;
		}

		public ProjectService()
		{
			_fileService = new FileService();
			_messageModel = new MessageModel();
			_penaltiesTmsList = new List<StarTranslationMemoryMetadata>();
			_machineTransList = new List<StarTranslationMemoryMetadata>();
			_tmConfig = new TranslationProviderConfiguration();
			var studioVersion = new StudioVersionService().GetStudioVersion();
			_initialFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), studioVersion.StudioDocumentsFolderName, "Translation Memories");
		}

		public ProjectService(IProjectsControllerService projectsControllerService):this()
		{
			_projectControllerService = projectsControllerService;
			_iconPath = string.IsNullOrEmpty(_iconPath) ? GetProjectIconPath() : _iconPath;
		}

		public virtual IProject CreateNewProject(ProjectInfo projectInfo, ProjectTemplateReference projectTemplateReference)
		{
			if(projectInfo is null)
			{
				return null;
			}
			var newProject = new FileBasedProject(projectInfo, projectTemplateReference);
			return newProject;
		}

		public virtual MessageModel UpdateProjectSettings(IProject project,Guid[] targetFilesIds,bool projectContainsTm)
		{
			var fileBasedProject = (FileBasedProject)project;

			if (projectContainsTm)
			{
				UpdateTmSettings(project);
				fileBasedProject.UpdateTranslationProviderConfiguration(_tmConfig);
			}

			project.RunAutomaticTask(targetFilesIds, AutomaticTaskTemplateIds.Scan);
			var taskSequence = fileBasedProject.RunAutomaticTasks(targetFilesIds, new[]
			{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages,
					AutomaticTaskTemplateIds.PreTranslateFiles,
			});

			if (taskSequence.Status.Equals(TaskStatus.Failed))
			{
				_messageModel.IsProjectCreated = false;
				foreach (var subTask in taskSequence.SubTasks)
				{
					_logger.Error($"Name:{subTask.Name}");

					foreach (var messages in subTask.Messages)
					{
						_logger.Error($"Exception: {messages?.Exception}");
						_logger.Error($"Message: {messages?.Message}");
					}
				}
				_messageModel.Message = "Project could not be created.Error occured while running automatic tasks!";
				_messageModel.Title = "Informative message";
				return _messageModel;
			}
			fileBasedProject.Save();
			return _messageModel;
		}

		public IProject CreateStudioProject(PackageModel transitPackage)
		{
			var studioProject = PrepareStudioProject(transitPackage);
			foreach (var languagePair in transitPackage.LanguagePairs)
			{
				var projectContainsTm = false;
				if (!languagePair.NoTm)
				{
					_messageModel = ImportTms(transitPackage, languagePair);
					projectContainsTm = true;
				}

				var targetFilesForLanguagePair = studioProject.AddFiles(languagePair.TargetFile.ToArray());
				var filesIds = targetFilesForLanguagePair.GetIds();
				_messageModel = UpdateProjectSettings(studioProject, filesIds,projectContainsTm); // runs Studio batchtask to save all the changes to the project
			}
			CreateMetadataFolder(transitPackage.Location, transitPackage.PathToPrjFile);
			return null;
		}

		private IProject PrepareStudioProject(PackageModel transitPackage)
		{
			var target = _fileService.GetStudioTargetLanguages(transitPackage.LanguagePairs);
			Directory.CreateDirectory(transitPackage.Location); //if the location is set using project template feature this folder does not exist 
			var projectInfo = new ProjectInfo
			{
				Name = transitPackage.Name,
				LocalProjectFolder = transitPackage.Location,
				SourceLanguage = new Language(transitPackage.LanguagePairs[0].SourceLanguage),
				TargetLanguages = target,
				DueDate = transitPackage.DueDate,
				ProjectOrigin = Resources.ProjectOrigin,
				IconPath = _iconPath
			};
			var newProject = CreateNewProject(projectInfo, new ProjectTemplateReference(transitPackage.ProjectTemplate.Uri));

			if (transitPackage.Customer != null)
			{
				((FileBasedProject)newProject).SetCustomer(transitPackage.Customer);
			}

			//Add StarTransit package source files. The same on all language pairs
			newProject.AddFiles(transitPackage.LanguagePairs[0].SourceFile.ToArray());

			//set the file role(user to display project details in Studio view)
			var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
			newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);
			_tmConfig = newProject.GetTranslationProviderConfiguration();

			return newProject;
		}
		private MessageModel ImportTms(PackageModel package, LanguagePair languagePair)
		{
			if (!languagePair.HasTm || string.IsNullOrEmpty(package.Location))
				return null; //TODO: investigate what we need to return

			foreach (var metadataTransitFile in languagePair.SelectedTranslationMemoryMetadatas)
			{
				var tmDescription = $"{metadataTransitFile.Name} Translation Memory";
				metadataTransitFile.LocalTmCreationPath = GetTmPathForDuplicatedName(metadataTransitFile.LocalTmCreationPath, _tmConfig.Entries);

				var importer = new TransitTmImporter(languagePair, tmDescription, metadataTransitFile.LocalTmCreationPath,
					metadataTransitFile.TmPenalty);
				importer.ImportStarTransitTm(metadataTransitFile.TransitTmsSourceFilesPath,
					metadataTransitFile.TransitTmsTargeteFilesPath, metadataTransitFile.TmPenalty, package);
				var providerRef = importer.GetTranslationProviderReference(metadataTransitFile.LocalTmCreationPath, languagePair);
				_logger.Info($"-->Import lang pair Provider Reference:{providerRef?.Uri}");

				if (providerRef == null) return null;
				var entryExists =
					_tmConfig.Entries.Any(e => e.MainTranslationProvider.Uri.Equals(providerRef.Uri));
				if (!entryExists)
				{
					_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(providerRef, true, true, true, metadataTransitFile.TmPenalty));
				}
			}

			return null;

			//	foreach (var groupedTm in languagePair.GroupedTmsByPenalty)
			//	{
			//		var penalty = groupedTm.Key;
			//		var tmsList = groupedTm.ToList();

			//		if (languagePair.CreateNewTm)
			//		{
			//			// we need to import all the tms into onbly one tm
			//			if (penalty.Equals(0) && tmsList.Count > 1)
			//			{
			//				//here we'll use the language pair name
			//				var selectedLanguagePairName =
			//					$"{languagePair.SourceLanguage.TwoLetterISOLanguageName}-{languagePair.TargetLanguage.TwoLetterISOLanguageName}";
			//				languagePair.TmName = $"{package.Name}.{selectedLanguagePairName}.sdltm";
			//			}

			//			//here we'll use the penalty in the name of the tm
			//			if (penalty > 0 && tmsList.Count > 1)
			//			{
			//				languagePair.TmName = $"{package.Name}.Penalty{penalty}.sdltm";
			//			}

			//			//individual tms/mts which have the original star transit file name
			//			if (tmsList.Count.Equals(1))
			//			{
			//				languagePair.TmName = $"{Path.GetFileName(tmsList[0].TargetFile)}.sdltm";
			//			}

			//			languagePair.TmPath = Path.Combine(localProjectPath, languagePair.TmName);
			//		}
			//		var importer = new TransitTmImporter(languagePair, tmDescription, languagePair.TmPath, penalty);
			//		importer.ImportStarTransitTm(tmsList, package);
			//		var providerRef = importer.GetTranslationProviderReference(languagePair.TmPath, languagePair);
			//		_logger.Info($"-->Import lang pair Provider Reference:{providerRef?.Uri}");
			//		if (providerRef == null) return null;
			//		_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(providerRef, true, true, true, penalty));



			//		return null;
			//}
		}

		/// <summary>
		/// If project was created based on Transit Template on the project with the same name, TM created using "Create Option" should have the name followed by {number}
		/// In this way we can distinguish which tm was created for the current project and which one comes from the Project Template.
		/// <returns>New Tm Path with TM name followed by {number} if the name already exists. Otherwise the same tmPath</returns>
		/// </summary>
		private string GetTmPathForDuplicatedName(string tmPath, List<TranslationProviderCascadeEntry> cascadeEntries)
		{
			var tmName = Path.GetFileNameWithoutExtension(tmPath);
			var path = Path.GetDirectoryName(tmPath);
			var tmsForLangPairCount =
				cascadeEntries.Count(e => e.MainTranslationProvider.Uri.LocalPath.Contains(tmName));
			if (tmsForLangPairCount == 0) return tmPath;
			return !string.IsNullOrEmpty(path) ? Path.Combine(path, $"{tmName}_{tmsForLangPairCount}.sdltm") : tmPath;
		}

		// Update the translation memory settings
		private void UpdateTmSettings(IProject project)
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

		private string GetProjectIconPath()
		{
			var assemblyPath = Assembly.GetExecutingAssembly().Location;
			var directoryInfo = Directory.GetParent(assemblyPath);
			var targetPath = Path.Combine(directoryInfo.FullName, "transit.ico");

			using (var fs = new FileStream(targetPath, FileMode.Create))
			{
				Resources.transit.Save(fs);
			}
			return targetPath;
		}

		/// <summary>
		/// Creates a folder named "StarTransitMetadata"and save the PRJ file in it.
		/// </summary>
		private void CreateMetadataFolder(string studioProjectPath, string prjFilePath)
		{
			try
			{
				var starTransitMetadataFolderPath = Path.Combine(studioProjectPath, "StarTransitMetadata");

				Directory.CreateDirectory(starTransitMetadataFolderPath);

				var prjFileName = Path.GetFileName(prjFilePath);
				if (prjFileName != null)
				{
					File.Copy(prjFilePath, Path.Combine(starTransitMetadataFolderPath, prjFileName), true);
				}
			}
			catch (Exception ex)
			{
				_logger.Error($"{ex.Message}\n {ex.StackTrace}");
			}
		}

		//public (MessageModel, IProject) CreateProject(PackageModel package)
		//{
		//	var target = _fileService.GetStudioTargetLanguages(package.LanguagePairs);

		//	var projectInfo = new ProjectInfo
		//	{
		//		Name = package.Name,
		//		LocalProjectFolder = package.Location,
		//		SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
		//		TargetLanguages = target,
		//		DueDate = package.DueDate,
		//		ProjectOrigin = "Star Transit project",
		//		IconPath = _iconPath
		//	};

		//	var newProject = CreateNewProject(projectInfo, new ProjectTemplateReference(package.ProjectTemplate.Uri));

		//	if (package.Customer != null)
		//	{
		//		((FileBasedProject) newProject).SetCustomer(package.Customer);
		//	}

		//	//Add StarTransit package source files. The same on all language pairs
		//	newProject.AddFiles(package.LanguagePairs[0].SourceFile.ToArray());

		//	//set the file role(user to display project details in Studio view)
		//	var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
		//	newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);
		//	_tmConfig = newProject.GetTranslationProviderConfiguration();

		//	_messageModel = SetLanguagePairInformation(newProject, package);

		//	if (_messageModel?.Message is null)
		//	{
		//		if (Directory.Exists(newProject?.GetProjectInfo()?.LocalProjectFolder))
		//		{
		//			//TODo ar trebui mutat in metoda care efectiv creaza proiectul de studio

		//			CreateMetadataFolder(package.Location, package.PathToPrjFile);
		//			//_projectsController?.RefreshProjects();
		//		}

		//		if (_messageModel != null)
		//		{
		//			_messageModel.IsProjectCreated = true;
		//			_messageModel.Message = "Project was successfully created!";
		//			_messageModel.Title = "Informative message";
		//		}
		//	}

		//	return (_messageModel, newProject);
		//}

		//private MessageModel SetLanguagePairInformation(
		//	IProject newProject,
		//	PackageModel package)
		//{
		//	if (package?.LanguagePairs != null)
		//	{
		//		foreach (var pair in package.LanguagePairs)
		//		{
		//			//this will not work we need to change the flow, because we are in the foreach this message is not displayed to the user
		//			if (!pair.TargetFile.Any() || pair.TargetFile.Count == 0)
		//			{
		//				_messageModel.IsProjectCreated = false;
		//				_messageModel.Message =
		//					"Project was not created correctly because no target files were found in the package!";
		//				_messageModel.Title = "Informative message";
		//				return _messageModel;
		//			}

		//			if (pair.CreateNewTm)
		//			{
		//				//TODO:Investigate and refactor
		//				foreach (var starTmMetadata in pair.StarTranslationMemoryMetadatas)
		//				{
		//					AddTmPenalties(package, starTmMetadata);
		//					AddMtMemories(package, starTmMetadata);
		//				}

		//				//TODO: Investigate if we really need this later we'll join the mt and tms becasue
		//				// Remove found items from pair.StarTranslationMemoryMetadatas (the remained ones are those which does not have penalties set on them)
		//				foreach (var item in _penaltiesTmsList)
		//				{
		//					pair.StarTranslationMemoryMetadatas.Remove(item);
		//				}

		//				// Remove Machine Translation memories from pair.StarTranslationMemoryMetadatas, if the user requests them, they will be imported separately, but never in the main TM
		//				pair.StarTranslationMemoryMetadatas.RemoveAll(item =>
		//					Path.GetFileName(item?.TargetFile ?? "").Contains("_AEXTR_MT_"));
		//			}

		//			// Import language pair TM if any
		//			ImportLanguagePairTm(pair, package);

		//			var targetFilesForLanguagePair = newProject.AddFiles(pair.TargetFile.ToArray());
		//			var filesIds=targetFilesForLanguagePair.GetIds();
		//			_messageModel = UpdateProjectSettings(newProject, filesIds,true);
		//		}
		//		//_projectsController?.RefreshProjects();
		//	}
		//	return _messageModel;
		//}



		//private void ImportLanguagePairTm(LanguagePair pair, PackageModel package)
		//{
		//	if (!pair.HasTm || string.IsNullOrEmpty(pair.TmPath) || string.IsNullOrEmpty(package.Location)) return;

		//	if (pair.StarTranslationMemoryMetadatas != null && pair.StarTranslationMemoryMetadatas.Any())
		//	{
		//		var newTmPath = Path.Combine(package.Location, Path.GetFileName(pair.TmPath));
		//		var importer = new TransitTmImporter(pair, newTmPath, null);

		//		importer.ImportStarTransitTm(pair.StarTranslationMemoryMetadatas, package);
		//		var providerRef = importer.GetTranslationProviderReference(newTmPath, pair);
		//		_logger.Info($"-->Import lang pair Provider Reference:{providerRef?.Uri}");
		//		if (providerRef == null) return;
		//		_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(providerRef, true, true, true));
		//	}

		//	CreateSeparateTms(pair, package.Location, package);
		//}

		// Create separate TM for each TM file on which user set penalty. The penalty is applied on top of any penalty that might be applied by the translation provider itself.
		// The name of the new TM will be the same with the one from StarTransit package
		// We'll create a single project which will contain all the tms and mt.
		//private void CreateSeparateTms(LanguagePair pair, string localProjectPath, PackageModel package)
		//{
		//	var allTransitTms = _penaltiesTmsList?.Concat(_machineTransList).ToList();

		//	if (allTransitTms != null && !allTransitTms.Any()) return;
		//	var tpReference = CreateTpReference(allTransitTms, pair, localProjectPath, package);
		//	foreach (var tpRef in tpReference)
		//	{
		//		_tmConfig.Entries.Add(new TranslationProviderCascadeEntry(tpRef.Key, true, true, true,tpRef.Value));
		//	}
		//}

		//private Dictionary<TranslationProviderReference, int> CreateTpReference(List<StarTranslationMemoryMetadata> tmsList, LanguagePair pair,string localProjectPath, PackageModel package)
		//{
		//	var importer = new TransitTmImporter(pair, localProjectPath, tmsList);
		//	importer.ImportStarTransitTm(tmsList, package);
		//	var translationProvRef = new Dictionary<TranslationProviderReference, int>();

		//	foreach (var tm in importer.StudioTranslationMemories)
		//	{
		//		var provider = new TranslationProviderReference(tm.Key.FilePath);
		//		if (!translationProvRef.ContainsKey(provider))
		//		{
		//			translationProvRef.Add(provider,tm.Value);
		//		}
		//	}
		//	return translationProvRef;
		//}

		// Separate all items from package.TMPenalties(files that are having penalties set), that are found in pair.StarTranslationMemoryMetadatas
		//private void AddTmPenalties(PackageModel package, StarTranslationMemoryMetadata starTmMetadata)
		//{
		//	if (package?.TMPenalties == null) return;
		//	if (package.TMPenalties.Any(t => t.Key.Equals(starTmMetadata.TargetFile)))
		//	{
		//		starTmMetadata.TmPenalty = package.TMPenalties.FirstOrDefault(t => t.Key.Equals(starTmMetadata.TargetFile)).Value;
		//		_penaltiesTmsList.Add(starTmMetadata);
		//	}
		//}

		////Separate all items from package.MachineTransMem (files that contain Machine Translation)
		//private void AddMtMemories(PackageModel package, StarTranslationMemoryMetadata starTmMetadata)
		//{
		//	if (package?.MTMemories == null) return;
		//	var hasMtMemories = package.MTMemories.Any(t => t.Equals(starTmMetadata.TargetFile));
		//	if (!hasMtMemories) return;
		//	starTmMetadata.TmPenalty = 1;
		//	_machineTransList.Add(starTmMetadata);
		//}


	}
}