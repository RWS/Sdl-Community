using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Core.Globalization;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitTmImporter
	{
		private readonly IFileService _fileService = new FileService();
		public Dictionary<FileBasedTranslationMemory, int> StudioTranslationMemories= new Dictionary<FileBasedTranslationMemory, int>();
		private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

		public TransitTmImporter(LanguagePair languagePair, string studioProjectPath, List<StarTranslationMemoryMetadata> tmsList)
		{
			if (tmsList != null && tmsList.Any())
			{
				//TM or star transit MTs which have penalties applied in the UI
				foreach (var transitTm in tmsList)
				{
					var transitTmName = Path.GetFileName(transitTm.TargetFile);
					var tmPath = Path.Combine(studioProjectPath, string.Concat(transitTmName, ".sdltm"));
					CreateStudioTranslationMemory(tmPath, $"{transitTmName} description", transitTm.TMPenalty, languagePair);
				}
			}
			else
			{
				if (languagePair.CreateNewTm)
				{ 
					//One general tm which includes all tms (tms which doesn't have penalty set in the UI
					CreateStudioTranslationMemory(studioProjectPath, string.Empty, 0, languagePair);
				}
				else
				{
					//Tm selected from local machine
					CreateStudioTranslationMemory(languagePair.TmPath, string.Empty, 0, null);
				}
			}
		}

		private void CreateStudioTranslationMemory(string tmPath, string description,int penalty,LanguagePair languagePair)
		{
			FileBasedTranslationMemory fileBasedTm;
			if (languagePair != null)
			{
				fileBasedTm = new FileBasedTranslationMemory(
					tmPath,
					description,
					languagePair.SourceLanguage,
					languagePair.TargetLanguage,
					FuzzyIndexes.SourceCharacterBased | FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetCharacterBased | FuzzyIndexes.TargetWordBased,
					BuiltinRecognizers.RecognizeAll,
					TokenizerFlags.DefaultFlags,
					WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);
			}
			else
			{
				fileBasedTm = new FileBasedTranslationMemory(tmPath);
			}
			
			fileBasedTm.Save();

			if (!StudioTranslationMemories.ContainsKey(fileBasedTm))
			{
				StudioTranslationMemories.Add(fileBasedTm, penalty);
			}
		}
		
		public void ImportStarTransitTm(List<StarTranslationMemoryMetadata> starTransitTms, PackageModel package)
		{
			var sdlXliffFolderFullPath = CreateTemporarySdlXliffs(starTransitTms, package);
			ImportSdlXliffIntoTm(sdlXliffFolderFullPath,package);
		}

		public TranslationProviderReference GetTranslationProviderReference(string tmPath,LanguagePair languagePair)
		{
			var tm = StudioTranslationMemories.FirstOrDefault(t => t.Key.FilePath.Equals(tmPath));
			if (tm.Key is null)
			{
				// tm was selected from local disk
				tm = StudioTranslationMemories.FirstOrDefault(t => t.Key.FilePath.Equals(languagePair.TmPath));
			}
			return tm.Key != null ? new TranslationProviderReference(tm.Key.FilePath, true) : null;
		}

		private void ImportSdlXliffIntoTm(string sdlXliffFolderPath, PackageModel package)
		{
			try
			{
				var targetLanguages = package.LanguagePairs.Select(f => f.TargetLanguage.Name).ToList();
				var importSettings = new ImportSettings
				{
					IsDocumentImport = false,
					CheckMatchingSublanguages = false,
					IncrementUsageCount = true,
					NewFields = ImportSettings.NewFieldsOption.AddToSetup,
					PlainText = false,
					ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.AddNew
				};
				foreach (var languageCode in targetLanguages)
				{
					var folderPath = Path.Combine(sdlXliffFolderPath, languageCode);
					var xliffFiles = Directory.GetFiles(folderPath);

					foreach (var xliffFile in xliffFiles)
					{
						var fileName = Path.GetFileNameWithoutExtension(xliffFile);
						// for tms list or MT list the name of star transit tm corresponds to Studio tm
						// for general tm the name of the tm is the name of the package. In constructor we set 0 as penalty for general tms
						var correspondingTm =
							StudioTranslationMemories.Keys.FirstOrDefault(t => t.Name.Equals(fileName)) ??
							StudioTranslationMemories.FirstOrDefault(s => s.Value.Equals(0)).Key;

						if (correspondingTm is null) return;
						var tmImporter = new TranslationMemoryImporter(correspondingTm.LanguageDirection)
						{
							ImportSettings = importSettings
						};

						tmImporter.Import(xliffFile);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex);
			}
		}

		/// <summary>
		/// Create temporary bilingual files (sdlxliff) used to import the information
		/// in Studio translation memories
		/// </summary>
		private string CreateTemporarySdlXliffs(List<StarTranslationMemoryMetadata> starTransitTms, PackageModel package)
		{
			var pathToExtractFolder = CreateFolderToExtract(Path.GetDirectoryName(starTransitTms[0].TargetFile));

			var sourceTmFiles = starTransitTms.Select(s => s.SourceFile).ToArray();
			var targetTmFiles = starTransitTms.Select(t => t.TargetFile).ToArray();
			var target = _fileService.GetStudioTargetLanguages(package.LanguagePairs);

			var projectInfo = new ProjectInfo
			{
				Name = $"TMExtractProject_{Guid.NewGuid()}",
				LocalProjectFolder = pathToExtractFolder,
				SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
				TargetLanguages = target
			};

			var newProject =
				new FileBasedProject(projectInfo,new ProjectTemplateReference(package.ProjectTemplate.Uri));
			newProject.AddFiles(sourceTmFiles);
			var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
			newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);

			var targetTms = newProject.AddFiles(targetTmFiles);
			newProject.RunAutomaticTask(targetTms?.GetIds(), AutomaticTaskTemplateIds.Scan);

			var taskSequence = newProject.RunAutomaticTasks(targetTms?.GetIds(),
				new[]
				{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages
				});

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
			throw new Exception("Failed to create xliff for corresponding Transit TMs");
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
