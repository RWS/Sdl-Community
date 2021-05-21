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

		public TransitTmImporter(LanguagePair languagePair, string description, string tmPath, int penalty)
		{
			FileBasedTranslationMemory fileBasedTm;

			//TODO: Refactor we don't need else
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

		public void ImportStarTransitTm(List<string>sourceTmFiles,List<string>targetTmFiles, int penalty, PackageModel package)
		{
			var sdlXliffFolderFullPath = CreateTemporarySdlXliffs(sourceTmFiles, targetTmFiles,package);
			ImportSdlXliffIntoTm(sdlXliffFolderFullPath,penalty,package);
		}
		//TODO: investigate this I don't think we need a list
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

		//TODO: We should receive the target language code and not iterating
		private void ImportSdlXliffIntoTm(string sdlXliffFolderPath,int penalty, PackageModel package)
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
					if (Directory.Exists(folderPath))
					{
						var xliffFiles = Directory.GetFiles(folderPath);
						foreach (var xliffFile in xliffFiles)
						{
							var fileName = Path.GetFileNameWithoutExtension(xliffFile);
							//TODO: investigate this it might not be required
							// for tms list or MT list the name of star transit tm corresponds to Studio tm
							// for general tm the name of the tm is the name of the package. In constructor we set 0 as penalty for general tms
							var correspondingTm =
								StudioTranslationMemories.Keys.FirstOrDefault(t => t.Name.Equals(fileName)) ??
								StudioTranslationMemories.FirstOrDefault(s => s.Value.Equals(penalty)).Key;

							if (correspondingTm is null) return;
							var tmImporter = new TranslationMemoryImporter(correspondingTm.LanguageDirection)
							{
								ImportSettings = importSettings
							};

							tmImporter.Import(xliffFile);
						}
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
		private string CreateTemporarySdlXliffs(List<string>sourceTmFiles,List<string>targetTmFiles, PackageModel package)
		{
			var pathToExtractFolder = CreateFolderToExtract(Path.GetDirectoryName(targetTmFiles[0]));

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
			newProject.AddFiles(sourceTmFiles.ToArray());
			var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
			newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);

			var targetTms = newProject.AddFiles(targetTmFiles.ToArray());
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
