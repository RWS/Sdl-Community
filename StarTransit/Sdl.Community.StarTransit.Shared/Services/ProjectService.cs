using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NLog;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Import;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;
using Sdl.ProjectAutomation.Settings;
using Sdl.Versioning;
using TaskStatus = Sdl.ProjectAutomation.Core.TaskStatus;

namespace Sdl.Community.StarTransit.Shared.Services
{
	public class ProjectService:IProjectService
	{
		private TranslationProviderConfiguration _tmConfig;
		private MessageModel _messageModel;
		private readonly string _iconPath;
		private readonly IFileService _fileService;
		private readonly IEventAggregatorService _eventAggregatorService;
		private readonly Logger _logger = LogManager.GetCurrentClassLogger();
		private readonly string _initialFolderPath;

		public ProjectService()
		{
			_fileService = new FileService();
			_messageModel = new MessageModel();
			_tmConfig = new TranslationProviderConfiguration();
			var studioVersion = new StudioVersionService().GetStudioVersion();
			_initialFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), studioVersion.StudioDocumentsFolderName, "Translation Memories");
		}

		public ProjectService(IEventAggregatorService eventAggregatorService):this()
		{
			_eventAggregatorService = eventAggregatorService;
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

		public virtual MessageModel UpdateProjectSettings(IProject project, Guid[] targetFilesIds,
			bool projectContainsTm,CultureInfo targetLanguage)
		{
			var fileBasedProject = (FileBasedProject) project;

			if (projectContainsTm)
			{
				UpdateTmSettings(project);
				fileBasedProject.UpdateTranslationProviderConfiguration(_tmConfig);
			}

			var projectCreationProgress = new ProjectCreationProgress { BatchTaskIds = new List<string>()};

			project.RunAutomaticTask(targetFilesIds, AutomaticTaskTemplateIds.Scan);
			var taskSequence = fileBasedProject.RunAutomaticTasks(targetFilesIds,
				new[]
				{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages, AutomaticTaskTemplateIds.PreTranslateFiles,
				}, (sender, args) => {
					projectCreationProgress.BatchTaskIds.Clear();
					projectCreationProgress.BatchTaskIds.AddRange(args.TaskTemplateIds);
					projectCreationProgress.Progress = args.PercentComplete;
					projectCreationProgress.TargetLanguage = targetLanguage;
					_eventAggregatorService.PublishEvent(projectCreationProgress);
				}, (sender, args) => { });

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

				throw new Exception("Project could not be created.Error occurred while running create project automatic task.");
			}

			fileBasedProject.Save();
			return _messageModel;
		}

		public Task<IProject> CreateStudioProject(PackageModel transitPackage)
		{
			return System.Threading.Tasks.Task.Run(() =>
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
					_messageModel = UpdateProjectSettings(studioProject, filesIds, projectContainsTm,languagePair.TargetLanguage); // runs Studio batchtask to save all the changes to the project
				}
				CreateMetadataFolder(transitPackage.Location, transitPackage.PathToPrjFile);
				return studioProject;
			});
		}

		private IProject PrepareStudioProject(PackageModel transitPackage)
		{
			var target = _fileService.GetStudioTargetLanguages(transitPackage.LanguagePairs);
			Directory.CreateDirectory(transitPackage.Location); //if the location is set using project template feature this folder does not exist 
			var projectInfo = new ProjectInfo
			{
				Name = transitPackage.Name,
				LocalProjectFolder = transitPackage.Location,
				SourceLanguage = LanguageRegistryApi.Instance.GetLanguage(transitPackage.LanguagePairs[0].SourceLanguage.Name),
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
				if (languagePair.CreateNewTm)
				{
					//Only for create new tm option we need to check for duplicated name, for Browse existing tm we'll import into selected tm
					metadataTransitFile.LocalTmCreationPath = GetTmPathForDuplicatedName(metadataTransitFile.LocalTmCreationPath, _tmConfig.Entries);
				}

				var importer = new TransitTmImporter(languagePair, tmDescription, metadataTransitFile.LocalTmCreationPath,_eventAggregatorService);
				importer.ImportStarTransitTm(metadataTransitFile.TransitTmsSourceFilesPath,
					metadataTransitFile.TransitTmsTargeteFilesPath, languagePair.TargetLanguage, package);

				var providerRef = importer.GetTranslationProviderReference();
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
	}
}