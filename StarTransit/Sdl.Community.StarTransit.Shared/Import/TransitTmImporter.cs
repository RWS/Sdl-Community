using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.Shared.Utils;
using Sdl.Core.Globalization;
using Sdl.FileTypeSupport.Framework.Core.Utilities.NativeApi;
using Sdl.FileTypeSupport.Framework.IntegrationApi;
using Sdl.LanguagePlatform.Core.Tokenization;
using Sdl.LanguagePlatform.TranslationMemory;
using Sdl.LanguagePlatform.TranslationMemoryApi;
using Sdl.ProjectAutomation.Core;
using Sdl.ProjectAutomation.FileBased;

namespace Sdl.Community.StarTransit.Shared.Import
{
	public class TransitTmImporter
	{
		private readonly IFileTypeManager _fileTypeManager;
		private readonly FileBasedTranslationMemory _fileBasedTM;
		private readonly IFileService _fileService;

		public TransitTmImporter(LanguagePair pair,	IFileTypeManager fileTypeManager, string studioTranslationMemory)
		{
			_fileTypeManager = fileTypeManager;
			_fileService = new FileService();

			if (pair.CreateNewTm)
			{
				_fileBasedTM = new FileBasedTranslationMemory(
					studioTranslationMemory,
					string.Empty,
					pair.SourceLanguage,
					pair.TargetLanguage,
					GetFuzzyIndexes(),
					GetRecognizers(),
					TokenizerFlags.DefaultFlags,
					GetWordCountFlags());
			}
			else
			{
				_fileBasedTM = new FileBasedTranslationMemory(pair.TmPath);
			}
		}

		public TransitTmImporter(IFileTypeManager fileTypeManager, LanguagePair pair, string projectPath, string fileName)
		{
			_fileTypeManager = fileTypeManager;

			_fileBasedTM = new FileBasedTranslationMemory(
						Path.Combine(projectPath, string.Concat(fileName, ".sdltm")),
						string.Concat(fileName, " description"),
						pair.SourceLanguage,
						pair.TargetLanguage,
						GetFuzzyIndexes(),
				 		GetRecognizers(),
						TokenizerFlags.DefaultFlags,
						GetWordCountFlags());

			_fileBasedTM.LanguageResourceBundles.Clear();
			_fileBasedTM.Save();
			TMFilePath = _fileBasedTM.FilePath;
		}

		public string TMFilePath { get; set; }

		public void ImportStarTransitTm(List<StarTranslationMemoryMetadata> starTransitTms, PackageModel package)
		{
			var sdlXliffFolderFullPath = CreateTemporarySdlXliffs(starTransitTms, package);
			ImportSdlXliffIntoTm(sdlXliffFolderFullPath,package);
		}

		public TranslationProviderReference GetTranslationProviderReference()
		{
			return new TranslationProviderReference(_fileBasedTM.FilePath, true);
		}

		private void ImportSdlXliffIntoTm(string sdlXliffFilderPath, PackageModel package)
		{
			try
			{
				var tmImporter = new TranslationMemoryImporter(_fileBasedTM.LanguageDirection);
				var importSettings = new ImportSettings
				{
					IsDocumentImport = false,
					CheckMatchingSublanguages = false,
					IncrementUsageCount = true,
					NewFields = ImportSettings.NewFieldsOption.Ignore,
					PlainText = false,
					ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.AddNew
				};
				tmImporter.ImportSettings = importSettings;
				var targetLanguages = package.LanguagePairs.Select(f => f.TargetLanguage.Name).ToList();

				foreach (var languageCode in targetLanguages)
				{
					var folderPath = Path.Combine(sdlXliffFilderPath, languageCode);
					var xliffFiles = Directory.GetFiles(folderPath);
					foreach (var xliffFile in xliffFiles)
					{
						tmImporter.Import(xliffFile);
					}

				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ImportSdlXliffIntoTm method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Create temporary bilingual files (sdlxliff) used to import the information
		/// in Studio translation memories
		/// </summary>
		/// TODO: Write UT
		private string CreateTemporarySdlXliffs(List<StarTranslationMemoryMetadata> starTransitTms, PackageModel package)
		{
			var pathToExtractFolder = CreateFolderToExtract(Path.GetDirectoryName(starTransitTms[0].TargetFile));

			//TODO: Create studio project based on the selected tms and return the xliff path
			var sourceTmFiles = starTransitTms.Select(s => s.SourceFile).ToArray();
			var targetTmFiles = starTransitTms.Select(t => t.TargetFile).ToArray();
			var target = _fileService.GetStudioTargetLanguages(package.LanguagePairs);

			var projectInfo = new ProjectInfo
			{
				Name = $"TMExtractProject_{Guid.NewGuid()}",
				LocalProjectFolder = pathToExtractFolder, //package.Location,
				SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
				TargetLanguages = target
			};

			var newProject =
				new FileBasedProject(projectInfo,new ProjectTemplateReference(package.ProjectTemplate.Uri)); // TODO: Use interface for final implementation
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
			return pathToExtractFolder;
		}

		/// <summary>
		/// Create temporary folder for TM import
		/// </summary>
		private string CreateFolderToExtract(string pathToTemp)
		{
			var pathToExtractFolder = Path.Combine(pathToTemp, "TmExtract");
			Directory.CreateDirectory(pathToExtractFolder);

			return pathToExtractFolder;
		}

		private static FuzzyIndexes GetFuzzyIndexes()
		{
			return FuzzyIndexes.SourceCharacterBased |
				FuzzyIndexes.SourceWordBased |
				FuzzyIndexes.TargetCharacterBased |
				FuzzyIndexes.TargetWordBased;
		}

		private static BuiltinRecognizers GetRecognizers()
		{
			return BuiltinRecognizers.RecognizeAll;
		}

		private static WordCountFlags GetWordCountFlags()
		{
			return WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe;
		}
	}
}
