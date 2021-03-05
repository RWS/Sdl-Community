using System;
using System.Collections.Generic;
using System.IO;
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

		public void ImportStarTransitTm(StarTranslationMemoryMetadata starTransitTm, PackageModel package)
		{
			try
			{
				var sdlXliffFullPath = CreateTemporarySdlXliff(starTransitTm,package);
				ImportSdlXliffIntoTm(sdlXliffFullPath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ImportStarTransitTm method: {ex.Message}\n {ex.StackTrace}");
			}
		}

	    public TranslationProviderReference GetTranslationProviderReference()
		{
			return new TranslationProviderReference(_fileBasedTM.FilePath, true);
		}

		private void ImportSdlXliffIntoTm(string sdlXliffFullPath)
		{
			try
			{
				var tmImporter = new TranslationMemoryImporter(_fileBasedTM.LanguageDirection);
				var importSettings = new ImportSettings
				{
					IsDocumentImport = false,
					CheckMatchingSublanguages = false,
					IncrementUsageCount = false,
					NewFields = ImportSettings.NewFieldsOption.Ignore,
					PlainText = false,
					ExistingTUsUpdateMode = ImportSettings.TUUpdateMode.AddNew
				};
				tmImporter.ImportSettings = importSettings;
				tmImporter.Import(sdlXliffFullPath);
			}
			catch (Exception ex)
			{
				Log.Logger.Error($"ImportSdlXliffIntoTm method: {ex.Message}\n {ex.StackTrace}");
			}
		}

		/// <summary>
		/// Create temporary bilingual file (sdlxliff) used to import the information
		/// in Studio translation memories
		/// </summary>
		/// TODO: Write UT
		private string CreateTemporarySdlXliff(StarTranslationMemoryMetadata starTransitTM, PackageModel package)
		{
			var pathToExtractFolder = CreateFolderToExtract(Path.GetDirectoryName(starTransitTM.TargetFile));
			var generatedXliffName = $"{Path.GetFileNameWithoutExtension(starTransitTM.TargetFile)}{".sdlxliff"}";

			var sdlXliffFullPath = Path.Combine(pathToExtractFolder, generatedXliffName);

			//TODO: Create studio project based on the selected tms and return the xliff path
			var target = _fileService.GetStudioTargetLanguages(package.LanguagePairs);

			var projectInfo = new ProjectInfo
			{
				Name = "TMTestProject",
				LocalProjectFolder = @"C:\Users\aghisa\Desktop\TestProject", //package.Location,
				SourceLanguage = new Language(package.LanguagePairs[0].SourceLanguage),
				TargetLanguages = target
			};
			var newProject =
				new FileBasedProject(projectInfo,
					new ProjectTemplateReference(package.ProjectTemplate
						.Uri)); // TODO: Use interface for final implementation
			newProject.AddFiles(new[] {starTransitTM.SourceFile});
			var sourceFilesIds = newProject.GetSourceLanguageFiles().GetIds();
			newProject.SetFileRole(sourceFilesIds, FileRole.Translatable);

			var targetTms = newProject.AddFiles(new[] {starTransitTM.TargetFile});
			newProject.RunAutomaticTask(targetTms?.GetIds(), AutomaticTaskTemplateIds.Scan);
			var taskSequence = newProject.RunAutomaticTasks(targetTms?.GetIds(),
				new[]
				{
					AutomaticTaskTemplateIds.ConvertToTranslatableFormat,
					AutomaticTaskTemplateIds.CopyToTargetLanguages
				});

			return sdlXliffFullPath; //Return target folde path
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
