using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NLog;
using Sdl.Community.StarTransit.Shared.Events;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;
using Sdl.Core.Globalization.LanguageRegistry;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitTmImporter
	{
		private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();
		private readonly FileBasedTranslationMemory _translationMemory;
		private readonly IEventAggregatorService _eventAggregator;

		public TransitTmImporter(LanguagePair languagePair, string description, string tmPath,IEventAggregatorService eventAggregator)
		{
			_eventAggregator = eventAggregator;
			_translationMemory = new FileBasedTranslationMemory(
				tmPath,
				description,
				languagePair.SourceLanguage,
				languagePair.TargetLanguage,
				FuzzyIndexes.SourceCharacterBased | FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetCharacterBased |
				FuzzyIndexes.TargetWordBased,
				BuiltinRecognizers.RecognizeAll,
				TokenizerFlags.DefaultFlags,
				WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);

			_translationMemory.Save();
		}

		public void ImportStarTransitTm(List<string>sourceTmFiles,List<string>targetTmFiles, CultureInfo targetLanguage, PackageModel package)
		{
			var sdlXliffFolderFullPath = CreateTemporarySdlXliffs(sourceTmFiles, targetTmFiles, targetLanguage,package);
			ImportSdlXliffsIntoTm(sdlXliffFolderFullPath, targetLanguage);
		}

		public TranslationProviderReference GetTranslationProviderReference()
		{
			return new TranslationProviderReference(_translationMemory.FilePath, true);
		}

		private void ImportSdlXliffsIntoTm(string sdlXliffFolderPath,CultureInfo targetLanguage)
		{
			try
			{
				ConfirmationLevel[] levels = { ConfirmationLevel.ApprovedTranslation, ConfirmationLevel.Translated, ConfirmationLevel.ApprovedSignOff };

				var importSettings = new ImportSettings
				{
					IsDocumentImport = true,
					CheckMatchingSublanguages = false,
					IncrementUsageCount = true,
					NewFields = ImportSettings.NewFieldsOption.AddToSetup,
					PlainText = false,
					ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.AddNew,
					ConfirmationLevels = levels
				};
				var tmImporter = new TranslationMemoryImporter(_translationMemory.LanguageDirection)
				{
					ImportSettings = importSettings
				};

				var folderPath = Path.Combine(sdlXliffFolderPath, targetLanguage.Name);
				if (!Directory.Exists(folderPath)) return;
				var xliffFiles = Directory.GetFiles(folderPath);
				var tmFileProgress = new TmFilesProgress
				{
					TotalFilesNumber = xliffFiles.Length,
					TargetLanguage = targetLanguage
				};

				foreach (var xliffFile in xliffFiles)
				{
					tmFileProgress.ProcessingFileNumber++;
					tmImporter.Import(xliffFile);
					_eventAggregator?.PublishEvent(tmFileProgress);
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

		/// <summary>
		/// Create temporary bilingual files (sdlxliff) used to import the information in Studio translation memories
		/// </summary>
		private string CreateTemporarySdlXliffs(List<string> sourceTmFiles, List<string> targetTmFiles, CultureInfo targetCultureInfo, PackageModel package)
		{
			var pathToExtractFolder = CreateFolderToExtract(Path.GetDirectoryName(targetTmFiles[0]));
			var targetLang = LanguageRegistryApi.Instance.GetLanguage(targetCultureInfo.Name);
			var projectInfo = new ProjectInfo
			{
				Name = $"TMExtractProject_{Guid.NewGuid()}",
				LocalProjectFolder = pathToExtractFolder,
				SourceLanguage = LanguageRegistryApi.Instance.GetLanguage(package.LanguagePairs[0].SourceLanguage.Name),
				TargetLanguages = new[] {targetLang} 
			};

			var newProject =
				new FileBasedProject(projectInfo, new ProjectTemplateReference(package.ProjectTemplate.Uri));
			newProject.AddFiles(sourceTmFiles.ToArray());
			var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
			newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);

			var targetTms = newProject.AddFiles(targetTmFiles.ToArray());
			newProject.RunAutomaticTask(targetTms?.GetIds(), AutomaticTaskTemplateIds.Scan);

			var xliffCreationProgress = new XliffCreationProgress
			{
				BatchTaskIds = new List<string>(), TargetLanguage = targetLang.CultureInfo,TotalFileNumber = sourceTmFiles.Count
			};

			var taskSequence = newProject.RunAutomaticTasks(targetTms?.GetIds(),
				new[]
				{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages
				}, (sender, args) =>
				{
					xliffCreationProgress.BatchTaskIds.Clear();
					xliffCreationProgress.BatchTaskIds.AddRange(args.TaskTemplateIds);
					xliffCreationProgress.Progress = args.PercentComplete;
					_eventAggregator.PublishEvent(xliffCreationProgress);
				}, (sender, args) => { });

			if (taskSequence.Status != TaskStatus.Failed) return pathToExtractFolder;

			foreach (var subTask in taskSequence.SubTasks)
			{
				_logger.Error($"Name:{subTask.Name}");

				foreach (var messages in subTask.Messages)
				{
					_logger.Error($"Exception: {messages?.Exception}");
					_logger.Error($"Message: {messages?.Message}");
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// Create temporary folder for TM import
		/// </summary>
		private string CreateFolderToExtract(string pathToTemp)
		{
			var pathToExtractFolder = Path.Combine(pathToTemp, "TmExtract",Guid.NewGuid().ToString());

			try
			{
				Directory.CreateDirectory(pathToExtractFolder);
			}
			catch (Exception e)
			{
				_logger.Error(e);
			}
			_logger.Info($"--> Temp Path folder for Studio project where we extract the tms: {pathToExtractFolder}");

			return pathToExtractFolder;
		}
	}
}
