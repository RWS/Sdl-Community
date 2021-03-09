using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sdl.Community.StarTransit.Shared.Models;
using Sdl.Community.StarTransit.Shared.Services;
using Sdl.Community.StarTransit.Shared.Services.Interfaces;
using Sdl.Community.StarTransit.Shared.Utils;
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
		private readonly FileBasedTranslationMemory _fileBasedTM;
		private readonly IFileService _fileService = new FileService();
		public Dictionary<FileBasedTranslationMemory, int> StudioTranslationMemories= new Dictionary<FileBasedTranslationMemory, int>();

		public TransitTmImporter(LanguagePair pair, string studioTranslationMemory)
		{

			if (pair.CreateNewTm)
			{
				//TODO: use existing method to create tm
				_fileBasedTM = new FileBasedTranslationMemory(
					studioTranslationMemory,
					string.Empty,
					pair.SourceLanguage,
					pair.TargetLanguage,
					FuzzyIndexes.SourceCharacterBased | FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetCharacterBased | FuzzyIndexes.TargetWordBased,
					BuiltinRecognizers.RecognizeAll,
					TokenizerFlags.DefaultFlags,
					WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);
			}
			else
			{
				//TODO: vezi cand intra aici
				_fileBasedTM = new FileBasedTranslationMemory(pair.TmPath);
			}
		}

		public TransitTmImporter(LanguagePair languagePair, string studioProjectPath, List<StarTranslationMemoryMetadata> tmsList)
		{
			if (tmsList != null && tmsList.Any())
			{
				foreach (var transitTm in tmsList)
				{
					var transitTmName = Path.GetFileName(transitTm.TargetFile);
					var tmPath = Path.Combine(studioProjectPath, string.Concat(transitTmName, ".sdltm"));
					CreateStudioTranslationMemory(tmPath, $"{transitTmName} description", transitTm.TMPenalty, languagePair);
				}
			}
			else
			{
				// sa vad cum arata pt mt pennalty sipathu
				//CreateStudioTranslationMemory(studioProjectPath, string.Empty, 1)
			}

		}

		private void CreateStudioTranslationMemory(string tmPath, string description,int penalty,LanguagePair pair)
		{
			var fileBasedTm = new FileBasedTranslationMemory(
				tmPath,
				description,
				pair.SourceLanguage,
				pair.TargetLanguage,
				FuzzyIndexes.SourceCharacterBased | FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetCharacterBased | FuzzyIndexes.TargetWordBased,
				BuiltinRecognizers.RecognizeAll,
				TokenizerFlags.DefaultFlags,
				WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);
			fileBasedTm.Save();

			if (!StudioTranslationMemories.ContainsKey(fileBasedTm))
			{
				StudioTranslationMemories.Add(fileBasedTm, penalty);
			}
		}

		//TODO: Adapteaza sa mearga si in cazul in care cream doar un tm, primul constructor
		private void CreateStudioTranslationMemory(StarTranslationMemoryMetadata tmMetadata,LanguagePair pair,string studioProjectPath )
		{
			//TODO muta in cosntructor path si ce mai e
			var transitTmName = Path.GetFileName(tmMetadata.TargetFile);
			var tmPath = Path.Combine(studioProjectPath, string.Concat(transitTmName, ".sdltm"));

			var fileBasedTm = new FileBasedTranslationMemory(
				tmPath,
				string.Concat(transitTmName, " description"),
				pair.SourceLanguage,
				pair.TargetLanguage,
				FuzzyIndexes.SourceCharacterBased | FuzzyIndexes.SourceWordBased | FuzzyIndexes.TargetCharacterBased | FuzzyIndexes.TargetWordBased,
				BuiltinRecognizers.RecognizeAll,
				TokenizerFlags.DefaultFlags,
				WordCountFlags.BreakOnTag | WordCountFlags.BreakOnDash | WordCountFlags.BreakOnApostrophe);
			fileBasedTm.Save();

			if (!StudioTranslationMemories.ContainsKey(fileBasedTm))
			{
				StudioTranslationMemories.Add(fileBasedTm,tmMetadata.TMPenalty);
			}
		}

		public string TMFilePath { get; set; }

		public void ImportStarTransitTm(List<StarTranslationMemoryMetadata> starTransitTms, PackageModel package)
		{
			var sdlXliffFolderFullPath = CreateTemporarySdlXliffs(starTransitTms, package);
			ImportSdlXliffIntoTm(sdlXliffFolderFullPath,package);
		}

		//TODO: Returneaza o lista de TP Reference
		public TranslationProviderReference GetTranslationProviderReference()
		{
			return new TranslationProviderReference(_fileBasedTM.FilePath, true);
		}

		private void ImportSdlXliffIntoTm(string sdlXliffFolderPath, PackageModel package)
		{
			try
			{
				var targetLanguages = package.LanguagePairs.Select(f => f.TargetLanguage.Name).ToList();

				foreach (var languageCode in targetLanguages)
				{
					var folderPath = Path.Combine(sdlXliffFolderPath, languageCode);
					var xliffFiles = Directory.GetFiles(folderPath);

					foreach (var xliffFile in xliffFiles)
					{
						var fileName = Path.GetFileNameWithoutExtension(xliffFile);
						var correspondingTm =
							StudioTranslationMemories.Keys.FirstOrDefault(t => t.Name.Equals(fileName));
						if(correspondingTm is null) return;

						var tmImporter = new TranslationMemoryImporter(correspondingTm.LanguageDirection);
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
						tmImporter.Import(xliffFile);
					}
				}
			}
			catch (Exception ex)
			{
				Log.Logger.Error(ex);
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
	}
}
